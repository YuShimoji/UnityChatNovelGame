using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NarrativeGen;
using Session = NarrativeGen.Runtime.Session;
using ProjectFoundPhone.UI;
using UnityEngine;

namespace ProjectFoundPhone.Core
{
    internal sealed class NarrativeGenPlayback
    {
        private readonly NarrativeModel m_Model;
        private readonly ChatController m_Chat;
        private Session m_Session;
        private IReadOnlyDictionary<string, HashSet<string>> m_Descriptions;
        private IReadOnlyDictionary<string, int> m_TemplateUsage;

        public NarrativeGenPlayback(string file, ChatController chat)
        {
            m_Model = LoadModel(file);
            m_Chat = chat != null ? chat : throw new ArgumentNullException(nameof(chat));
            m_Session = Engine.StartSession(m_Model);
        }

        internal static void ValidateModelFile(string file)
        {
            var model = LoadModel(file);
            var session = Engine.StartSession(model);
            Engine.GetAvailableChoices(session, model);
        }

        private static NarrativeModel LoadModel(string file) => Engine.LoadModel(File.ReadAllText(file));

        public IEnumerator Play()
        {
            // Use the chat after its Start has initialised the input and bubble pool.
            yield return null;
            m_Chat.SetInputEnabled(false);
            m_Chat.ClearMessages();

            while (TryReadCurrentTurn(out var text, out var choices, out var labels))
            {
                m_Chat.AddSystemMessage(text);
                if (choices.Count == 0)
                {
                    m_Chat.AddSystemMessage(m_Model.Nodes[m_Session.CurrentNodeId].Choices?.Count > 0
                        ? "選べる選択肢がありません。"
                        : "再生終了");
                    yield break;
                }

                int selection = -1;
                m_Chat.ShowChoices(labels, index =>
                {
                    if (selection < 0 && index >= 0 && index < choices.Count)
                    {
                        selection = index;
                    }
                });

                while (selection < 0 && m_Chat.IsChoicePromptVisible)
                {
                    yield return null;
                }

                if (selection < 0)
                {
                    m_Chat.AddSystemMessage("選択肢を表示できませんでした。");
                    yield break;
                }

                if (!TryApply(choices[selection].Id))
                {
                    yield break;
                }

                m_Chat.AddMessage("player", labels[selection]);
                // The existing button handler fades and clears its prompt after invoking the callback.
                // Wait for that cleanup before installing the next set of choices.
                while (m_Chat.IsChoicePromptVisible)
                {
                    yield return null;
                }
            }
        }

        private bool TryReadCurrentTurn(out string text, out IReadOnlyList<Choice> choices, out List<string> labels)
        {
            try
            {
                var node = m_Model.Nodes[m_Session.CurrentNodeId];
                var display = Resolve(node.Text, appendTemplates: true);
                text = string.IsNullOrWhiteSpace(node.Speaker)
                    ? display.Text
                    : string.IsNullOrEmpty(display.Text)
                        ? node.Speaker
                        : $"{node.Speaker}\n{display.Text}";
                m_Descriptions = display.DescriptionState;
                if (!string.IsNullOrEmpty(node.Text))
                {
                    m_TemplateUsage = display.TemplateUsageState;
                }

                choices = Engine.GetAvailableChoices(m_Session, m_Model);
                labels = new List<string>(choices.Count);
                foreach (var choice in choices)
                {
                    labels.Add(Resolve(choice.Text, appendTemplates: false).Text);
                }
                return true;
            }
            catch (Exception error)
            {
                ReportPlaybackError(error);
                text = string.Empty;
                choices = Array.Empty<Choice>();
                labels = new List<string>();
                return false;
            }
        }

        private bool TryApply(string choiceId)
        {
            try
            {
                m_Session = Engine.ApplyChoice(m_Session, m_Model, choiceId);
                return true;
            }
            catch (Exception error)
            {
                ReportPlaybackError(error);
                return false;
            }
        }

        private ResolveNarrativeDisplayTextTrackedResult Resolve(string text, bool appendTemplates)
        {
            return NarrativeDisplayText.ResolveNarrativeDisplayTextTracked(
                text, m_Model, m_Session, new ResolveNarrativeDisplayTextTrackedOptions
                {
                    AppendConversationTemplates = appendTemplates,
                    DescriptionSeed = (int)m_Session.Time,
                    DescriptionState = m_Descriptions,
                    TemplateUsageState = m_TemplateUsage
                });
        }

        private void ReportPlaybackError(Exception error)
        {
            m_Chat.AddSystemMessage("NarrativeGenの再生を終了しました。詳細はUnityのConsoleを確認してください。");
            Debug.LogException(error);
        }
    }
}
