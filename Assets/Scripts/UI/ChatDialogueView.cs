#if YARN_SPINNER
#nullable enable
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace ProjectFoundPhone.UI
{
    public class ChatDialogueView : DialoguePresenterBase
    {
        [SerializeField] private float m_LineDisplayDelay = 0.5f;
        [SerializeField] private bool m_ShowDebugOverlay = true;

        private DialogueRunner? m_DialogueRunner;
        private TextMeshProUGUI? m_DebugOverlayText;
        private GameObject? m_DebugOverlayObj;
        private bool m_DebugOverlayExpanded = false;
        private string m_CurrentNodeName = "-";
        private string m_CurrentLineId = "-";
        private string m_CurrentTags = "-";

        private void Awake()
        {
            m_DialogueRunner = GetComponent<DialogueRunner>();
            EnsureDebugOverlay();
            RefreshDebugOverlay();
        }

        public override async YarnTask RunLineAsync(LocalizedLine dialogueLine, LineCancellationToken token)
        {
            ChatController chatController = FindFirstObjectByType<ChatController>();
            string lineText = dialogueLine?.TextWithoutCharacterName.Text ?? string.Empty;

            UpdateDebugState(dialogueLine);

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

            await YarnTask.Delay((int)(m_LineDisplayDelay * 1000), token.NextContentToken).SuppressCancellationThrow();
        }

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
                        // 選択された内容をプレイヤーの発言としてチャットに追加
                        chatController.AddMessage("player", choiceTexts[index]);
                    }

                    choiceMade = true;
                });

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

            Debug.LogWarning("ChatDialogueView: ChatController not found. Selecting default option.");
            return dialogueOptions.Length > 0 ? dialogueOptions[0] : null;
        }

        public override YarnTask OnDialogueStartedAsync()
        {
            EnsureDebugOverlay();
            RefreshDebugOverlay();
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            m_CurrentLineId = "-";
            m_CurrentTags = "-";
            RefreshDebugOverlay();
            return YarnTask.CompletedTask;
        }

        public override void OnNodeEnter(string nodeName)
        {
            m_CurrentNodeName = string.IsNullOrEmpty(nodeName) ? "-" : nodeName;

            if (m_DialogueRunner != null && m_DialogueRunner.VariableStorage != null)
            {
                m_DialogueRunner.VariableStorage.SetValue("$current_node", nodeName);
            }

            RefreshDebugOverlay();
        }

        private void UpdateDebugState(LocalizedLine? dialogueLine)
        {
            m_CurrentLineId = dialogueLine == null || string.IsNullOrEmpty(dialogueLine.TextID)
                ? "-"
                : dialogueLine.TextID;

            if (dialogueLine == null || dialogueLine.Metadata == null || dialogueLine.Metadata.Length == 0)
            {
                m_CurrentTags = "-";
            }
            else
            {
                m_CurrentTags = string.Join(", ", dialogueLine.Metadata);
            }

            RefreshDebugOverlay();
        }

        private void EnsureDebugOverlay()
        {
            if (!m_ShowDebugOverlay || m_DebugOverlayText != null)
            {
                return;
            }

            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                return;
            }

            Transform existing = canvas.transform.Find("ContentAuthoringDebugOverlay");
            if (existing != null)
            {
                m_DebugOverlayObj = existing.gameObject;
                m_DebugOverlayText = existing.GetComponentInChildren<TextMeshProUGUI>();
                return;
            }

            // 右上に小さなバッジとして配置（クリックで展開/折りたたみ）
            GameObject overlayObj = new GameObject("ContentAuthoringDebugOverlay", typeof(RectTransform), typeof(Image), typeof(Button));
            overlayObj.transform.SetParent(canvas.transform, false);
            m_DebugOverlayObj = overlayObj;

            RectTransform overlayRect = overlayObj.GetComponent<RectTransform>();
            overlayRect.anchorMin = new Vector2(1f, 1f);
            overlayRect.anchorMax = new Vector2(1f, 1f);
            overlayRect.pivot = new Vector2(1f, 1f);
            overlayRect.anchoredPosition = new Vector2(-12f, -12f);
            overlayRect.sizeDelta = new Vector2(120f, 36f);

            Image overlayImage = overlayObj.GetComponent<Image>();
            overlayImage.color = new Color(0f, 0f, 0f, 0.45f);
            overlayImage.raycastTarget = true;

            Button overlayButton = overlayObj.GetComponent<Button>();
            overlayButton.transition = Selectable.Transition.None;
            overlayButton.onClick.AddListener(ToggleDebugOverlay);

            GameObject textObj = new GameObject("Label", typeof(RectTransform));
            textObj.transform.SetParent(overlayObj.transform, false);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 4f);
            textRect.offsetMax = new Vector2(-8f, -4f);

            m_DebugOverlayText = textObj.AddComponent<TextMeshProUGUI>();
            m_DebugOverlayText.fontSize = 16f;
            m_DebugOverlayText.color = new Color(1f, 1f, 1f, 0.8f);
            m_DebugOverlayText.alignment = TextAlignmentOptions.TopLeft;
            m_DebugOverlayText.textWrappingMode = TextWrappingModes.NoWrap;
            m_DebugOverlayText.overflowMode = TextOverflowModes.Ellipsis;
            m_DebugOverlayText.raycastTarget = false;

            m_DebugOverlayExpanded = false;
        }

        private void ToggleDebugOverlay()
        {
            m_DebugOverlayExpanded = !m_DebugOverlayExpanded;

            if (m_DebugOverlayObj == null) return;
            RectTransform rect = m_DebugOverlayObj.GetComponent<RectTransform>();
            if (rect == null) return;

            if (m_DebugOverlayExpanded)
            {
                rect.sizeDelta = new Vector2(400f, 80f);
                if (m_DebugOverlayText != null) m_DebugOverlayText.fontSize = 16f;
            }
            else
            {
                rect.sizeDelta = new Vector2(120f, 36f);
                if (m_DebugOverlayText != null) m_DebugOverlayText.fontSize = 16f;
            }

            RefreshDebugOverlay();
        }

        private void RefreshDebugOverlay()
        {
            if (!m_ShowDebugOverlay)
            {
                return;
            }

            EnsureDebugOverlay();
            if (m_DebugOverlayText == null)
            {
                return;
            }

            if (m_DebugOverlayExpanded)
            {
                m_DebugOverlayText.text =
                    $"node: {m_CurrentNodeName}\n" +
                    $"line: {m_CurrentLineId}\n" +
                    $"tag: {m_CurrentTags}";
            }
            else
            {
                m_DebugOverlayText.text = $"[D] {m_CurrentNodeName}";
            }
        }
    }
}
#endif
