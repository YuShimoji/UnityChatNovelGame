using Yarn.Unity;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// Yarn Spinner の DialogueViewBase を継承したチャット専用ダイアログビュー。
    /// DialogueRunner の出力を ChatController と連携して表示する。
    /// </summary>
    public class ChatDialogueView : DialogueViewBase
    {
        private Coroutine m_WaitCoroutine;
        private bool m_IsWaitingForChoice = false;
        private System.Action<int> m_OnOptionSelected;
        private int[] m_CurrentOptionIDs;
        private DialogueRunner m_DialogueRunner;

        private void Awake()
        {
            m_DialogueRunner = GetComponent<DialogueRunner>();
        }

        /// <summary>
        /// テキスト行を表示する。
        /// </summary>
        /// <param name="dialogueLine">表示する行データ</param>
        /// <param name="onDialogueLineFinished">表示終了時のコールバック</param>
        public override void RunLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
        {
            ChatController chatController = FindFirstObjectByType<ChatController>();
            string lineText = dialogueLine?.TextWithoutCharacterName.Text ?? string.Empty;

            if (chatController != null)
            {
                string charID = "npc";

                if (m_DialogueRunner != null && m_DialogueRunner.VariableStorage != null)
                {
                    if (m_DialogueRunner.VariableStorage.TryGetValue<string>("$speaker", out string speaker) &&
                        string.IsNullOrEmpty(speaker) == false)
                    {
                        charID = speaker;
                    }
                }

                chatController.AddMessage(charID, lineText);
            }
            else
            {
                Debug.LogWarning($"ChatDialogueView: ChatController not found. Line: {lineText}");
            }

            onDialogueLineFinished?.Invoke();
        }

        /// <summary>
        /// 選択肢を表示し、選択が行われるまで待機する。
        /// </summary>
        /// <param name="dialogueOptions">選択肢一覧</param>
        /// <param name="onOptionSelected">選択時のコールバック (DialogueOptionID)</param>
        public override void RunOptions(DialogueOption[] dialogueOptions, System.Action<int> onOptionSelected)
        {
            ChatController chatController = FindFirstObjectByType<ChatController>();
            if (chatController != null)
            {
                List<string> choiceTexts = new List<string>();
                m_CurrentOptionIDs = new int[dialogueOptions.Length];
                for (int i = 0; i < dialogueOptions.Length; i++)
                {
                    choiceTexts.Add(dialogueOptions[i].Line.TextWithoutCharacterName.Text);
                    m_CurrentOptionIDs[i] = dialogueOptions[i].DialogueOptionID;
                }

                m_OnOptionSelected = onOptionSelected;
                m_IsWaitingForChoice = true;

                chatController.ShowChoices(choiceTexts, OnChoiceSelected);
                m_WaitCoroutine = StartCoroutine(WaitForChoice());
            }
            else
            {
                Debug.LogWarning("ChatDialogueView: ChatController not found. Selecting default option.");
                onOptionSelected?.Invoke(dialogueOptions.Length > 0 ? dialogueOptions[0].DialogueOptionID : 0);
            }
        }

        /// <summary>
        /// 選択肢が選択された時の処理。
        /// </summary>
        /// <param name="index">選択された選択肢インデックス</param>
        private void OnChoiceSelected(int index)
        {
            m_IsWaitingForChoice = false;

            if (m_WaitCoroutine != null)
            {
                StopCoroutine(m_WaitCoroutine);
                m_WaitCoroutine = null;
            }

            int selectedOptionID = index;
            if (m_CurrentOptionIDs != null && index >= 0 && index < m_CurrentOptionIDs.Length)
            {
                selectedOptionID = m_CurrentOptionIDs[index];
            }

            m_OnOptionSelected?.Invoke(selectedOptionID);
        }

        /// <summary>
        /// 選択肢選択を待機するコルーチン。
        /// </summary>
        private IEnumerator WaitForChoice()
        {
            while (m_IsWaitingForChoice)
            {
                yield return null;
            }
        }

        /// <summary>
        /// ダイアログ開始時の処理。
        /// </summary>
        public override void DialogueStarted()
        {
        }

        /// <summary>
        /// ダイアログ完了時の処理。
        /// </summary>
        public override void DialogueComplete()
        {
        }
    }
}
