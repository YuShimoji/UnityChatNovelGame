#if YARN_SPINNER
using Yarn.Unity;
using UnityEngine;
using System.Collections.Generic;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// Yarn Spinner の DialoguePresenterBase を拡張したチャット風ダイアログビュー。
    /// DialogueRunner の出力を ChatController と連携して表示する。
    /// </summary>
    public class ChatDialogueView : DialoguePresenterBase
    {
        [SerializeField] private float m_LineDisplayDelay = 0.5f;

        private DialogueRunner m_DialogueRunner;
        private ChatController m_ChatController;

        private void Awake()
        {
            m_DialogueRunner = GetComponent<DialogueRunner>();
        }

        private void Start()
        {
            // キャッシュして毎回のFindを回避
            m_ChatController = FindFirstObjectByType<ChatController>();
        }

        public override async YarnTask RunLineAsync(LocalizedLine dialogueLine, LineCancellationToken token)
        {
            if (m_ChatController == null)
            {
                m_ChatController = FindFirstObjectByType<ChatController>();
            }

            string lineText = dialogueLine?.TextWithoutCharacterName.Text ?? string.Empty;

            if (m_ChatController != null)
            {
                // 話者解決: CharacterName → $speaker 変数 → "npc" フォールバック
                string charID = ResolveSpeaker(dialogueLine);
                m_ChatController.AddMessage(charID, lineText);
            }
            else
            {
                Debug.LogWarning($"ChatDialogueView: ChatController not found. Line: {lineText}");
            }

            // ライン表示のための遅延
            await YarnTask.Delay((int)(m_LineDisplayDelay * 1000), token.NextContentToken).SuppressCancellationThrow();
        }

        /// <summary>
        /// 話者IDを解決する。優先順位: CharacterName → $speaker 変数 → "npc" フォールバック
        /// </summary>
        private string ResolveSpeaker(LocalizedLine dialogueLine)
        {
            // 1. CharacterName を優先使用
            if (dialogueLine != null && !string.IsNullOrEmpty(dialogueLine.CharacterName))
            {
                return dialogueLine.CharacterName;
            }

            // 2. $speaker 変数をフォールバックとして使用
            if (m_DialogueRunner != null && m_DialogueRunner.VariableStorage != null)
            {
                if (m_DialogueRunner.VariableStorage.TryGetValue<string>("$speaker", out string speaker) &&
                    !string.IsNullOrEmpty(speaker))
                {
                    return speaker;
                }
            }

            // 3. デフォルトフォールバック
            return "npc";
        }

        /// <summary>
        /// 選択肢を表示し、選択が確定するまで待機する
        /// </summary>
        public override async YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] dialogueOptions, LineCancellationToken cancellationToken)
        {
            if (m_ChatController == null)
            {
                m_ChatController = FindFirstObjectByType<ChatController>();
            }

            if (m_ChatController != null)
            {
                List<string> choiceTexts = new List<string>();
                for (int i = 0; i < dialogueOptions.Length; i++)
                {
                    choiceTexts.Add(dialogueOptions[i].Line.TextWithoutCharacterName.Text);
                }

                DialogueOption selectedOption = null;
                bool choiceMade = false;

                m_ChatController.ShowChoices(choiceTexts, (index) =>
                {
                    if (index >= 0 && index < dialogueOptions.Length)
                    {
                        selectedOption = dialogueOptions[index];
                    }
                    choiceMade = true;
                });

                // 選択確定待機
                while (!choiceMade)
                {
                    if (cancellationToken.IsNextContentRequested)
                    {
                        // キャンセル時は選択肢を確実に非表示にしてUI状態を復元
                        m_ChatController.HideChoices();
                        return null;
                    }
                    await YarnTask.Yield();
                }

                return selectedOption;
            }
            else
            {
                Debug.LogWarning("ChatDialogueView: ChatController not found. Selecting default option.");
                return dialogueOptions.Length > 0 ? dialogueOptions[0] : null;
            }
        }

        /// <summary>
        /// ダイアログ開始時の処理。入力をロックし選択肢をクリアする。
        /// </summary>
        public override YarnTask OnDialogueStartedAsync()
        {
            if (m_ChatController == null)
            {
                m_ChatController = FindFirstObjectByType<ChatController>();
            }

            // ダイアログ開始時は入力を無効化し、選択肢を確実にクリア
            m_ChatController?.SetInputEnabled(false);
            m_ChatController?.HideChoices();

            return YarnTask.CompletedTask;
        }

        /// <summary>
        /// ダイアログ終了時の処理。入力を復元し選択肢をクリアする。
        /// </summary>
        public override YarnTask OnDialogueCompleteAsync()
        {
            if (m_ChatController == null)
            {
                m_ChatController = FindFirstObjectByType<ChatController>();
            }

            // ダイアログ終了時は選択肢を確実に非表示にし、入力を再有効化
            m_ChatController?.HideChoices();
            m_ChatController?.SetInputEnabled(true);

            return YarnTask.CompletedTask;
        }

        public override void OnNodeEnter(string nodeName)
        {
            if (m_DialogueRunner != null && m_DialogueRunner.VariableStorage != null)
            {
                m_DialogueRunner.VariableStorage.SetValue("$current_node", nodeName);
            }
        }
    }
}
#endif



