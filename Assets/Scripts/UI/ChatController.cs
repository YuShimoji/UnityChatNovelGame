using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using System.Collections.Generic;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;
using Unity.Profiling;
using System.Linq;
using UnityEngine.EventSystems;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// チャット画面のUI制御を行うコントローラー
    /// ScrollRect + VerticalLayoutGroup + ContentSizeFitterを使用したメッセージ表示システム
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ChatController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        #region Private Fields
        [SerializeField] private ScrollRect m_ScrollRect;
        [SerializeField] private VerticalLayoutGroup m_LayoutGroup;
        [SerializeField] private GameObject m_MessageBubblePrefab;
        [SerializeField] private GameObject m_TypingIndicator;
        [SerializeField] private TMP_InputField m_InputField;
        [SerializeField] private Button m_SendButton;
        [SerializeField] private float m_AutoScrollThreshold = 0.1f; // 自動スクロールを実行する閾値（0.0-1.0）
        [SerializeField] private MessageBubblePool m_MessageBubblePool;

        [Header("Image Bubble Settings")]
        [SerializeField] private GameObject m_ImageBubblePrefab;
        [SerializeField] private float m_ImageMaxWidth = 300f;
        [SerializeField] private float m_ImageMaxHeight = 200f;

        [Header("Choice Settings")]
        [SerializeField] private GameObject m_ChoiceButtonPrefab;
        [SerializeField] private Transform m_ChoiceContainer;

        [Header("Font Settings")]
        [SerializeField] private TMP_FontAsset m_JapaneseFontAsset;

        [Header("Typewriter Effect Settings")]
        [SerializeField] private bool m_EnableTypewriterEffect = true;
        [SerializeField] private float m_TypewriterSpeed = 0.05f; // 1文字あたりの表示時間（秒）

        private bool m_IsUserScrolling = false;
        private bool m_IsUserDragging = false;
        private float m_LastScrollPosition = 1.0f;

        private bool m_AutoScrollScheduled = false;
        private bool m_IsAutoScrolling = false;
        private bool m_PinnedToBottom = true;
        private bool m_IsTypewriterActive = false;
        private TextMeshProUGUI m_LastTypewriterTarget;
        /// <summary>タイプライター完了時に発火するイベント。ChatDialogueView が待機に使用する。</summary>
        public event Action OnTypewriterCompleted;
        private GameObject m_TopSpacer;

        private GameObject m_RuntimeChoiceButtonTemplate;
        private GameObject m_RuntimeImageBubbleTemplate;
        private GameObject m_RuntimeMessageBubbleTemplate;
        private TMP_InputField m_RuntimeInputField;
        private Button m_RuntimeSendButton;
        private Button m_InventoryShortcutButton;
        // m_TypingIndicatorWrapper は廃止: ConfigureBubble がラッパーを生成するため
        // m_TypingIndicator.transform.parent で動的に取得する
        private static readonly ProfilerMarker s_CreateMessageBubbleMarker = new ProfilerMarker("ChatController.CreateMessageBubble");
        private static readonly ProfilerMarker s_AddMessageMarker = new ProfilerMarker("ChatController.AddMessage");
        private static readonly ProfilerMarker s_AddImageMessageMarker = new ProfilerMarker("ChatController.AddImageMessage");
        private static readonly ProfilerMarker s_AddSystemMessageMarker = new ProfilerMarker("ChatController.AddSystemMessage");
        private static readonly ProfilerMarker s_ShowChoicesMarker = new ProfilerMarker("ChatController.ShowChoices");
        private static readonly ProfilerMarker s_AutoScrollMarker = new ProfilerMarker("ChatController.AutoScroll");

        private readonly List<SavedChatMessage> m_ChatHistory = new List<SavedChatMessage>();
        private bool m_IsRestoringHistory = false;

        // スレッド切替時のフェードイン + スムーズスクロール用
        private CanvasGroup m_ContentCanvasGroup;
        private Tween m_ScrollTween;
        private Coroutine m_FadeInCoroutine;
        private const float ThreadSwitchFadeDuration = 0.15f;
        private const float SmoothScrollDuration = 0.2f;

        /// <summary>現在表示中のスレッドID (null = メインスレッド)</summary>
        private string m_ActiveThreadId;

        /// <summary>現在表示中のスレッドの種別 (null = メインスレッド)</summary>
        private ThreadType? m_ActiveThreadType;

        /// <summary>スレッド別の会話履歴 (メインスレッドは null キーで管理)</summary>
        private readonly Dictionary<string, List<SavedChatMessage>> m_ThreadHistories
            = new Dictionary<string, List<SavedChatMessage>>();

        /// <summary>スレッド別のスクロール位置</summary>
        private readonly Dictionary<string, float> m_ThreadScrollPositions
            = new Dictionary<string, float>();

        /// <summary>前回のメッセージの話者（連続メッセージ判定用）</summary>
        private string m_LastSpeaker = null;

        /// <summary>タイピングインジケーターのドットアニメーション Tween 群</summary>
        private readonly List<Tween> m_TypingDotTweens = new List<Tween>();

        /// <summary>ChatUIConfig SO へのキャッシュ付きアクセス</summary>
        private ChatUIConfig m_UIConfig;
        private ChatUIConfig UIConfig => m_UIConfig ??= ChatUIConfig.Instance;

        /// <summary>ScrollRect の RectTransform キャッシュ（バブル幅計算用）</summary>
        private RectTransform m_ScrollRectTransform;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            if (m_SendButton != null)
            {
                m_SendButton.onClick.AddListener(OnSubmit);
            }

            if (m_InputField != null)
            {
                m_InputField.onSubmit.AddListener((text) => OnSubmit());
            }

            // スクロール位置の変更をイベントで監視（毎フレームUpdate不要）
            if (m_ScrollRect != null)
            {
                m_ScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            }
        }

        private void LateUpdate()
        {
            // タイプライター中のコンテンツ成長に追従するため、
            // タイプライター効果がアクティブな間のみ毎フレーム最下部に固定
            if (m_PinnedToBottom && !m_IsUserScrolling && m_ScrollRect != null && m_IsTypewriterActive)
            {
                // OnScrollValueChanged が誤検知しないようガード
                m_IsAutoScrolling = true;
                m_ScrollRect.verticalNormalizedPosition = 0f;
                m_IsAutoScrolling = false;
            }
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            m_IsUserDragging = true;
            m_IsUserScrolling = true;
            m_PinnedToBottom = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_IsUserDragging = false;
            // ドラッグ終了後、最下部付近なら自動吸着を再開
            if (m_ScrollRect != null && m_ScrollRect.verticalNormalizedPosition < m_AutoScrollThreshold)
            {
                m_IsUserScrolling = false;
                m_PinnedToBottom = true;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// ScrollRect の Canvas スケーリング後の幅を返す。
        /// Screen.width の代わりに使用し、リサイズに追従させる。
        /// </summary>
        private float GetContainerWidth()
        {
            if (m_ScrollRectTransform != null)
            {
                return m_ScrollRectTransform.rect.width;
            }
            // フォールバック: キャッシュ未初期化時は Screen.width を使用
            return Screen.width;
        }

        // ---------- Thread Markup ----------

        /// <summary>
        /// TMP テキストにリンクのクリックハンドラを設定する。
        /// "thread:threadId" リンクをクリックするとスレッドに切り替わる。
        /// </summary>
        private void SetupLinkClickHandler(TextMeshProUGUI textComponent)
        {
            // EventTrigger でポインタークリックを検出
            var trigger = textComponent.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = textComponent.gameObject.AddComponent<EventTrigger>();
            }
            textComponent.raycastTarget = true;

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entry.callback.AddListener((data) =>
            {
                var pointerData = (PointerEventData)data;
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, pointerData.position, null);
                if (linkIndex >= 0)
                {
                    string linkId = textComponent.textInfo.linkInfo[linkIndex].GetLinkID();
                    if (linkId.StartsWith("thread:"))
                    {
                        string threadId = linkId.Substring("thread:".Length);
                        Debug.Log($"ChatController: Thread link clicked — '{threadId}'");

                        // ThreadSwitcherController 経由でスレッド切替
                        var threadSwitcher = FindFirstObjectByType<ThreadSwitcherController>();
                        if (threadSwitcher != null)
                        {
                            threadSwitcher.ForceSelectThread(threadId);
                        }
                    }
                }
            });
            trigger.triggers.Add(entry);
        }

        /// <summary>
        /// canvas 幅に応じたバブル最大幅パーセントを返す。
        /// 狭い画面ではバブルを広くして折り返しを減らす。
        /// </summary>
        private float GetResponsiveBubblePercent()
        {
            float w = GetContainerWidth();
            float basePercent = UIConfig.bubbleMaxWidthPercent;
            if (w < 800f)
                return Mathf.Max(basePercent, 0.85f);
            if (w < 1000f)
                return Mathf.Max(basePercent, Mathf.Lerp(0.85f, basePercent, (w - 800f) / 200f));
            return basePercent;
        }

        /// <summary>
        /// canvas 幅に応じたフォントサイズスケールを返す。
        /// 狭い画面ではフォントを縮小して情報密度を調整する。
        /// </summary>
        private float GetResponsiveFontScale()
        {
            return UIFontConfig.Instance.GetResponsiveScale(GetContainerWidth());
        }

        /// <summary>
        /// 必要なコンポーネントの初期化
        /// </summary>
        private void InitializeComponents()
        {
            if (m_ScrollRect == null)
            {
                m_ScrollRect = GetComponent<ScrollRect>();
            }

            // ScrollRect の RectTransform をキャッシュ（バブル幅計算用）
            if (m_ScrollRect != null)
            {
                m_ScrollRectTransform = m_ScrollRect.GetComponent<RectTransform>();
            }

            if (m_LayoutGroup == null && m_ScrollRect != null && m_ScrollRect.content != null)
            {
                m_LayoutGroup = m_ScrollRect.content.GetComponent<VerticalLayoutGroup>();
                
                // VerticalLayoutGroupのスペーシング設定を確保
                if (m_LayoutGroup != null)
                {
                    m_LayoutGroup.spacing = Mathf.Max(m_LayoutGroup.spacing, UIConfig.minLayoutSpacing);
                    m_LayoutGroup.childControlHeight = false;
                    m_LayoutGroup.childForceExpandHeight = false;
                }
            }

            // メッセージが少ない時に下詰めにするフレキシブルスペーサー
            if (m_ScrollRect != null && m_ScrollRect.content != null && m_TopSpacer == null)
            {
                m_TopSpacer = new GameObject("TopSpacer", typeof(RectTransform), typeof(LayoutElement));
                m_TopSpacer.transform.SetParent(m_ScrollRect.content, false);
                m_TopSpacer.transform.SetAsFirstSibling();
                LayoutElement spacerLayout = m_TopSpacer.GetComponent<LayoutElement>();
                spacerLayout.flexibleHeight = 1f;
                spacerLayout.minHeight = 0f;
            }

            // m_MessageBubblePrefab、m_TypingIndicatorのnullチェックと警告
            if (m_MessageBubblePrefab == null)
            {
                Debug.LogWarning("ChatController: m_MessageBubblePrefab is not assigned. Runtime message template will be used.");
            }

            if (m_TypingIndicator == null)
            {
                Debug.LogWarning("ChatController: m_TypingIndicator is not assigned. Typing indicator will not be displayed.");
            }

            // プールの初期化（未設定時は自動生成）
            EnsureMessageBubblePool();

            EnsureChoiceUIElements();
            EnsureImageBubbleTemplate();
            EnsureInputControls();
            EnsureInventoryShortcutButton();

            // 入力欄の表示/非表示制御
            if (!UIConfig.showInputField)
            {
                if (m_InputField != null) m_InputField.gameObject.SetActive(false);
                if (m_SendButton != null) m_SendButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// MessageBubblePool の初期化（未設定時は自動生成）
        /// </summary>
        private void EnsureMessageBubblePool()
        {
            if (m_MessageBubblePool == null)
            {
                m_MessageBubblePool = GetComponent<MessageBubblePool>();
                if (m_MessageBubblePool == null)
                {
                    m_MessageBubblePool = gameObject.AddComponent<MessageBubblePool>();
                }
            }

            // プールにPrefabを設定（Inspector 未設定時はランタイムテンプレートで代替）
            if (m_MessageBubblePool != null)
            {
                GameObject prefab = m_MessageBubblePrefab != null
                    ? m_MessageBubblePrefab
                    : GetSafeMessageBubbleTemplate();
                m_MessageBubblePool.SetPrefab(prefab);
            }
        }

        /// <summary>
        /// ScrollRect.onValueChanged イベントハンドラ
        /// ユーザーが手動でスクロールしているかを検知する
        /// </summary>
        private void OnScrollValueChanged(Vector2 scrollPosition)
        {
            // AutoScroll 中の位置変化はユーザー操作とみなさない
            if (m_IsAutoScrolling) return;

            // ドラッグ中は OnScrollValueChanged で状態を変更しない
            // （OnBeginDrag/OnEndDrag で管理する）
            if (m_IsUserDragging) return;

            // verticalNormalizedPosition: 0=最下部（最新メッセージ）, 1=最上部（最古メッセージ）
            float verticalPos = scrollPosition.y;

            // 最下部から一定以上離れている場合、ユーザーが過去ログを見ていると判定
            if (verticalPos > m_AutoScrollThreshold)
            {
                m_IsUserScrolling = true;
                m_PinnedToBottom = false;
            }
            else
            {
                // 最下部付近: コンテンツ追加後のレイアウト変化で一時的に離れても
                // 最下部に戻ったらユーザースクロール状態を解除する
                m_IsUserScrolling = false;
            }

            m_LastScrollPosition = verticalPos;
        }

        /// <summary>
        /// テーマカラーとバブルスプライトのみを適用する（ラッパー生成なし）。
        /// ConfigureBubble の前にテキスト色を確定させたい場合に使用。
        /// </summary>
        private void ApplyThemeColor(GameObject bubble, string charID)
        {
            bool isPlayer = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.IsPlayer(charID)
                : charID == "player";

            Color themeColor = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.GetThemeColor(charID)
                : (isPlayer ? new Color(0.2f, 0.6f, 1.0f) : new Color(0.3f, 0.3f, 0.35f));

            bool isAnnotationCard = m_ActiveThreadType == ThreadType.Annotation;

            Image bubbleBackground = bubble.GetComponent<Image>();
            if (bubbleBackground != null)
            {
                if (isAnnotationCard)
                {
                    Color typeColor = GetThreadTypeColor(ThreadType.Annotation);
                    bubbleBackground.color = Color.Lerp(new Color(0.15f, 0.15f, 0.2f, 0.85f), typeColor, 0.15f);
                }
                else if (m_ActiveThreadType.HasValue)
                {
                    Color typeColor = GetThreadTypeColor(m_ActiveThreadType.Value);
                    bubbleBackground.color = Color.Lerp(themeColor, typeColor, 0.12f);
                }
                else
                {
                    bubbleBackground.color = themeColor;
                }
            }

            // 角丸スプライト + 影を適用
            ApplyBubbleVisuals(bubble);

            TextMeshProUGUI textComponent = bubble.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                if (isAnnotationCard)
                {
                    Color typeColor = GetThreadTypeColor(ThreadType.Annotation);
                    textComponent.color = new Color(
                        Mathf.Min(typeColor.r + 0.3f, 1f),
                        Mathf.Min(typeColor.g + 0.3f, 1f),
                        Mathf.Min(typeColor.b + 0.3f, 1f),
                        1f);
                }
                else
                {
                    textComponent.color = isPlayer ? UIConfig.playerTextColor : UIConfig.npcTextColor;
                }
            }
        }

        /// <summary>
        /// バブルGameObjectにキャラクターベースのレイアウトとテーマカラーを適用する
        /// </summary>
        /// <param name="bubble">設定対象のバブルGameObject</param>
        /// <param name="charID">キャラクターID</param>
        /// <param name="isConsecutive">前回と同じ話者の連続メッセージか</param>
        private void ConfigureBubble(GameObject bubble, string charID, bool isConsecutive = false)
        {
            bool isPlayer = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.IsPlayer(charID)
                : charID == "player";

            Color themeColor = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.GetThemeColor(charID)
                : (isPlayer ? new Color(0.2f, 0.6f, 1.0f) : new Color(0.3f, 0.3f, 0.35f));

            bool isAnnotationCard = m_ActiveThreadType == ThreadType.Annotation;

            // バブル背景色を決定
            Image bubbleBackground = bubble.GetComponent<Image>();
            if (bubbleBackground != null)
            {
                if (isAnnotationCard)
                {
                    // A型(注釈): 型色ベースのカード背景
                    Color typeColor = GetThreadTypeColor(ThreadType.Annotation);
                    bubbleBackground.color = Color.Lerp(new Color(0.15f, 0.15f, 0.2f, 0.85f), typeColor, 0.15f);
                }
                else if (m_ActiveThreadType.HasValue)
                {
                    // B/C/分岐: 型色ティント（キャラ色に12%混合）
                    Color typeColor = GetThreadTypeColor(m_ActiveThreadType.Value);
                    bubbleBackground.color = Color.Lerp(themeColor, typeColor, 0.12f);
                }
                else
                {
                    // メインスレッド: 通常
                    bubbleBackground.color = themeColor;
                }
            }

            // 角丸スプライト + 影を適用
            ApplyBubbleVisuals(bubble);

            // MessageBubble の m_OriginalColor を同期（プール再利用時の色汚染防止）
            MessageBubble mb = bubble.GetComponent<MessageBubble>();
            if (mb != null)
            {
                mb.SyncOriginalColor();
            }

            // バブルの LayoutElement: CreateMessageBubble で設定済みの preferredWidth を維持
            // flexibleWidth = 0 でテキスト幅フィットを実現

            if (m_ScrollRect == null || m_ScrollRect.content == null) return;

            // ラッパーで配置を実現
            string wrapperName = isAnnotationCard ? "AnnotationRow" : (isPlayer ? "PlayerRow" : "NpcRow");
            GameObject wrapper = new GameObject(wrapperName,
                typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement), typeof(ContentSizeFitter));
            wrapper.transform.SetParent(m_ScrollRect.content, false);
            wrapper.transform.SetSiblingIndex(bubble.transform.GetSiblingIndex());

            HorizontalLayoutGroup hlg = wrapper.GetComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandWidth = false; // テキスト幅にフィット (バブルの preferredWidth を尊重)
            hlg.childForceExpandHeight = false;
            hlg.childControlWidth = true;      // HLG が子要素の幅を制御
            hlg.childControlHeight = true;
            hlg.spacing = UIConfig.iconBubbleSpacing; // アイコンとバブルの間隔

            int edgePad = UIConfig.wrapperEdgePadding;
            float containerWidth = GetContainerWidth();
            float maxWidthFromPercent = containerWidth * GetResponsiveBubblePercent();
            float maxWidth = Mathf.Min(maxWidthFromPercent, UIConfig.bubbleMaxWidthPx);
            int sideMargin = Mathf.Max(0, (int)(containerWidth - maxWidth - edgePad));
            int vPad = UIConfig.wrapperVerticalPadding;
            int topPad = isConsecutive ? 2 : vPad;
            int bottomPad = vPad;

            if (isAnnotationCard)
            {
                // A型(注釈): 中央配置、対称マージン
                int symPad = edgePad * 2;
                hlg.padding = new RectOffset(symPad, symPad, topPad, bottomPad);
                hlg.childAlignment = TextAnchor.MiddleCenter;
            }
            else
            {
                // 通常: パディングで片側にマージン → 左 or 右寄せ
                hlg.padding = isPlayer
                    ? new RectOffset(sideMargin, edgePad, topPad, bottomPad)
                    : new RectOffset(edgePad, sideMargin, topPad, bottomPad);
            }

            LayoutElement wrapperLayout = wrapper.GetComponent<LayoutElement>();
            wrapperLayout.flexibleWidth = 1f;

            ContentSizeFitter wrapperFitter = wrapper.GetComponent<ContentSizeFitter>();
            wrapperFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            wrapperFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // キャラクターアイコンを追加（A型注釈カードでは省略）
            if (!isAnnotationCard && UIConfig.showCharacterIcon && !string.IsNullOrEmpty(charID) && !isConsecutive)
            {
                GameObject iconObj = CreateCharacterIcon(charID);
                if (iconObj != null)
                {
                    iconObj.transform.SetParent(wrapper.transform, false);
                    // NPC: アイコン→バブル、Player: バブル→アイコン
                    if (isPlayer)
                    {
                        iconObj.transform.SetAsLastSibling();
                    }
                    else
                    {
                        iconObj.transform.SetAsFirstSibling();
                    }
                }
            }

            bubble.transform.SetParent(wrapper.transform, false);

            // アイコン表示 (A型注釈カードでは省略、NPC + DisplayMode がアイコン含む場合のみ)
            if (!isAnnotationCard && !isPlayer)
            {
                var profile = CharacterDatabase.Instance?.GetProfile(charID);
                if (profile != null && profile.Icon != null &&
                    profile.DisplayMode != CharacterDisplayMode.NameOnly)
                {
                    hlg.childForceExpandWidth = false;

                    GameObject iconObj = new GameObject("CharIcon",
                        typeof(RectTransform), typeof(Image), typeof(LayoutElement));
                    iconObj.transform.SetParent(wrapper.transform, false);
                    iconObj.transform.SetAsFirstSibling();

                    Image iconImage = iconObj.GetComponent<Image>();
                    iconImage.sprite = profile.Icon;
                    iconImage.preserveAspect = true;
                    iconImage.raycastTarget = false;

                    LayoutElement iconLayout = iconObj.GetComponent<LayoutElement>();
                    iconLayout.preferredWidth = 36f;
                    iconLayout.preferredHeight = 36f;
                    iconLayout.minWidth = 36f;
                    iconLayout.flexibleWidth = 0f;
                }
            }

            // テキストの色を調整
            TextMeshProUGUI textComponent = bubble.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                if (isAnnotationCard)
                {
                    // 注釈カード: 型色で統一
                    Color typeColor = GetThreadTypeColor(ThreadType.Annotation);
                    textComponent.color = new Color(
                        Mathf.Min(typeColor.r + 0.3f, 1f),
                        Mathf.Min(typeColor.g + 0.3f, 1f),
                        Mathf.Min(typeColor.b + 0.3f, 1f),
                        1f);
                }
                else
                {
                    textComponent.color = isPlayer ? UIConfig.playerTextColor : UIConfig.npcTextColor;
                }
            }
        }

        /// <summary>
        /// キャラクターアイコンを作成する（Discord風の円形アイコン）
        /// </summary>
        /// <param name="charID">キャラクターID</param>
        /// <returns>生成されたアイコンGameObject（取得失敗時null）</returns>
        private GameObject CreateCharacterIcon(string charID)
        {
            if (CharacterDatabase.Instance == null)
            {
                return null;
            }

            Sprite iconSprite = CharacterDatabase.Instance.GetIcon(charID);
            if (iconSprite == null)
            {
                // アイコンが設定されていない場合はスキップ
                return null;
            }

            float iconSize = UIConfig.characterIconSize;

            // コンテナ（円形マスク用）
            GameObject container = new GameObject("CharacterIconContainer",
                typeof(RectTransform), typeof(Image), typeof(Mask), typeof(LayoutElement));

            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(iconSize, iconSize);

            // 円形の背景Image（マスク用）
            Image maskImage = container.GetComponent<Image>();
            maskImage.sprite = BubbleSpriteFactory.CreateCircleSprite(); // 実行時に円形Spriteを生成
            maskImage.color = Color.white;

            // Mask component
            Mask mask = container.GetComponent<Mask>();
            mask.showMaskGraphic = false; // マスク自体は非表示

            // LayoutElement設定（HLGでのサイズ固定）
            LayoutElement containerLayout = container.GetComponent<LayoutElement>();
            containerLayout.minWidth = iconSize;
            containerLayout.minHeight = iconSize;
            containerLayout.preferredWidth = iconSize;
            containerLayout.preferredHeight = iconSize;
            containerLayout.flexibleWidth = 0f;
            containerLayout.flexibleHeight = 0f;

            // 実際のアイコンImage（子要素）
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(container.transform, false);

            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.sizeDelta = Vector2.zero;

            Image iconImage = iconObj.GetComponent<Image>();
            iconImage.sprite = iconSprite;
            iconImage.preserveAspect = true;

            return container;
        }

        /// <summary>
        /// <summary>
        /// メッセージバブルのPrefabをインスタンス化（プール経由）
        /// </summary>
        /// <param name="charID">キャラクターID（自分/相手の判定に使用）</param>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="isConsecutive">前回と同じ話者の連続メッセージか</param>
        /// <returns>生成されたGameObject</returns>
        private GameObject CreateMessageBubble(string charID, string text, bool isConsecutive = false)
        {
            using var _ = s_CreateMessageBubbleMarker.Auto();

            // バブルを取得（プールまたはPrefab）
            GameObject messageBubble = AcquireBubble();
            if (messageBubble == null)
            {
                return null;
            }

            // プール再利用時の状態汚染を防止: RectTransform をデフォルトにリセット
            RectTransform bubbleRect = messageBubble.GetComponent<RectTransform>();
            if (bubbleRect != null)
            {
                bubbleRect.anchorMin = new Vector2(0f, 1f);
                bubbleRect.anchorMax = new Vector2(1f, 1f);
                bubbleRect.pivot = new Vector2(0.5f, 1f);
                bubbleRect.sizeDelta = new Vector2(0f, UIConfig.bubbleInitialHeight);
            }

            // Imageコンポーネントを追加（背景表示用）
            Image bubbleImage = messageBubble.GetComponent<Image>();
            if (bubbleImage == null)
            {
                bubbleImage = messageBubble.AddComponent<Image>();
                bubbleImage.color = new Color(0.85f, 0.85f, 0.85f);
                bubbleImage.raycastTarget = false;
            }

            // LayoutElementを追加・設定（高さを自動調整）
            LayoutElement layoutElement = messageBubble.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = messageBubble.AddComponent<LayoutElement>();
            }
            float minH = UIConfig.bubbleMinHeight;
            layoutElement.minHeight = minH;
            layoutElement.preferredHeight = -1f;
            layoutElement.flexibleHeight = -1f;
            layoutElement.flexibleWidth = 1f;

            // ContentSizeFitter はバブル自体には付けない（入れ子 CSF の競合防止）
            ContentSizeFitter sizeFitter = messageBubble.GetComponent<ContentSizeFitter>();
            if (sizeFitter != null)
            {
                DestroyImmediate(sizeFitter);
            }

            // テキストコンポーネントの取得または作成（ConfigureBubble の前に必要）
            TextMeshProUGUI textComponent = messageBubble.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent == null)
            {
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(messageBubble.transform, false);
                textComponent = textObj.AddComponent<TextMeshProUGUI>();

                RectTransform textRect = textComponent.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0, 0);
                textRect.anchorMax = new Vector2(1, 1);
                textRect.offsetMin = new Vector2(10, 10);
                textRect.offsetMax = new Vector2(-10, -10);
            }

            // フォント・書式プロパティを設定（プール再利用時のリセット兼用）
            float fontScale = GetResponsiveFontScale();
            float responsiveFontSize = UIConfig.messageFontSize * fontScale;
            textComponent.fontStyle = FontStyles.Normal;
            textComponent.fontSize = responsiveFontSize;
            textComponent.alignment = TextAlignmentOptions.TopLeft;
            textComponent.textWrappingMode = TMPro.TextWrappingModes.Normal;
            textComponent.enableAutoSizing = false;
            if (m_JapaneseFontAsset != null)
            {
                textComponent.font = m_JapaneseFontAsset;
            }

            // テーマカラーのみ先行適用（ラッパー生成なし）
            // ConfigureBubble はテキスト高さ算出後に1回だけ呼ぶ
            ApplyThemeColor(messageBubble, charID);

            // 表示名・テキスト構築 (DisplayMode に応じた切替)
            // NPC: 名前行と本文行を改行で分離し、折り返し時に本文開始位置で揃える
            bool isSystemMessage = string.IsNullOrEmpty(charID) || charID.ToLower() == "system";
            bool isPlayerMsg = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.IsPlayer(charID) : charID == "player";
            bool isAnnotationCard = m_ActiveThreadType == ThreadType.Annotation;

            string finalText;
            string nameLineText = null; // 名前行の幅計算用 (名前がある場合のみ)
            if (isAnnotationCard)
            {
                // A型(注釈)スレッド: 情報カード表示。キャラクター名は省略し中央テキスト
                finalText = text;
                textComponent.alignment = TextAlignmentOptions.Center;
            }
            else if (isSystemMessage)
            {
                finalText = text;
            }
            else if (isPlayerMsg)
            {
                // Player バブルにはアイコン・名前を表示しない (仕様 6.2)
                finalText = text;
            }
            else
            {
                var profile = CharacterDatabase.Instance?.GetProfile(charID);
                var displayMode = profile?.DisplayMode ?? CharacterDisplayMode.NameOnly;

                if (displayMode == CharacterDisplayMode.IconOnly)
                {
                    finalText = text;
                }
                else
                {
                    string displayName = profile?.DisplayName ?? charID;
                    // 名前行と本文行を改行で分離
                    // 名前は小さめフォント + ボールド、行高さを縮小して1行占有感を軽減
                    float nameSize = responsiveFontSize * 0.75f;
                    nameLineText = $"{displayName}";
                    // body の未閉リッチテキストタグを補完し、name への逆伝播を防止
                    string safeBody = ChatTextParser.CloseUnclosedRichTextTags(text);
                    finalText = $"<line-height=80%><size={nameSize:F0}><b>{displayName}</b></size>\n</line-height>{safeBody}";
                }
            }
            // サブスレッド内: マークアップ ([link:...], [artifact:...]) を TMP リッチテキストに変換
            bool hasLinks = false;
            if (m_ActiveThreadType.HasValue)
            {
                string before = finalText;
                finalText = ChatTextParser.ParseThreadMarkup(finalText);
                hasLinks = finalText != before && finalText.Contains("<link=");
            }
            textComponent.text = finalText;

            // リンクが含まれる場合、クリックハンドラを設定
            if (hasLinks)
            {
                SetupLinkClickHandler(textComponent);
            }

            // --- バブル幅フィット + 高さ計算 ---
            // テキスト領域の左右パディング合計 (offsetMin.x=10 + |offsetMax.x|=10 = 20)
            float textPadH = 20f;
            // フォントメトリクス丸め誤差対策の安全マージン
            float safetyMargin = 4f;
            float maxWidthFromPercent = GetContainerWidth() * GetResponsiveBubblePercent();
            float maxBubbleWidth = Mathf.Min(maxWidthFromPercent, UIConfig.bubbleMaxWidthPx);

            // TMPの GetPreferredValues で折り返しなしのテキスト幅を取得
            // 名前行がある場合: 名前行と本文行それぞれの自然幅の大きい方を使う
            float naturalTextWidth;
            if (nameLineText != null)
            {
                // 本文のみの自然幅
                Vector2 bodyPref = textComponent.GetPreferredValues(text);
                // 名前行の自然幅 (リッチテキストタグ含む)
                float nameSize = UIConfig.messageFontSize * 0.75f;
                Vector2 namePref = textComponent.GetPreferredValues($"<size={nameSize:F0}><b>{nameLineText}</b></size>");
                naturalTextWidth = Mathf.Max(bodyPref.x, namePref.x);
            }
            else
            {
                Vector2 preferredSize = textComponent.GetPreferredValues(finalText);
                naturalTextWidth = preferredSize.x;
            }
            float fitWidth = Mathf.Min(naturalTextWidth + textPadH + safetyMargin, maxBubbleWidth);

            // バブル幅を仮確定 (ConfigureBubble 後に FinalizeBubbleSize で最終調整)
            layoutElement.preferredWidth = fitWidth;
            layoutElement.flexibleWidth = 0f;

            // バブルの最終処理（ラッパー生成 + 配置）
            ConfigureBubble(messageBubble, charID, isConsecutive);

            // ラッパー内での最終幅に基づいて高さを再計算
            FinalizeBubbleSize(messageBubble, minH);

            AnimateBubbleIn(messageBubble);

            // 履歴復元中はタイプライター・遅延スクロールをスキップ（即時表示）
            if (!m_IsRestoringHistory)
            {
                // タイプライター効果を適用
                ApplyTypewriterEffect(textComponent);

                // スクロール吸着を有効化（タイプライター効果中も追従）
                if (!m_IsUserScrolling)
                {
                    m_PinnedToBottom = true;
                    // タイプライター効果後に最終的な位置調整
                    float typewriterDuration = m_EnableTypewriterEffect ? text.Length * m_TypewriterSpeed : 0f;
                    Invoke(nameof(DelayedAutoScroll), typewriterDuration + 0.1f);
                }
            }

            return messageBubble;
        }

        /// <summary>
        /// 遅延AutoScroll（タイプライター効果後に呼ばれる）
        /// </summary>
        private void DelayedAutoScroll()
        {
            if (!m_IsUserScrolling)
            {
                AutoScroll();
            }
        }

        /// <summary>
        /// テキストにタイプライター効果を適用（1文字ずつ表示）
        /// </summary>
        private void ApplyTypewriterEffect(TextMeshProUGUI textComponent)
        {
            if (!m_EnableTypewriterEffect || textComponent == null || string.IsNullOrEmpty(textComponent.text))
            {
                // タイプライター無効時は全文字を即座に表示
                if (textComponent != null)
                    textComponent.maxVisibleCharacters = int.MaxValue;
                return;
            }

            // 前回のタイプライター tween が残っていればキルし、全文字表示にリセット
            DOTween.Kill(textComponent, complete: true);

            int totalCharacters = textComponent.text.Length;
            textComponent.maxVisibleCharacters = 0;
            m_IsTypewriterActive = true;
            m_LastTypewriterTarget = textComponent;

            DOTween.To(
                () => textComponent.maxVisibleCharacters,
                x => textComponent.maxVisibleCharacters = x,
                totalCharacters,
                totalCharacters * m_TypewriterSpeed
            ).SetEase(Ease.Linear)
             .SetUpdate(true)
             .SetTarget(textComponent)
             .OnComplete(() =>
             {
                 textComponent.maxVisibleCharacters = totalCharacters;
                 m_IsTypewriterActive = false;
                 m_LastTypewriterTarget = null;
                 OnTypewriterCompleted?.Invoke();
             });
        }

        /// <summary>
        /// バブルが出現する際のアニメーション演出
        /// </summary>
        private void AnimateBubbleIn(GameObject bubble)
        {
            if (bubble == null) return;
            if (m_IsRestoringHistory)
            {
                bubble.transform.localScale = Vector3.one;
                return;
            }
            bubble.transform.localScale = Vector3.zero;
            bubble.transform.DOScale(1f, UIConfig.bubbleAnimationDuration).SetEase(Ease.OutBack).SetUpdate(true);
        }

        /// <summary>
        /// プールまたはPrefabからバブルGameObjectを取得する
        /// </summary>
        /// <param name="prefab">使用するPrefab（nullの場合はMessageBubblePrefabを使用）</param>
        /// <returns>取得したバブルGameObject</returns>
        private GameObject AcquireBubble(GameObject prefab = null)
        {
            if (m_ScrollRect == null || m_ScrollRect.content == null)
            {
                Debug.LogError("ChatController: Cannot acquire bubble. ScrollRect or content is not assigned.");
                return null;
            }

            // Prefabが指定されていない場合はMessageBubblePrefabを使用
            if (prefab == null)
            {
                prefab = m_MessageBubblePrefab;
            }

            // プール未設定時はフォールバック
            if (m_MessageBubblePool == null)
            {
                EnsureMessageBubblePool();
            }

            GameObject bubble;

            // プールから取得（プールが有効で、MessageBubblePrefabを使用する場合）
            if (m_MessageBubblePool != null && prefab == m_MessageBubblePrefab)
            {
                bubble = m_MessageBubblePool.Get(m_ScrollRect.content);
            }
            else
            {
                // フォールバック: 直接Instantiate
                if (prefab == null)
                {
                    Debug.LogError("ChatController: Cannot acquire bubble. Prefab is null.");
                    return null;
                }
                bubble = Instantiate(prefab, m_ScrollRect.content);
            }

            if (bubble != null)
            {
                bubble.SetActive(true);
            }

            return bubble;
        }

        /// <summary>
        /// バブルの最終処理（配置、アニメーション、スクロール）
        /// </summary>
        /// <param name="bubble">処理対象のバブル</param>
        /// <param name="charID">キャラクターID（配置とテーマカラーの決定に使用）</param>
        /// <param name="isConsecutive">前回と同じ話者の連続メッセージか</param>
        private void FinalizeBubble(GameObject bubble, string charID, bool isConsecutive = false)
        {
            if (bubble == null) return;

            // プレイヤー判定・テーマカラー・配置を共通処理で設定
            ConfigureBubble(bubble, charID, isConsecutive);

            // アニメーション演出
            AnimateBubbleIn(bubble);

            // ユーザーが過去ログを見ていない場合のみAutoScroll()を実行
            if (!m_IsUserScrolling)
            {
                AutoScroll();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 指定された LineTag を持つ MessageBubble を検索する
        /// </summary>
        public MessageBubble FindBubbleByLineTag(string lineTag)
        {
            if (string.IsNullOrEmpty(lineTag) || m_ScrollRect == null || m_ScrollRect.content == null)
                return null;

            var content = m_ScrollRect.content;
            for (int i = 0; i < content.childCount; i++)
            {
                var mb = content.GetChild(i).GetComponentInChildren<MessageBubble>();
                if (mb != null && mb.LineTag == lineTag)
                    return mb;
            }
            return null;
        }

        /// <summary>
        /// 新しいメッセージをチャットに追加
        /// </summary>
        /// <param name="charID">キャラクターID（例: "player", "npc_001"）</param>
        /// <param name="text">メッセージテキスト</param>
        public void AddMessage(string charID, string text)
        {
            AddMessage(charID, text, null);
        }

        /// <summary>
        /// LineTag 付きメッセージをチャットに追加（矛盾指摘システム対応）
        /// </summary>
        /// <param name="charID">キャラクターID</param>
        /// <param name="text">メッセージテキスト</param>
        /// <param name="lineTag">矛盾判定用の識別タグ（null なら通常メッセージ）</param>
        public void AddMessage(string charID, string text, string lineTag)
        {
            using var _ = s_AddMessageMarker.Auto();

            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("ChatController: Attempted to add empty message.");
                return;
            }

            // 履歴に記録（復元中は二重記録しない）
            if (!m_IsRestoringHistory)
            {
                m_ChatHistory.Add(new SavedChatMessage
                {
                    Type = ChatMessageType.Normal,
                    CharacterID = charID,
                    Text = text,
                    LineTag = lineTag
                });
            }

            // 連続メッセージ判定
            bool isConsecutive = (charID == m_LastSpeaker);

            // メッセージバブルの生成と追加
            GameObject messageBubble = CreateMessageBubble(charID, text, isConsecutive);
            if (messageBubble == null)
            {
                Debug.LogError($"[AddMessage] CreateMessageBubble returned null for charID='{charID}', text='{text}'");
                return;
            }

            // LineTag が設定されている場合、MessageBubble コンポーネントをアタッチ
            if (!string.IsNullOrEmpty(lineTag))
            {
                MessageBubble bubble = messageBubble.GetComponent<MessageBubble>();
                if (bubble == null)
                {
                    bubble = messageBubble.AddComponent<MessageBubble>();
                }
                bubble.Initialize(lineTag, messageBubble.GetComponent<UnityEngine.UI.Image>());

                // 矛盾指摘のポインターイベントを受け取るために raycastTarget を有効化
                Image bubbleImg = messageBubble.GetComponent<Image>();
                if (bubbleImg != null)
                {
                    bubbleImg.raycastTarget = true;
                }
            }

            // 話者を更新
            m_LastSpeaker = charID;

            // Note: AutoScrollはCreateMessageBubble内のFinalizeBubbleで実行される
        }

        /// <summary>
        /// 画像メッセージをチャットに追加
        /// </summary>
        /// <param name="charID">キャラクターID</param>
        /// <param name="imageSprite">表示する画像Sprite</param>
        public void AddImageMessage(string charID, Sprite imageSprite)
        {
            using var _ = s_AddImageMessageMarker.Auto();

            if (imageSprite == null)
            {
                Debug.LogWarning("ChatController: Attempted to add image message with null sprite.");
                return;
            }

            // 履歴に記録（復元中は二重記録しない）
            if (!m_IsRestoringHistory)
            {
                m_ChatHistory.Add(new SavedChatMessage
                {
                    Type = ChatMessageType.Image,
                    CharacterID = charID,
                    ImageResourcePath = imageSprite.name
                });
            }

            // 遅延初期化: ImageBubblePrefabが未設定の場合はランタイム生成
            EnsureImageBubbleTemplate();

            // ImageBubblePrefabが設定されていない場合はテキストバブルにフォールバック
            GameObject prefab = m_ImageBubblePrefab != null ? m_ImageBubblePrefab : m_MessageBubblePrefab;
            if (prefab == null)
            {
                Debug.LogError("ChatController: No prefab available for image message.");
                return;
            }

            // バブルを取得（プールまたはPrefab）
            GameObject imageBubble = AcquireBubble(prefab);
            if (imageBubble == null)
            {
                return;
            }

            // 画像を表示するImageコンポーネントを検索して設定
            // ImageBubblePrefab内に "ImageContent" という名前の子オブジェクトを想定
            Transform imageContentTransform = imageBubble.transform.Find("ImageContent");
            Image imageContent = imageContentTransform != null
                ? imageContentTransform.GetComponent<Image>()
                : null;

            if (imageContent == null)
            {
                // 子階層から最初のImage（背景以外）を探す
                Image[] images = imageBubble.GetComponentsInChildren<Image>();
                foreach (var img in images)
                {
                    if (img.gameObject != imageBubble)
                    {
                        imageContent = img;
                        break;
                    }
                }
            }

            if (imageContent != null)
            {
                imageContent.sprite = imageSprite;
                imageContent.preserveAspect = true;

                // 画像サイズを制限
                RectTransform imgRect = imageContent.GetComponent<RectTransform>();
                if (imgRect != null)
                {
                    float aspectRatio = (float)imageSprite.texture.width / imageSprite.texture.height;
                    float width = Mathf.Min(m_ImageMaxWidth, imageSprite.texture.width);
                    float height = width / aspectRatio;
                    if (height > m_ImageMaxHeight)
                    {
                        height = m_ImageMaxHeight;
                        width = height * aspectRatio;
                    }
                    imgRect.sizeDelta = new Vector2(width, height);
                }

                // 画像のフェードイン演出
                imageContent.color = new Color(1, 1, 1, 0);
                imageContent.DOFade(1f, 0.6f).SetUpdate(true);
            }
            else
            {
                // フォールバック: テキストとして画像名を表示
                TextMeshProUGUI textComponent = imageBubble.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    // 表示名をCharacterDatabaseから取得（fallback: IDそのまま）
                    string displayName = CharacterDatabase.Instance != null
                        ? CharacterDatabase.Instance.GetDisplayName(charID)
                        : charID;
                    textComponent.text = $"{displayName}: [Image: {imageSprite.name}]";
                }
            }

            // 画像バブルの幅をコンテンツサイズに合わせる
            LayoutElement imgLayout = imageBubble.GetComponent<LayoutElement>();
            if (imgLayout == null) imgLayout = imageBubble.AddComponent<LayoutElement>();
            RectTransform imgContentRect = imageContent?.GetComponent<RectTransform>();
            float imgBubbleWidth = imgContentRect != null ? imgContentRect.sizeDelta.x + 20f : m_ImageMaxWidth + 20f;
            imgLayout.preferredWidth = imgBubbleWidth;
            imgLayout.flexibleWidth = 0f;

            // 連続メッセージ判定
            bool isConsecutive = (charID == m_LastSpeaker);

            // バブルの最終処理（配置、アニメーション、スクロール）
            FinalizeBubble(imageBubble, charID, isConsecutive);

            // 話者を更新
            m_LastSpeaker = charID;
        }

        /// <summary>
        /// システムメッセージ（通知）をチャットに追加
        /// キャラクターの発言ではなく、中央揃えのグレーテキストで表示する
        /// 例: 「グループに参加しました」「新しいトピックが解放されました」
        ///
        /// ルーティング規則:
        /// - 「【SYSTEM】」プレフィックス: 端末状態通知 (将来的にステータスバーへ移行予定)
        /// - それ以外: 進行通知 (インライン表示を維持)
        /// 現時点では両方ともインライン表示するが、分岐ポイントを用意しておく。
        /// </summary>
        /// <param name="text">システムメッセージのテキスト</param>
        public void AddSystemMessage(string text)
        {
            using var _ = s_AddSystemMessageMarker.Auto();

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // ルーティング分岐: 端末状態通知 vs 進行通知
            // 将来的に status カテゴリはステータスバー/トーストへ移行する
            // bool isStatusMessage = text.StartsWith("【SYSTEM】");
            // TODO: isStatusMessage == true の場合、ステータスバー/トーストへルーティング

            // 履歴に記録（復元中は二重記録しない）
            if (!m_IsRestoringHistory)
            {
                m_ChatHistory.Add(new SavedChatMessage
                {
                    Type = ChatMessageType.System,
                    Text = text
                });
            }

            if (m_ScrollRect == null || m_ScrollRect.content == null)
            {
                Debug.LogError("ChatController: Cannot create system message. ScrollRect or content is not assigned.");
                return;
            }

            if (m_MessageBubblePool == null)
            {
                EnsureMessageBubblePool();
            }

            GameObject systemBubble;

            // プールから取得（設定されていれば）
            if (m_MessageBubblePool != null)
            {
                systemBubble = m_MessageBubblePool.Get(m_ScrollRect.content);
            }
            else
            {
                // フォールバック: 直接Instantiate
                if (m_MessageBubblePrefab == null)
                {
                    Debug.LogError("ChatController: Cannot create system message. MessageBubblePrefab is not assigned.");
                    return;
                }
                systemBubble = Instantiate(m_MessageBubblePrefab, m_ScrollRect.content);
            }
            systemBubble.SetActive(true);

            // ストレッチアンカーで全幅（テキストは中央揃え、アンカーはVLG互換に）
            RectTransform rectTransform = systemBubble.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);
            }

            // Imageコンポーネントを追加（背景表示用）
            Image bubbleBackground = systemBubble.GetComponent<Image>();
            if (bubbleBackground == null)
            {
                bubbleBackground = systemBubble.AddComponent<Image>();
                bubbleBackground.color = new Color(0.15f, 0.15f, 0.2f, 0.7f);
            }
            // サブスレッド内: 型色でシステムメッセージ背景をティント
            if (m_ActiveThreadType.HasValue)
            {
                Color typeColor = GetThreadTypeColor(m_ActiveThreadType.Value);
                bubbleBackground.color = Color.Lerp(UIConfig.systemMessageBgColor, typeColor, 0.10f);
            }
            else
            {
                bubbleBackground.color = UIConfig.systemMessageBgColor;
            }
            bubbleBackground.raycastTarget = false;

            // システムメッセージは薄いバー表示のため、角丸スプライトを適用しない。
            // 9-slice の cornerRadius がバー高さに対して大きすぎ、表示が圧迫される。
            // プール再利用時に前バブルのスプライトが残存するため、明示的にクリアする。
            bubbleBackground.sprite = null;
            bubbleBackground.type = Image.Type.Simple;

            // LayoutElementを追加
            LayoutElement layoutElement = systemBubble.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = systemBubble.AddComponent<LayoutElement>();
            }
            float sysMinH = UIConfig.systemMessageMinHeight;
            layoutElement.minHeight = sysMinH;
            layoutElement.preferredHeight = -1f;

            // ContentSizeFitterを追加
            ContentSizeFitter sizeFitter = systemBubble.GetComponent<ContentSizeFitter>();
            if (sizeFitter == null)
            {
                sizeFitter = systemBubble.AddComponent<ContentSizeFitter>();
            }
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            // テキストコンポーネントの取得または作成
            TextMeshProUGUI textComponent = systemBubble.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent == null)
            {
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(systemBubble.transform, false);
                textComponent = textObj.AddComponent<TextMeshProUGUI>();

                RectTransform textRect = textComponent.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0, 0);
                textRect.anchorMax = new Vector2(1, 1);
                textRect.offsetMin = new Vector2(10, 10);
                textRect.offsetMax = new Vector2(-10, -10);
            }

            // プール再利用時にも確実にシステムメッセージ用の設定を適用
            textComponent.fontSize = UIConfig.systemMessageFontSize;
            textComponent.color = UIConfig.systemMessageTextColor;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontStyle = FontStyles.Italic;
            textComponent.textWrappingMode = TMPro.TextWrappingModes.Normal;
            textComponent.enableAutoSizing = false;
            if (m_JapaneseFontAsset != null)
            {
                textComponent.font = m_JapaneseFontAsset;
            }

            textComponent.text = text;

            // テキストのメッシュを強制更新して高さを計算
            textComponent.ForceMeshUpdate();

            // テキストの高さに基づいてバブルの高さを動的に設定
            if (layoutElement != null)
            {
                float textHeight = textComponent.preferredHeight;
                layoutElement.preferredHeight = Mathf.Max(sysMinH, textHeight + UIConfig.bubbleTextPadding);
            }

            // レイアウトを即座に更新
            Canvas.ForceUpdateCanvases();

            AnimateBubbleIn(systemBubble);

            if (!m_IsUserScrolling)
            {
                AutoScroll();
            }

            // システムメッセージ後は連続が途切れる
            m_LastSpeaker = null;
        }

        /// <summary>
        /// 入力欄と送信ボタンの有効/無効を切り替える
        /// ScenarioManagerの入力ロック連動で使用
        /// </summary>
        /// <param name="enabled">有効にする場合true</param>
        public void SetInputEnabled(bool enabled)
        {
            if (m_InputField != null)
            {
                m_InputField.interactable = enabled;
            }
            if (m_SendButton != null)
            {
                m_SendButton.interactable = enabled;
            }
        }

        /// <summary>タイプライター効果が進行中かどうか</summary>
        public bool IsTypewriterActive => m_IsTypewriterActive;

        /// <summary>
        /// 現在進行中のタイプライター効果を即座に完了させる。
        /// タップスキップ用。
        /// </summary>
        public void CompleteCurrentTypewriter()
        {
            if (m_LastTypewriterTarget != null)
            {
                DOTween.Kill(m_LastTypewriterTarget, complete: true);
            }
        }

        /// <summary>
        /// タイピングインジケーターの表示/非表示を切り替え
        /// </summary>
        /// <param name="show">表示する場合true</param>
        public void ShowTypingIndicator(bool show)
        {
            if (m_TypingIndicator == null)
            {
                EnsureTypingIndicator();
            }

            if (m_TypingIndicator != null)
            {
                // ConfigureBubble がラッパー（NpcRow）を生成するため、
                // ラッパーごと表示/非表示・移動する必要がある
                Transform wrapper = m_TypingIndicator.transform.parent;
                bool hasWrapper = wrapper != null && m_ScrollRect != null
                    && wrapper != m_ScrollRect.content;
                GameObject target = hasWrapper ? wrapper.gameObject : m_TypingIndicator;

                target.SetActive(show);

                if (hasWrapper)
                {
                    m_TypingIndicator.SetActive(show);
                }

                if (show)
                {
                    StartTypingAnimation();
                    target.transform.SetAsLastSibling();
                    AutoScroll();
                }
                else
                {
                    StopTypingAnimation();
                }
            }
        }

        /// <summary>
        /// 専用タイピングインジケーターの生成（ドットアニメーション方式）
        /// プールは使わず専用 GameObject を生成する
        /// </summary>
        private void EnsureTypingIndicator()
        {
            if (m_TypingIndicator != null || m_ScrollRect == null || m_ScrollRect.content == null)
            {
                return;
            }

            // 専用インジケーターを生成（プール不使用）
            GameObject indicatorObj = new GameObject("TypingIndicator",
                typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            indicatorObj.transform.SetParent(m_ScrollRect.content, false);

            RectTransform rect = indicatorObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, 48f);

            // 背景
            Image bgImage = indicatorObj.GetComponent<Image>();
            bgImage.color = UIConfig.typingIndicatorColor;
            bgImage.raycastTarget = false;

            // LayoutElement
            LayoutElement layoutElement = indicatorObj.GetComponent<LayoutElement>();
            layoutElement.minHeight = 48f;
            layoutElement.preferredHeight = 48f;
            layoutElement.flexibleWidth = 1f;

            // 3つのドットを生成（中央配置）
            float dotSize = 8f;
            float dotSpacing = 6f;
            float totalWidth = 3 * dotSize + 2 * dotSpacing;
            float startX = -totalWidth / 2f + dotSize / 2f;

            for (int i = 0; i < 3; i++)
            {
                GameObject dot = new GameObject($"Dot{i}",
                    typeof(RectTransform), typeof(Image));
                dot.transform.SetParent(indicatorObj.transform, false);

                RectTransform dotRect = dot.GetComponent<RectTransform>();
                dotRect.anchorMin = new Vector2(0.5f, 0.5f);
                dotRect.anchorMax = new Vector2(0.5f, 0.5f);
                dotRect.pivot = new Vector2(0.5f, 0.5f);
                dotRect.sizeDelta = new Vector2(dotSize, dotSize);
                dotRect.anchoredPosition = new Vector2(
                    startX + i * (dotSize + dotSpacing), 0f);

                Image dotImage = dot.GetComponent<Image>();
                dotImage.color = UIConfig.typingIndicatorTextColor;
                dotImage.raycastTarget = false;
            }

            // NPC側に配置（ラッパー生成 + テーマカラー適用）
            ConfigureBubble(indicatorObj, "npc");

            // インジケーター固有の色を ConfigureBubble 後に再適用
            bgImage = indicatorObj.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = UIConfig.typingIndicatorColor;
            }

            m_TypingIndicator = indicatorObj;
            m_TypingIndicator.SetActive(false);
        }

        /// <summary>
        /// ドットのスケールアニメーションを開始（順次バウンス）
        /// </summary>
        private void StartTypingAnimation()
        {
            StopTypingAnimation();
            if (m_TypingIndicator == null) return;

            for (int i = 0; i < 3; i++)
            {
                Transform dot = m_TypingIndicator.transform.Find($"Dot{i}");
                if (dot == null) continue;

                dot.localScale = Vector3.one;
                Tween t = dot.DOScale(1.4f, 0.35f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetDelay(i * 0.15f)
                    .SetUpdate(true);
                m_TypingDotTweens.Add(t);
            }
        }

        /// <summary>
        /// ドットアニメーションを停止
        /// </summary>
        private void StopTypingAnimation()
        {
            foreach (var t in m_TypingDotTweens)
            {
                if (t != null && t.IsActive()) t.Kill();
            }
            m_TypingDotTweens.Clear();
        }



        /// <summary>
        /// 選択肢を表示する
        /// </summary>
        /// <param name="options">選択肢のテキストリスト</param>
        /// <param name="onSelected">選択時のコールバック (index)</param>
        public void ShowChoices(List<string> options, System.Action<int> onSelected)
        {
            using var _ = s_ShowChoicesMarker.Auto();

            // 遅延初期化: ChoiceButtonPrefab/Containerが未設定の場合はランタイム生成
            EnsureChoiceUIElements();

            if (m_ChoiceButtonPrefab == null || m_ChoiceContainer == null)
            {
                Debug.LogError("ChatController: ChoiceButtonPrefab or ChoiceContainer is not assigned after initialization.");
                return;
            }

            // 既存の選択肢をクリア
            HideChoices();

            m_ChoiceContainer.gameObject.SetActive(true);
            m_ChoiceContainer.SetAsLastSibling();

            // 入力欄を非表示にする（既にSetInputEnabled(false)されていれば、ここでは単に見た目の制御）
            if (m_InputField != null) m_InputField.gameObject.SetActive(false);
            if (m_SendButton != null) m_SendButton.gameObject.SetActive(false);

            for (int i = 0; i < options.Count; i++)
            {
                GameObject buttonObj = Instantiate(m_ChoiceButtonPrefab, m_ChoiceContainer);
                buttonObj.SetActive(true);
                buttonObj.name = "Choice" + (char)('A' + i); // For Test Automation (MVPTestHelper)

                // 選択肢ボタンのスタイリング（UIConfig の choiceButtonColor を使用）
                Image buttonBg = buttonObj.GetComponent<Image>();
                if (buttonBg != null)
                {
                    buttonBg.color = UIConfig.choiceButtonColor;
                    // 角丸スプライト適用 (選択肢ボタンにも共通)
                    // NOTE: ?? は Unity "fake null" を透過するため明示判定
                    Sprite choiceSprite = UIConfig.bubbleSprite != null ? UIConfig.bubbleSprite : BubbleSpriteFactory.GetOrCreateRoundedSprite(UIConfig.bubbleCornerRadius);
                    if (choiceSprite != null)
                    {
                        buttonBg.sprite = choiceSprite;
                        buttonBg.type = Image.Type.Sliced;
                    }
                }

                // ボタンのテキスト設定
                TextMeshProUGUI btnText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.text = options[i];
                    btnText.color = UIConfig.playerTextColor;
                }

                // クリックイベント設定（既存リスナーをクリアしてから登録）
                Button btn = buttonObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    ChoiceButtonHandler handler = buttonObj.GetComponent<ChoiceButtonHandler>();
                    if (handler == null)
                    {
                        handler = buttonObj.AddComponent<ChoiceButtonHandler>();
                    }
                    handler.Initialize(this, i, onSelected);
                    btn.onClick.AddListener(handler.OnClick);
                }
            }

            // ChoiceContainer 表示によるコンテンツ高さ変動で m_IsUserScrolling が
            // 誤って true になる場合があるためリセットしてから最下部へスクロール
            m_IsUserScrolling = false;
            AutoScroll();
        }

        /// <summary>
        /// 選択肢を非表示にする
        /// </summary>
        public void HideChoices()
        {
            m_IsFadingChoices = false;
            if (m_ChoiceContainer != null)
            {
                // DOTween のフェードアニメーションを完了させてから破棄
                for (int i = m_ChoiceContainer.childCount - 1; i >= 0; i--)
                {
                    var go = m_ChoiceContainer.GetChild(i).gameObject;
                    DOTween.Kill(go);
                    DestroyImmediate(go);
                }
                m_ChoiceContainer.gameObject.SetActive(false);
            }

            // 入力欄を再表示 (showInputField 有効時のみ)
            if (UIConfig.showInputField)
            {
                if (m_InputField != null) m_InputField.gameObject.SetActive(true);
                if (m_SendButton != null) m_SendButton.gameObject.SetActive(true);
            }

            // 選択肢表示中にズレたスクロール位置を修正
            m_IsUserScrolling = false;
            AutoScroll();
        }

        public void AutoScroll()
        {
            using var _ = s_AutoScrollMarker.Auto();

            if (m_ScrollRect == null) return;

            // m_PinnedToBottom が true なら、コンテンツ追加による一時的な
            // m_IsUserScrolling 誤検知を無視してスクロールを実行する
            if (m_IsUserScrolling && !m_PinnedToBottom)
            {
                return;
            }
            if (m_IsUserScrolling && m_PinnedToBottom)
            {
                m_IsUserScrolling = false;
            }

            // ピンニングは CreateMessageBubble のタイプライター開始時のみ設定
            // ここではワンショットスクロールのみ実行

            if (m_AutoScrollScheduled)
            {
                return;
            }

            m_AutoScrollScheduled = true;
            Invoke(nameof(PerformAutoScroll), UIConfig.autoScrollDelay);
        }

        private void PerformAutoScroll()
        {
            m_AutoScrollScheduled = false;

            if (m_ScrollRect == null) return;
            if (m_IsUserScrolling && !m_PinnedToBottom) return;

            // レイアウトを強制更新
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_ScrollRect.content);

            // 既存のスクロールアニメーションをキャンセル
            m_ScrollTween?.Kill();

            // スムーズスクロールで最下部へ (BL-001: フェード付きスクロール吸着)
            m_IsAutoScrolling = true;
            float currentPos = m_ScrollRect.verticalNormalizedPosition;

            // 既にほぼ最下部にいる場合は即時移動 (微小な差はアニメ不要)
            if (currentPos < 0.02f)
            {
                m_ScrollRect.verticalNormalizedPosition = 0f;
                m_LastScrollPosition = 0f;
                m_IsAutoScrolling = false;
                return;
            }

            m_ScrollTween = DOTween.To(
                () => m_ScrollRect.verticalNormalizedPosition,
                x => m_ScrollRect.verticalNormalizedPosition = x,
                0f,
                SmoothScrollDuration
            )
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                m_LastScrollPosition = 0f;
                m_IsAutoScrolling = false;
                m_ScrollTween = null;
            });
        }

        private void OnDisable()
        {
            if (m_AutoScrollScheduled)
            {
                CancelInvoke(nameof(PerformAutoScroll));
                m_AutoScrollScheduled = false;
            }

            m_ScrollTween?.Kill();
            m_ScrollTween = null;

            if (m_FadeInCoroutine != null)
            {
                StopCoroutine(m_FadeInCoroutine);
                m_FadeInCoroutine = null;
            }

            m_IsUserScrolling = false;
            m_IsUserDragging = false;
            m_PinnedToBottom = true;
            m_IsAutoScrolling = false;
        }

        private void EnsureChoiceUIElements()
        {
            if (m_ChoiceContainer == null)
            {
                m_ChoiceContainer = CreateRuntimeChoiceContainer();
            }

            if (m_ChoiceButtonPrefab == null)
            {
                m_RuntimeChoiceButtonTemplate = CreateChoiceButtonTemplate();
                m_ChoiceButtonPrefab = m_RuntimeChoiceButtonTemplate;
            }
        }

        private void EnsureImageBubbleTemplate()
        {
            if (m_ImageBubblePrefab == null && m_RuntimeImageBubbleTemplate == null)
            {
                m_RuntimeImageBubbleTemplate = CreateImageBubbleTemplate();
                m_ImageBubblePrefab = m_RuntimeImageBubbleTemplate;
            }
        }

        private void EnsureInputControls()
        {
            // 入力欄非表示モードではランタイム生成をスキップ
            if (!UIConfig.showInputField) return;

            if (m_InputField != null && m_SendButton != null)
            {
                return;
            }

            const float footerHeight = 160f;
            Transform parentForFooter = transform.parent != null ? transform.parent : transform;
            GameObject footer = new GameObject("AutoFooter", typeof(RectTransform), typeof(Image));
            AssignUILayer(footer);
            footer.transform.SetParent(parentForFooter, false);
            footer.transform.SetAsLastSibling();

            RectTransform footerRect = footer.GetComponent<RectTransform>();
            footerRect.anchorMin = new Vector2(0f, 0f);
            footerRect.anchorMax = new Vector2(1f, 0f);
            footerRect.pivot = new Vector2(0.5f, 0f);
            footerRect.offsetMin = new Vector2(0f, 0f);
            footerRect.offsetMax = new Vector2(0f, footerHeight);
        }

        private void EnsureInventoryShortcutButton()
        {
            if (m_InventoryShortcutButton != null)
            {
                return;
            }

            RectTransform rootRect = transform as RectTransform;
            if (rootRect == null)
            {
                return;
            }

            GameObject buttonObj = new GameObject("InventoryShortcutButton", typeof(RectTransform), typeof(Image), typeof(Button));
            AssignUILayer(buttonObj);
            buttonObj.transform.SetParent(transform, false);
            buttonObj.transform.SetAsLastSibling();

            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(1f, 1f);
            buttonRect.anchorMax = new Vector2(1f, 1f);
            buttonRect.pivot = new Vector2(1f, 1f);
            buttonRect.anchoredPosition = new Vector2(-12f, -12f);
            buttonRect.sizeDelta = new Vector2(60f, 34f);

            Image buttonBg = buttonObj.GetComponent<Image>();
            buttonBg.color = new Color(0.12f, 0.12f, 0.16f, 0.88f);
            buttonBg.raycastTarget = true;

            Button button = buttonObj.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = buttonBg;

            ColorBlock cb = button.colors;
            cb.highlightedColor = new Color(0.18f, 0.18f, 0.24f, 0.92f);
            cb.pressedColor = new Color(0.22f, 0.22f, 0.3f, 1f);
            button.colors = cb;
            button.onClick.AddListener(OpenInventoryOverlay);

            TextMeshProUGUI label = CreateTMPText(buttonObj.transform, "Text", new Color(0.82f, 0.84f, 0.92f), FontStyles.Bold);
            RectTransform labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = false;
            label.fontSize = UIFontConfig.Instance.captionFontSize;
            label.text = "INV";

            m_InventoryShortcutButton = button;
        }

        private void OpenInventoryOverlay()
        {
            DashboardController dashboard = FindFirstObjectByType<DashboardController>();
            if (dashboard == null)
            {
                Debug.LogWarning("ChatController: DashboardController not found. Inventory overlay cannot be opened.");
                return;
            }

            dashboard.ShowInventoryOverlay();
        }

        private Transform CreateRuntimeChoiceContainer()
        {
            if (m_ScrollRect == null || m_ScrollRect.content == null)
            {
                Debug.LogError("ChatController: Cannot create choice container. ScrollRect or content is missing.");
                return null;
            }

            GameObject container = new GameObject("AutoChoiceContainer", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter), typeof(LayoutElement));
            container.transform.SetParent(m_ScrollRect.content, false);
            RectTransform rect = container.GetComponent<RectTransform>();
            
            // レイアウトグループ内での配置設定
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            
            Image background = container.GetComponent<Image>();
            background.color = new Color(0.07f, 0.07f, 0.09f, 0f); // 背景は透明に（メッセージの流れと同化）

            VerticalLayoutGroup layoutGroup = container.GetComponent<VerticalLayoutGroup>();
            float cSpacing = UIConfig.choiceSpacing;
            // プレイヤーバブル風の右寄せレイアウト（ConfigureBubble と同じ計算）
            int edgePadC = UIConfig.wrapperEdgePadding;
            float containerWidthC = GetContainerWidth();
            float maxWidthFromPercent = containerWidthC * GetResponsiveBubblePercent();
            float maxWidthC = Mathf.Min(maxWidthFromPercent, UIConfig.bubbleMaxWidthPx);
            int choiceSideMargin = Mathf.Max(0, (int)(containerWidthC - maxWidthC - edgePadC));
            layoutGroup.spacing = cSpacing;
            layoutGroup.padding = new RectOffset(choiceSideMargin, edgePadC, 6, 10);
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;

            ContentSizeFitter fitter = container.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            LayoutElement le = container.GetComponent<LayoutElement>();
            le.flexibleWidth = 1f;

            container.SetActive(false);
            return container.transform;
        }

        private GameObject CreateChoiceButtonTemplate()
        {
            GameObject template = new GameObject("AutoChoiceButtonTemplate", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            template.transform.SetParent(transform, false);
            template.SetActive(false);

            Image buttonBackground = template.GetComponent<Image>();
            buttonBackground.color = UIConfig.choiceButtonColor;

            Button button = template.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = buttonBackground;

            ColorBlock cb = button.colors;
            cb.highlightedColor = UIConfig.choiceButtonHighlightColor;
            cb.pressedColor = UIConfig.choiceButtonPressedColor;
            button.colors = cb;

            LayoutElement layout = template.GetComponent<LayoutElement>();
            layout.minHeight = UIConfig.choiceButtonMinHeight;
            layout.preferredHeight = UIConfig.choiceButtonPreferredHeight;
            layout.flexibleWidth = 1f;

            RectTransform rect = template.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = new Vector2(0f, 0f);
            rect.offsetMax = new Vector2(0f, 0f);

            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            textObj.transform.SetParent(template.transform, false);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(16f, 8f);
            textRect.offsetMax = new Vector2(-16f, -8f);

            TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = "Choice";
            label.enableAutoSizing = true;
            label.fontSizeMin = UIConfig.choiceFontSizeMin;
            label.fontSizeMax = UIConfig.choiceFontSizeMax;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.color = UIConfig.choiceTextColor;
            label.raycastTarget = false;

            // 日本語フォントが設定されていればそれを使用、なければデフォルトフォントを使用
            if (m_JapaneseFontAsset != null)
            {
                label.font = m_JapaneseFontAsset;
            }
            else if (TMP_Settings.defaultFontAsset != null)
            {
                label.font = TMP_Settings.defaultFontAsset;
            }

            return template;
        }

        private GameObject CreateImageBubbleTemplate()
        {
            GameObject template = new GameObject("AutoImageBubblePrefab", typeof(RectTransform), typeof(Image), typeof(ContentSizeFitter));
            template.transform.SetParent(transform, false);
            template.SetActive(false);

            Image bubbleBackground = template.GetComponent<Image>();
            bubbleBackground.color = Color.white;
            bubbleBackground.raycastTarget = true;

            ContentSizeFitter sizeFitter = template.GetComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            GameObject imageContainer = new GameObject("ImageContent", typeof(RectTransform), typeof(Image));
            imageContainer.transform.SetParent(template.transform, false);
            RectTransform imageRect = imageContainer.GetComponent<RectTransform>();
            imageRect.anchorMin = Vector2.zero;
            imageRect.anchorMax = Vector2.one;
            imageRect.offsetMin = new Vector2(12f, 12f);
            imageRect.offsetMax = new Vector2(-12f, -12f);

            Image contentImage = imageContainer.GetComponent<Image>();
            contentImage.preserveAspect = true;

            return template;
        }

        private GameObject CreateMessageBubbleTemplate()
        {
            GameObject template = new GameObject("AutoMessageBubblePrefab", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            template.transform.SetParent(transform, false);
            template.SetActive(false);

            RectTransform bubbleRect = template.GetComponent<RectTransform>();
            bubbleRect.anchorMin = new Vector2(0f, 1f);
            bubbleRect.anchorMax = new Vector2(1f, 1f);
            bubbleRect.pivot = new Vector2(0.5f, 1f);
            bubbleRect.sizeDelta = new Vector2(0f, 72f);

            Image bubbleBackground = template.GetComponent<Image>();
            bubbleBackground.color = Color.white;
            bubbleBackground.raycastTarget = false;

            LayoutElement layout = template.GetComponent<LayoutElement>();
            layout.minHeight = 40f;
            layout.flexibleWidth = 1f;

            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            textObj.transform.SetParent(template.transform, false);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(16f, 10f);
            textRect.offsetMax = new Vector2(-16f, -10f);

            TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = "Message";
            label.textWrappingMode = TextWrappingModes.Normal;
            label.color = Color.white;
            label.enableAutoSizing = false;
            label.fontSize = UIFontConfig.Instance.titleFontSize;
            label.alignment = TextAlignmentOptions.TopLeft;
            label.raycastTarget = false;

            // 日本語フォントが設定されていればそれを使用、なければデフォルトフォントを使用
            if (m_JapaneseFontAsset != null)
            {
                label.font = m_JapaneseFontAsset;
            }
            else if (TMP_Settings.defaultFontAsset != null)
            {
                label.font = TMP_Settings.defaultFontAsset;
            }

            return template;
        }

        private GameObject GetSafeMessageBubbleTemplate()
        {
            if (m_RuntimeMessageBubbleTemplate == null)
            {
                m_RuntimeMessageBubbleTemplate = CreateMessageBubbleTemplate();
            }

            return m_RuntimeMessageBubbleTemplate;
        }

        private TMP_InputField CreateRuntimeInputField(Transform parent)
        {
            GameObject inputObj = new GameObject("InputField", typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
            AssignUILayer(inputObj);
            inputObj.transform.SetParent(parent, false);

            RectTransform inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0f, 0f);
            inputRect.anchorMax = new Vector2(1f, 1f);
            inputRect.offsetMin = new Vector2(20f, 20f);
            inputRect.offsetMax = new Vector2(-150f, -20f);

            Image inputBg = inputObj.GetComponent<Image>();
            inputBg.color = new Color(0.16f, 0.16f, 0.2f, 0.95f);
            inputBg.raycastTarget = true;

            TMP_InputField inputField = inputObj.GetComponent<TMP_InputField>();
            inputField.textViewport = CreateInputTextViewport(inputObj.transform, out TextMeshProUGUI textComponent, out TextMeshProUGUI placeholderComponent);
            inputField.textComponent = textComponent;
            inputField.placeholder = placeholderComponent;
            inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
            inputField.characterLimit = 0;

            return inputField;
        }

        private RectTransform CreateInputTextViewport(Transform parent, out TextMeshProUGUI textComponent, out TextMeshProUGUI placeholderComponent)
        {
            GameObject textArea = new GameObject("TextArea", typeof(RectTransform));
            AssignUILayer(textArea);
            textArea.transform.SetParent(parent, false);
            RectTransform textAreaRect = textArea.GetComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(10f, 8f);
            textAreaRect.offsetMax = new Vector2(-10f, -8f);

            GameObject viewportObj = new GameObject("TextViewport", typeof(RectTransform), typeof(RectMask2D));
            AssignUILayer(viewportObj);
            viewportObj.transform.SetParent(textArea.transform, false);
            RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            placeholderComponent = CreateTMPText(viewportObj.transform, "Placeholder", new Color(0.65f, 0.65f, 0.7f, 0.85f), FontStyles.Italic);
            placeholderComponent.text = "Type a message...";

            textComponent = CreateTMPText(viewportObj.transform, "Text", Color.white, FontStyles.Normal);
            textComponent.textWrappingMode = TextWrappingModes.Normal;
            textComponent.text = string.Empty;

            return viewportRect;
        }

        private Button CreateRuntimeSendButton(Transform parent)
        {
            GameObject buttonObj = new GameObject("SendButton", typeof(RectTransform), typeof(Image), typeof(Button));
            AssignUILayer(buttonObj);
            buttonObj.transform.SetParent(parent, false);

            RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1f, 0f);
            btnRect.anchorMax = new Vector2(1f, 1f);
            btnRect.pivot = new Vector2(1f, 0.5f);
            btnRect.offsetMin = new Vector2(-130f, 20f);
            btnRect.offsetMax = new Vector2(-20f, -20f);

            Image btnImage = buttonObj.GetComponent<Image>();
            btnImage.color = new Color(0.25f, 0.55f, 1.0f, 1.0f);
            btnImage.raycastTarget = true;

            Button button = buttonObj.GetComponent<Button>();
            button.targetGraphic = btnImage;
            button.transition = Selectable.Transition.ColorTint;

            TextMeshProUGUI label = CreateTMPText(buttonObj.transform, "Text", Color.white, FontStyles.Bold);
            RectTransform labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 20f;
            label.fontSizeMax = 40f;
            label.text = "Send";

            return button;
        }

        private TextMeshProUGUI CreateTMPText(Transform parent, string name, Color color, FontStyles fontStyle)
        {
            GameObject textObj = new GameObject(name, typeof(RectTransform));
            AssignUILayer(textObj);
            textObj.transform.SetParent(parent, false);

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.color = color;
            tmp.fontSize = UIFontConfig.Instance.titleFontSize;
            tmp.fontStyle = fontStyle;
            tmp.text = string.Empty;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = UIFontConfig.Instance.captionFontSize;
            tmp.fontSizeMax = UIFontConfig.Instance.titleFontSize * 1.7f;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.raycastTarget = false;

            // 日本語フォントが設定されていればそれを使用、なければデフォルトフォントを使用
            if (m_JapaneseFontAsset != null)
            {
                tmp.font = m_JapaneseFontAsset;
            }
            else if (TMP_Settings.defaultFontAsset != null)
            {
                tmp.font = TMP_Settings.defaultFontAsset;
            }

            return tmp;
        }

        private void AssignUILayer(GameObject go)
        {
            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer >= 0)
            {
                go.layer = uiLayer;
            }
        }

        private sealed class ChoiceButtonHandler : MonoBehaviour
        {
            private ChatController m_Owner;
            private int m_Index;
            private Action<int> m_OnSelected;

            public void Initialize(ChatController owner, int index, Action<int> onSelected)
            {
                m_Owner = owner;
                m_Index = index;
                m_OnSelected = onSelected;
            }

            public void OnClick()
            {
                // 全ボタンを即座に無効化（二重クリック防止）
                if (m_Owner != null && m_Owner.m_ChoiceContainer != null)
                {
                    foreach (Transform child in m_Owner.m_ChoiceContainer)
                    {
                        var btn = child.GetComponent<Button>();
                        if (btn != null) btn.interactable = false;
                    }
                }

                // 選択を即時通知（Yarn ダイアログの続行のため）
                m_OnSelected?.Invoke(m_Index);

                if (m_Owner != null)
                {
                    m_Owner.FadeAndHideChoices(m_Index);
                }
            }
        }

        /// <summary>
        /// 選択済みボタンを即破棄、未選択をフェードアウト後にクリーンアップ。
        /// 再入ガード付き（二重クリック時の二重実行を防止）。
        /// </summary>
        private bool m_IsFadingChoices;
        internal void FadeAndHideChoices(int selectedIndex)
        {
            if (m_ChoiceContainer == null) return;
            if (m_IsFadingChoices) return;
            m_IsFadingChoices = true;

            float fadeTime = 0.25f;

            for (int i = m_ChoiceContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = m_ChoiceContainer.GetChild(i);
                if (child == null) continue;

                if (i == selectedIndex)
                {
                    // 選択済みは即破棄（RunOptionsAsync がプレイヤーメッセージとして再表示する）
                    DestroyImmediate(child.gameObject);
                }
                else
                {
                    // 未選択はフェードアウト
                    CanvasGroup cg = child.gameObject.GetComponent<CanvasGroup>();
                    if (cg == null) cg = child.gameObject.AddComponent<CanvasGroup>();
                    cg.DOFade(0f, fadeTime).SetTarget(child.gameObject).SetUpdate(true);
                }
            }

            // フェードアウト完了後にクリーンアップ
            DOVirtual.DelayedCall(fadeTime + 0.05f, () =>
            {
                HideChoices();
                m_IsFadingChoices = false;
            }).SetUpdate(true);
        }

        public void OnSubmit()
        {
            if (m_InputField == null) return;

            string text = m_InputField.text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                AddMessage("player", text);
                m_InputField.text = "";
                
                // 入力欄にフォーカスを戻す
                m_InputField.ActivateInputField();
            }
        }

        /// <summary>
        /// チャット履歴をクリア（プール返却方式）
        /// 注意: ConfigureBubble が生成するラッパー（PlayerRow/NpcRow）と
        /// プール管理対象のバブル本体を正しく分離して処理する
        /// </summary>
        public void ClearMessages()
        {
            if (m_ScrollRect == null || m_ScrollRect.content == null)
            {
                return;
            }

            // チャプター遷移時の状態汚染を防止: スクロール状態をリセット
            m_IsUserScrolling = false;
            m_IsUserDragging = false;
            m_PinnedToBottom = true;
            m_AutoScrollScheduled = false;
            m_IsAutoScrolling = false;

            // 選択肢を確実に非表示
            HideChoices();

            // タイピングインジケーターを非表示にしてからプール返却
            if (m_TypingIndicator != null)
            {
                ShowTypingIndicator(false);
            }

            // プール未設定時は従来通りDestroy
            if (m_MessageBubblePool == null)
            {
                int childCount = m_ScrollRect.content.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    Transform child = m_ScrollRect.content.GetChild(i);
                    if (child != null && child.gameObject != m_TopSpacer)
                    {
                        Destroy(child.gameObject);
                    }
                }
                m_ChatHistory.Clear();
                m_TypingIndicator = null;
                if (m_TopSpacer != null) m_TopSpacer.transform.SetAsFirstSibling();
                return;
            }

            // プール追跡済みオブジェクト（実際のバブル）を正しく返却
            // ReturnAll はバブル本体をラッパーから取り出してプールコンテナに戻す
            m_MessageBubblePool.ReturnAll();

            // 残存する子オブジェクト（空ラッパー、ChoiceContainer、TypingIndicator以外）を破棄
            for (int i = m_ScrollRect.content.childCount - 1; i >= 0; i--)
            {
                Transform child = m_ScrollRect.content.GetChild(i);
                if (child == null) continue;
                // TopSpacerは保持
                if (m_TopSpacer != null && child.gameObject == m_TopSpacer) continue;
                // 選択肢コンテナは保持
                if (m_ChoiceContainer != null && child == m_ChoiceContainer) continue;
                // TypingIndicatorのラッパーは保持（動的に親を辿って判定）
                if (m_TypingIndicator != null && m_TypingIndicator.transform.parent != null
                    && child == m_TypingIndicator.transform.parent) continue;
                Destroy(child.gameObject);
            }

            // TypingIndicatorは保持（プール管理外のため破棄しない）
            // 次回使用時も同じインスタンスを再利用
            if (m_TypingIndicator != null)
            {
                ShowTypingIndicator(false); // 非表示にしておく
            }

            // TopSpacerを先頭に維持
            if (m_TopSpacer != null) m_TopSpacer.transform.SetAsFirstSibling();

            m_ChatHistory.Clear();

            // 連続メッセージ状態をリセット
            m_LastSpeaker = null;
        }

        /// <summary>
        /// 現在のチャット履歴を取得（セーブ用）
        /// </summary>
        public List<SavedChatMessage> GetChatHistory()
        {
            return m_ChatHistory.ToList();
        }

        /// <summary>
        /// 保存されたチャット履歴からバブルを復元する（ロード用）
        /// アニメーションなしで即座に表示する
        /// </summary>
        public void RestoreChatHistory(List<SavedChatMessage> history)
        {
            // 常に一旦クリアする（空履歴のときも。さもないと別スレッドのバブルが残り、
            // DialogueRunner 状態と表示だけが不整合になる）
            ClearMessages();

            if (history == null || history.Count == 0)
            {
                return;
            }

            m_IsRestoringHistory = true;
            try
            {
                foreach (var msg in history)
                {
                    switch (msg.Type)
                    {
                        case ChatMessageType.Normal:
                            // 古いセーブデータには名前リッチテキストが埋め込まれている場合がある。
                            // AddMessage → CreateMessageBubble で名前が再付加されるため、
                            // 復元前にストリップして二重表示を防止する。
                            string bodyText = ChatTextParser.StripNamePrefix(msg.Text);
                            AddMessage(msg.CharacterID, bodyText, msg.LineTag);
                            break;
                        case ChatMessageType.System:
                            AddSystemMessage(msg.Text);
                            break;
                        case ChatMessageType.Image:
                            if (!string.IsNullOrEmpty(msg.ImageResourcePath))
                            {
                                Sprite sprite = Resources.Load<Sprite>(msg.ImageResourcePath);
                                if (sprite != null)
                                {
                                    AddImageMessage(msg.CharacterID, sprite);
                                }
                                else
                                {
                                    // 画像が見つからない場合はテキストでフォールバック
                                    AddMessage(msg.CharacterID, $"[Image: {msg.ImageResourcePath}]");
                                }
                            }
                            break;
                    }
                }
            }
            finally
            {
                m_IsRestoringHistory = false;
            }

            // 復元完了後、最下部へスクロール
            AutoScroll();
        }

        #endregion

        #region Subthread System

        /// <summary>現在表示中のスレッドID (null = メインスレッド)</summary>
        public string ActiveThreadId => m_ActiveThreadId;

        /// <summary>現在表示中のスレッドの種別 (null = メインスレッド)</summary>
        public ThreadType? ActiveThreadType => m_ActiveThreadType;

        /// <summary>アクティブスレッドの種別を設定する</summary>
        public void SetActiveThreadType(ThreadType? type)
        {
            m_ActiveThreadType = type;
        }

        // ThreadType 別色定数 (ThreadSwitcherController と同一値)
        private static readonly Color s_TypeColorAnnotation = new Color(0.29f, 0.56f, 0.85f, 1f);
        private static readonly Color s_TypeColorTracking = new Color(0.30f, 0.69f, 0.31f, 1f);
        private static readonly Color s_TypeColorScout = new Color(1f, 0.60f, 0f, 1f);
        private static readonly Color s_TypeColorBranch = new Color(0.61f, 0.15f, 0.69f, 1f);

        /// <summary>ThreadType から色を取得</summary>
        private static Color GetThreadTypeColor(ThreadType type)
        {
            switch (type)
            {
                case ThreadType.Annotation: return s_TypeColorAnnotation;
                case ThreadType.Tracking: return s_TypeColorTracking;
                case ThreadType.Scout: return s_TypeColorScout;
                case ThreadType.Branch: return s_TypeColorBranch;
                default: return s_TypeColorAnnotation;
            }
        }

        /// <summary>
        /// スレッドを切り替える。現在のスレッドの履歴とスクロール位置を保存し、
        /// 対象スレッドの履歴を復元する。
        /// </summary>
        /// <param name="threadId">切替先スレッドID (null = メインスレッド)</param>
        /// <param name="history">初回切替時に使用する履歴 (省略可)</param>
        public void SwitchToThread(string threadId, List<SavedChatMessage> history = null)
        {
            // 同じスレッドなら何もしない
            if (m_ActiveThreadId == threadId) return;

            // 現在のスレッドの履歴とスクロール位置を保存
            string currentKey = m_ActiveThreadId ?? "__main__";
            m_ThreadHistories[currentKey] = m_ChatHistory.ToList();
            SyncDeclaredThreadChatHistoryFromCache(currentKey);
            if (m_ScrollRect != null)
            {
                m_ThreadScrollPositions[currentKey] = m_ScrollRect.verticalNormalizedPosition;
            }

            // 切替先を設定
            m_ActiveThreadId = threadId;
            string targetKey = threadId ?? "__main__";

            // 対象スレッドの履歴を取得
            // 空のキャッシュだけが残っていると TryGetValue が true になり、
            // ScenarioManager 側の thread.ChatHistory（中身あり）が無視されて履歴が消える。
            m_ThreadHistories.TryGetValue(targetKey, out var cachedList);
            bool cacheHasMessages = cachedList != null && cachedList.Count > 0;
            bool paramHasMessages = history != null && history.Count > 0;

            List<SavedChatMessage> targetHistory;
            if (cacheHasMessages)
            {
                targetHistory = new List<SavedChatMessage>(cachedList);
            }
            else if (paramHasMessages)
            {
                targetHistory = new List<SavedChatMessage>(history);
            }
            else
            {
                targetHistory = new List<SavedChatMessage>();
            }

            if (!cacheHasMessages && cachedList != null && cachedList.Count == 0)
            {
                m_ThreadHistories.Remove(targetKey);
            }

            // フラッシュ防止: コンテンツを非表示にしてから復元
            var cg = GetOrAddContentCanvasGroup();
            if (cg != null) cg.alpha = 0f;

            // 前回のフェードインが実行中なら停止
            if (m_FadeInCoroutine != null)
            {
                StopCoroutine(m_FadeInCoroutine);
                m_FadeInCoroutine = null;
            }

            // 履歴を復元 (非表示のまま実行 — ユーザーにはフラッシュが見えない)
            RestoreChatHistory(targetHistory);

            // RestoreChatHistory が予約した AutoScroll をキャンセル
            // (スクロール位置は RestoreScrollAndFadeIn で明示的に設定する)
            if (m_AutoScrollScheduled)
            {
                CancelInvoke(nameof(PerformAutoScroll));
                m_AutoScrollScheduled = false;
            }

            // レイアウト確定後にスクロール位置を復元し、フェードインで表示
            m_FadeInCoroutine = StartCoroutine(RestoreScrollAndFadeIn(targetKey));
        }

        /// <summary>
        /// スレッド切替後のレイアウト確定 → スクロール位置復元 → フェードイン。
        /// ClearMessages→RestoreChatHistory 間の空白フレームをユーザーに見せない。
        /// </summary>
        private System.Collections.IEnumerator RestoreScrollAndFadeIn(string targetKey)
        {
            // レイアウト確定を待つ (2フレーム: ネストLayoutGroup対応)
            yield return null;
            Canvas.ForceUpdateCanvases();
            yield return null;

            // スクロール位置を復元 (即時 — スレッド切替時はアニメ不要)
            if (m_ScrollRect != null)
            {
                m_IsAutoScrolling = true;
                if (m_ThreadScrollPositions.TryGetValue(targetKey, out float scrollPos))
                {
                    m_ScrollRect.verticalNormalizedPosition = scrollPos;
                }
                else
                {
                    m_ScrollRect.verticalNormalizedPosition = 0f;
                }
                m_LastScrollPosition = m_ScrollRect.verticalNormalizedPosition;
                m_IsAutoScrolling = false;
            }

            // フェードインで表示
            var cg = GetOrAddContentCanvasGroup();
            if (cg != null)
            {
                cg.DOKill(); // 前回のフェードがあればキャンセル
                cg.DOFade(1f, ThreadSwitchFadeDuration).SetEase(Ease.OutQuad);
            }

            m_FadeInCoroutine = null;
        }

        /// <summary>
        /// ScrollRect content の CanvasGroup を取得または追加する (遅延初期化)
        /// </summary>
        private CanvasGroup GetOrAddContentCanvasGroup()
        {
            if (m_ContentCanvasGroup != null) return m_ContentCanvasGroup;
            if (m_ScrollRect == null || m_ScrollRect.content == null) return null;
            m_ContentCanvasGroup = m_ScrollRect.content.GetComponent<CanvasGroup>();
            if (m_ContentCanvasGroup == null)
                m_ContentCanvasGroup = m_ScrollRect.content.gameObject.AddComponent<CanvasGroup>();
            return m_ContentCanvasGroup;
        }

        /// <summary>
        /// 全スレッドの履歴を取得（セーブ用）。
        /// 現在表示中のスレッドはm_ChatHistoryから、それ以外はキャッシュから返す。
        /// </summary>
        public Dictionary<string, List<SavedChatMessage>> GetAllThreadHistories()
        {
            // 現在のスレッドの履歴を更新
            string currentKey = m_ActiveThreadId ?? "__main__";
            m_ThreadHistories[currentKey] = m_ChatHistory.ToList();
            SyncDeclaredThreadChatHistoryFromCache(currentKey);
            return new Dictionary<string, List<SavedChatMessage>>(m_ThreadHistories);
        }

        /// <summary>
        /// 表示中のチャット履歴を SubthreadData.ChatHistory に反映する（セーブ・再切替の整合用）。
        /// </summary>
        private void SyncDeclaredThreadChatHistoryFromCache(string threadKey)
        {
            if (threadKey == "__main__")
            {
                return;
            }

            if (!m_ThreadHistories.TryGetValue(threadKey, out var list) || list == null)
            {
                return;
            }

            var scenario = FindFirstObjectByType<ScenarioManager>();
            if (scenario == null)
            {
                return;
            }

            SubthreadData td = scenario.GetDeclaredThread(threadKey);
            if (td == null)
            {
                return;
            }

            td.ChatHistory.Clear();
            td.ChatHistory.AddRange(list);
        }

        /// <summary>
        /// 現在のActiveThreadIdを設定（ロード復元用。SwitchToThreadと違いUI操作なし）
        /// </summary>
        public void SetActiveThreadId(string threadId)
        {
            m_ActiveThreadId = threadId;
        }

        #endregion

        #region Bubble Visual Helpers

        /// <summary>
        /// ConfigureBubble でラッパーに配置した後、最終的なバブル幅に基づいてテキスト高さを計算し
        /// LayoutElement.preferredHeight を確定する。
        /// これにより「幅計算 → ラッパー配置 → 利用可能幅変化 → 高さ不整合」問題を解消する。
        /// </summary>
        private void FinalizeBubbleSize(GameObject bubble, float minHeight)
        {
            // ラッパー内でのレイアウトを確定（2回呼ぶことでネスト構造のレイアウトを確実に更新）
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(bubble.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();

            TextMeshProUGUI textComponent = bubble.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent == null) return;

            // テキストメッシュを最終幅で再生成
            textComponent.ForceMeshUpdate();

            LayoutElement bubbleLayout = bubble.GetComponent<LayoutElement>();
            if (bubbleLayout != null)
            {
                float textHeight = textComponent.preferredHeight;
                // 名前行のline-height=80%やフォントメトリクス丸めの安全マージンを追加
                bubbleLayout.preferredHeight = Mathf.Max(minHeight, textHeight + UIConfig.bubbleTextPadding + 4f);
            }

            // 最終レイアウト更新
            Canvas.ForceUpdateCanvases();
        }

        /// <summary>
        /// バブルの Image に角丸スプライトと影を適用する。
        /// </summary>
        private void ApplyBubbleVisuals(GameObject bubble)
        {
            Image img = bubble.GetComponent<Image>();
            if (img == null) return;

            // 角丸スプライト
            // NOTE: ?? は C# 参照 null のみ判定し Unity の "fake null" (Inspector 未設定の
            //       SerializedField) を透過する。Unity overloaded != で明示判定する。
            Sprite sprite = UIConfig.bubbleSprite != null ? UIConfig.bubbleSprite : BubbleSpriteFactory.GetOrCreateRoundedSprite(UIConfig.bubbleCornerRadius);
            #if UNITY_EDITOR
            if (sprite == null)
            {
                Debug.LogWarning($"[ApplyBubbleVisuals] sprite is null. cornerRadius={UIConfig.bubbleCornerRadius}, bubbleSprite={(UIConfig.bubbleSprite != null ? "set" : "null")}");
            }
            #endif
            if (sprite != null)
            {
                img.sprite = sprite;
                img.type = Image.Type.Sliced;
                img.pixelsPerUnitMultiplier = 1f;
            }

            // Shadow コンポーネント
            if (UIConfig.bubbleShadowEnabled)
            {
                Shadow shadow = bubble.GetComponent<Shadow>();
                if (shadow == null)
                {
                    shadow = bubble.AddComponent<Shadow>();
                }
                shadow.effectColor = UIConfig.bubbleShadowColor;
                shadow.effectDistance = UIConfig.bubbleShadowDistance;
            }
        }

        #endregion
    }
}
