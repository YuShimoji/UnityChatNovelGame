using Yarn.Unity;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// Yarn SpinnerのDialogueViewBaseを継承したチャット専用ダイアログビュー
    /// DialogueRunnerの進行をChatControllerと連携して制御する
    /// </summary>
    public class ChatDialogueView : DialogueViewBase
    {
        private Coroutine m_WaitCoroutine;
        private bool m_IsWaitingForChoice = false;
        private System.Action<int> m_OnOptionSelected;

        /// <summary>
        /// テキスト行を表示
        /// </summary>
        /// <param name="line">表示する行データ</param>
        /// <param name="onDialogueLineFinished">表示完了時のコールバック</param>
        public override void RunLine(Yarn.Line line, System.Action onDialogueLineFinished)
        {
            // ChatControllerを使ってメッセージを表示
            ChatController chatController = FindFirstObjectByType<ChatController>();
            if (chatController != null)
            {
                // キャラクターIDはYarn変数$speakerから取得、未設定時はデフォルトNPC
                string charID = "npc"; // デフォルトキャラクターID
                if (dialogueRunner != null && dialogueRunner.VariableStorage != null)
                {
                    if (dialogueRunner.VariableStorage.TryGetValue("$speaker", out string speaker))
                    {
                        charID = speaker;
                    }
                }
                chatController.AddMessage(charID, line.Text);
            }
            else
            {
                Debug.LogWarning($"ChatDialogueView: ChatController not found. Line: {line.Text}");
            }
            onDialogueLineFinished();
        }

        /// <summary>
        /// 選択肢を表示し、選択まで待機
        /// </summary>
        /// <param name="options">選択肢セット</param>
        /// <param name="onOptionSelected">選択時のコールバック</param>
        public override void RunOptions(Yarn.OptionSet options, System.Action<int> onOptionSelected)
        {
            ChatController chatController = FindFirstObjectByType<ChatController>();
            if (chatController != null)
            {
                List<string> choiceTexts = new List<string>();
                foreach (var option in options.Options)
                {
                    choiceTexts.Add(option.Line.Text);
                }

                m_OnOptionSelected = onOptionSelected;
                m_IsWaitingForChoice = true;

                chatController.ShowChoices(choiceTexts, OnChoiceSelected);

                // 選択まで待機するコルーチン開始
                m_WaitCoroutine = StartCoroutine(WaitForChoice());
            }
            else
            {
                Debug.LogWarning("ChatDialogueView: ChatController not found. Selecting default option.");
                onOptionSelected(0); // デフォルトで最初の選択肢を選択
            }
        }

        /// <summary>
        /// 選択肢が選択された時の処理
        /// </summary>
        /// <param name="index">選択された選択肢のインデックス</param>
        private void OnChoiceSelected(int index)
        {
            m_IsWaitingForChoice = false;
            if (m_WaitCoroutine != null)
            {
                StopCoroutine(m_WaitCoroutine);
                m_WaitCoroutine = null;
            }
            m_OnOptionSelected?.Invoke(index);
        }

        /// <summary>
        /// 選択肢選択を待機するコルーチン
        /// </summary>
        private System.Collections.IEnumerator WaitForChoice()
        {
            while (m_IsWaitingForChoice)
            {
                yield return null;
            }
        }

        /// <summary>
        /// ダイアログ開始時の処理
        /// </summary>
        public override void DialogueStarted()
        {
            // 必要に応じて初期化処理
        }

        /// <summary>
        /// ダイアログ完了時の処理
        /// </summary>
        public override void DialogueComplete()
        {
            // 必要に応じてクリーンアップ処理
        }
    }
}
