#if YARN_SPINNER
#nullable enable
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// Yarn Spinner の DialoguePresenterBase を拡張したチャット風ダイアログビュー。
    /// DialogueRunner の出力を ChatController と連携して表示する。
    /// </summary>
    public class ChatDialogueView : DialoguePresenterBase
    {
        [SerializeField] private float m_LineDisplayDelay = 0.5f;
        [SerializeField] private bool m_ShowDebugOverlay = true;

        private DialogueRunner? m_DialogueRunner;
        private ChatController? m_ChatController;
        private TextMeshProUGUI? m_DebugOverlayText;
        private GameObject? m_DebugOverlayObj;
        private bool m_DebugOverlayExpanded = false;
        private string m_CurrentNodeName = "-";
        private string m_CurrentLineId = "-";
        private string m_CurrentTags = "-";

        private bool m_FastForwardEnabled = false;
        /// <summary>早送りモード（F11 トグル）。有効時はタイピング遅延とタイプライター待ちをスキップする。</summary>
        public bool FastForwardEnabled { get => m_FastForwardEnabled; set { m_FastForwardEnabled = value; RefreshDebugOverlay(); } }

        private void Awake()
        {
            m_DialogueRunner = GetComponent<DialogueRunner>();
            EnsureDebugOverlay();
            RefreshDebugOverlay();
        }

        private void Start()
        {
            // キャッシュして毎回のFindを回避
            m_ChatController = FindFirstObjectByType<ChatController>();
        }

        private void Update()
        {
            // F11 で早送りトグル（F12 は DebugHub が使用）
            if (Input.GetKeyDown(KeyCode.F11))
            {
                FastForwardEnabled = !FastForwardEnabled;
            }
        }

        public override async YarnTask RunLineAsync(LocalizedLine dialogueLine, LineCancellationToken token)
        {
            if (m_ChatController == null)
            {
                m_ChatController = FindFirstObjectByType<ChatController>();
            }

            string lineText = dialogueLine?.TextWithoutCharacterName.Text ?? string.Empty;

            UpdateDebugState(dialogueLine);

            if (m_ChatController != null)
            {
                // 話者解決: CharacterName → $speaker 変数 → "npc" フォールバック
                string charID = ResolveSpeaker(dialogueLine);

                // NPC発話の場合のみTypingIndicatorを表示（早送り時はスキップ）
                bool isPlayer = charID == "player";
                if (!isPlayer && !m_FastForwardEnabled)
                {
                    m_ChatController.ShowTypingIndicator(true);
                    await YarnTask.Delay((int)(m_LineDisplayDelay * 0.6f * 1000), token.NextContentToken).SuppressCancellationThrow();
                    m_ChatController.ShowTypingIndicator(false);
                }

                // TextID は Yarn の #line: タグに対応 — 矛盾指摘システムの識別子として使用
                string lineTag = dialogueLine != null && !string.IsNullOrEmpty(dialogueLine.TextID)
                    ? dialogueLine.TextID
                    : null;
                m_ChatController.AddMessage(charID, lineText, lineTag);

                // タイプライター効果の完了を待つ（早送り時は最小遅延のみ）
                if (!m_FastForwardEnabled)
                {
                    float typewriterDuration = lineText.Length * 0.05f;
                    await YarnTask.Delay((int)((typewriterDuration + 0.3f) * 1000), token.NextContentToken).SuppressCancellationThrow();
                }
                else
                {
                    await YarnTask.Delay(30, token.NextContentToken).SuppressCancellationThrow();
                }
            }
            else
            {
                Debug.LogWarning($"ChatDialogueView: ChatController not found. Line: {lineText}");
            }
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
                        // プレイヤーの選択をメッセージとして自動表示（Yarn側でのエコー不要）
                        m_ChatController.AddMessage("player", choiceTexts[index]);
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

            Debug.LogWarning("ChatDialogueView: ChatController not found. Selecting default option.");
            return dialogueOptions.Length > 0 ? dialogueOptions[0] : null;
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

            EnsureDebugOverlay();
            RefreshDebugOverlay();
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

            string ffTag = m_FastForwardEnabled ? " [FF]" : "";
            if (m_DebugOverlayExpanded)
            {
                m_DebugOverlayText.text =
                    $"node: {m_CurrentNodeName}\n" +
                    $"line: {m_CurrentLineId}\n" +
                    $"tag: {m_CurrentTags}{ffTag}";
            }
            else
            {
                m_DebugOverlayText.text = $"[D] {m_CurrentNodeName}{ffTag}";
            }
        }
    }
}
#endif
