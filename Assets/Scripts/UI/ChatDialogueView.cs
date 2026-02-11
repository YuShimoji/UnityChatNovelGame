#if YARN_SPINNER
using Yarn.Unity;
using UnityEngine;
using System.Collections.Generic;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// Yarn Spinner 縺ｮ DialoguePresenterBase 繧堤ｶ呎価縺励◆繝√Ε繝・ヨ蟆ら畑繝繧､繧｢繝ｭ繧ｰ繝薙Η繝ｼ縲・
    /// DialogueRunner 縺ｮ蜃ｺ蜉帙ｒ ChatController 縺ｨ騾｣謳ｺ縺励※陦ｨ遉ｺ縺吶ｋ縲・
    /// </summary>
    public class ChatDialogueView : DialoguePresenterBase
    {
        [SerializeField] private float m_LineDisplayDelay = 0.5f;

        private DialogueRunner m_DialogueRunner;

        private void Awake()
        {
            m_DialogueRunner = GetComponent<DialogueRunner>();
        }

        public override async YarnTask RunLineAsync(LocalizedLine dialogueLine, LineCancellationToken token)
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

            // 繝√Ε繝・ヨ繧峨＠縺・ユ繝ｳ繝昴・縺溘ａ縺ｮ驕・ｻｶ
            await YarnTask.Delay((int)(m_LineDisplayDelay * 1000), token.NextContentToken).SuppressCancellationThrow();
        }

        /// <summary>
        /// 驕ｸ謚櫁い繧定｡ｨ遉ｺ縺励・∈謚槭′陦後ｏ繧後ｋ縺ｾ縺ｧ蠕・ｩ溘☆繧九・
        /// </summary>
        public override async YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] dialogueOptions, LineCancellationToken cancellationToken)
        {
            ChatController chatController = FindFirstObjectByType<ChatController>();
            if (chatController != null)
            {
                List<string> choiceTexts = new List<string>();
                for (int i = 0; i < dialogueOptions.Length; i++)
                {
                    choiceTexts.Add(dialogueOptions[i].Line.TextWithoutCharacterName.Text);
                }

                DialogueOption selectedOption = null;
                bool choiceMade = false;

                chatController.ShowChoices(choiceTexts, (index) =>
                {
                    if (index >= 0 && index < dialogueOptions.Length)
                    {
                        selectedOption = dialogueOptions[index];
                    }
                    choiceMade = true;
                });

                // 驕ｸ謚槫ｾ・■
                while (!choiceMade)
                {
                    if (cancellationToken.IsNextContentRequested)
                    {
                        chatController.HideChoices();
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
        /// 繝繧､繧｢繝ｭ繧ｰ髢句ｧ区凾縺ｮ蜃ｦ逅・・
        /// </summary>
        public override YarnTask OnDialogueStartedAsync()
        {
            return YarnTask.CompletedTask;
        }

        /// <summary>
        /// 繝繧､繧｢繝ｭ繧ｰ螳御ｺ・凾縺ｮ蜃ｦ逅・・
        /// </summary>
        public override YarnTask OnDialogueCompleteAsync()
        {
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



