using System;
using System.Collections;
using System.IO;
using NarrativeGen;
using ProjectFoundPhone.UI;
using UnityEngine;

namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// Plays a NarrativeGen model in the chat. The SDK's ScreenFrameBuilder decides each
    /// screen (the same frames as NarrativeGen Studio); NarrativeGenChatScreen draws it.
    /// The preview is an in-memory session and never touches Yarn or FoundPhone saves.
    /// </summary>
    internal sealed class NarrativeGenPlayback
    {
        private readonly ScreenFrameBuilder m_Screen;
        private readonly ChatController m_Chat;

        public NarrativeGenPlayback(string file, ChatController chat)
        {
            m_Screen = new ScreenFrameBuilder(LoadModel(file));
            m_Chat = chat != null ? chat : throw new ArgumentNullException(nameof(chat));
        }

        internal static void ValidateModelFile(string file)
        {
            _ = new ScreenFrameBuilder(LoadModel(file)).Current;
        }

        private static NarrativeModel LoadModel(string file) => Engine.LoadModel(File.ReadAllText(file));

        public IEnumerator Play()
        {
            // Use the chat after its Start has initialised the input and bubble pool.
            yield return null;
            m_Chat.SetInputEnabled(false);
            m_Chat.ClearMessages();

            ScreenFrame previous = null;
            while (true)
            {
                ScreenFrame frame = m_Screen.Current;
                Show(frame, previous);
                var choices = NarrativeGenChatScreen.Choices(frame);
                if (frame.Ending || choices.Count == 0)
                {
                    yield break;
                }

                int selection = -1;
                m_Chat.ShowChoices(choices.ConvertAll(c => c.Label), index =>
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

                // The existing button handler fades and clears its prompt after invoking the callback.
                // Wait for that cleanup before installing the next set of choices.
                while (m_Chat.IsChoicePromptVisible)
                {
                    yield return null;
                }

                previous = frame;
                try
                {
                    m_Screen.Choose(choices[selection].Id);
                }
                catch (Exception error)
                {
                    ReportPlaybackError(error);
                    yield break;
                }
            }
        }

        private void Show(ScreenFrame frame, ScreenFrame previous)
        {
            foreach (var line in NarrativeGenChatScreen.Lines(frame, previous))
            {
                if (line.Kind == NarrativeGenChatScreen.LineKind.System)
                {
                    m_Chat.AddSystemMessage(line.Text);
                }
                else
                {
                    m_Chat.AddMessage(line.CharacterId, line.Text);
                }
            }
        }

        private void ReportPlaybackError(Exception error)
        {
            m_Chat.AddSystemMessage("NarrativeGenの再生を終了しました。詳細はUnityのConsoleを確認してください。");
            Debug.LogException(error);
        }
    }
}
