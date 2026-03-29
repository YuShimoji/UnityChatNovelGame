#if YARN_SPINNER
#nullable enable
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Unity;
using ProjectFoundPhone.Core;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// Yarn Spinner の DialoguePresenterBase を拡張したチャット風ダイアログビュー。
    /// DialogueRunner の出力を ChatController と連携して表示する。
    /// </summary>
    public class ChatDialogueView : DialoguePresenterBase
    {
        [Header("Message Timing")]
        [Tooltip("NPC発話前のタイピングインジケーター表示時間（秒）")]
        [SerializeField] private float m_TypingIndicatorDuration = 0.8f;
        [Tooltip("メッセージ表示後の余韻時間（秒）。タイプライター完了後に追加される")]
        [SerializeField] private float m_PostMessageDelay = 0.4f;

        [Header("Tap Skip")]
        [Tooltip("画面タップでメッセージ送り/アニメスキップを有効にする")]
        [SerializeField] private bool m_EnableTapSkip = true;

        [Header("Debug")]
        [SerializeField] private bool m_ShowDebugOverlay = false;

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

        // タップスキップ用の状態管理
        private CancellationTokenSource? m_LineSkipCts;
        private bool m_IsShowingLine = false;

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

            // 画面タップでメッセージスキップ（VN風テキスト送り）
            if (m_EnableTapSkip && m_IsShowingLine && Input.GetMouseButtonDown(0))
            {
                // UI ボタン上のクリックは無視（選択肢ボタン等）
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    // ただしチャット領域（ScrollRect等）はスキップ対象
                    // ボタン以外のUI要素上のクリックはスキップとして扱う
                    var selected = EventSystem.current.currentSelectedGameObject;
                    if (selected != null && selected.GetComponent<Button>() != null)
                    {
                        return;
                    }
                }

                SkipCurrentLine();
            }
        }

        /// <summary>
        /// 現在表示中のメッセージのアニメーションをスキップし、即座に全文表示する。
        /// タイピングインジケーター・タイプライター・待ち時間をすべてキャンセルする。
        /// </summary>
        private void SkipCurrentLine()
        {
            // タイプライター効果を即完了
            m_ChatController?.CompleteCurrentTypewriter();

            // タイピングインジケーターを非表示
            m_ChatController?.ShowTypingIndicator(false);

            // 現在の遅延をキャンセル
            if (m_LineSkipCts != null && !m_LineSkipCts.IsCancellationRequested)
            {
                m_LineSkipCts.Cancel();
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
                m_IsShowingLine = true;

                // スキップ用トークンを初期化
                m_LineSkipCts?.Dispose();
                m_LineSkipCts = new CancellationTokenSource();

                // 話者解決: CharacterName → $speaker 変数 → "npc" フォールバック
                string charID = ResolveSpeaker(dialogueLine);

                // NPC発話の場合のみTypingIndicatorを表示（早送り時はスキップ）
                bool isPlayer = charID == "player";
                if (!isPlayer && !m_FastForwardEnabled)
                {
                    m_ChatController.ShowTypingIndicator(true);

                    // タイピングインジケーター表示時間（タップスキップ可能）
                    int indicatorMs = (int)(m_TypingIndicatorDuration * 1000);
                    await DelayWithSkip(indicatorMs, token.NextContentToken);

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
                    // タイプライター所要時間 + 余韻
                    float typewriterDuration = lineText.Length * 0.05f;
                    int waitMs = (int)((typewriterDuration + m_PostMessageDelay) * 1000);
                    await DelayWithSkip(waitMs, token.NextContentToken);
                }
                else
                {
                    await YarnTask.Delay(30, token.NextContentToken).SuppressCancellationThrow();
                }

                m_IsShowingLine = false;
            }
            else
            {
                Debug.LogWarning($"ChatDialogueView: ChatController not found. Line: {lineText}");
            }
        }

        /// <summary>
        /// YarnTask.Delay のラッパー。タップスキップとYarnのNextContentToken両方でキャンセル可能。
        /// </summary>
        private async YarnTask DelayWithSkip(int milliseconds, CancellationToken yarnToken)
        {
            if (m_LineSkipCts == null || m_LineSkipCts.IsCancellationRequested)
            {
                return;
            }

            using var linked = CancellationTokenSource.CreateLinkedTokenSource(yarnToken, m_LineSkipCts.Token);
            await YarnTask.Delay(milliseconds, linked.Token).SuppressCancellationThrow();
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
                // 全選択肢の条件が偽の場合のセーフティ: 無限待機を防止
                if (dialogueOptions.Length == 0)
                {
                    Debug.LogWarning("[ChatDialogueView] RunOptionsAsync received 0 available options. Ending dialogue to prevent infinite loop.");
                    return null;
                }

                // タイプライター効果が完全に終了するまで短い遅延を追加（食い気味防止）
                if (!m_FastForwardEnabled)
                {
                    await YarnTask.Delay(200, cancellationToken.NextContentToken).SuppressCancellationThrow();
                }

                List<string> choiceTexts = new List<string>();
                for (int i = 0; i < dialogueOptions.Length; i++)
                {
                    choiceTexts.Add(dialogueOptions[i].Line.TextWithoutCharacterName.Text);
                }

                DialogueOption selectedOption = null;
                bool choiceMade = false;

                m_ChatController.ShowChoices(choiceTexts, (index) =>
                {
                    // 二重実行防止：既に選択済みの場合は無視
                    if (choiceMade) return;
                    choiceMade = true;

                    if (index >= 0 && index < dialogueOptions.Length)
                    {
                        selectedOption = dialogueOptions[index];
                        // プレイヤーの選択をメッセージとして自動表示（Yarn側でのエコー不要）
                        Debug.Log($"[RunOptionsAsync] Player selected index={index}, text='{choiceTexts[index]}'");
                        m_ChatController.AddMessage("player", choiceTexts[index]);

                        // AutoSpeakerAfterChoice: 選択後に $speaker を "player" に自動設定
                        if (m_DialogueRunner?.VariableStorage != null &&
                            m_DialogueRunner.VariableStorage.TryGetValue<bool>("$auto_speaker_after_choice", out bool autoSpeaker) &&
                            autoSpeaker)
                        {
                            m_DialogueRunner.VariableStorage.SetValue("$speaker", "player");
                        }
                    }
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

                // 選択肢選択後にオートセーブをトリガー
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.AutoSave();
                }

                return selectedOption;
            }

            Debug.LogError("[ChatDialogueView] ChatController not found in scene. Cannot display choices. Returning first option as fallback.");
            return dialogueOptions.Length > 0 ? dialogueOptions[0] : null;
        }

        /// <summary>
        /// ダイアログ開始時の処理。入力をロックし選択肢をクリアする。
        /// </summary>
        public override YarnTask OnDialogueStartedAsync()
        {
            // チャプター遷移時の早送り状態引き継ぎを防止
            FastForwardEnabled = false;

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
            m_IsShowingLine = false;
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

            // ノード遷移時にオートセーブをトリガー
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.AutoSave();
            }
        }

        private void OnDestroy()
        {
            m_LineSkipCts?.Dispose();
            m_LineSkipCts = null;
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
