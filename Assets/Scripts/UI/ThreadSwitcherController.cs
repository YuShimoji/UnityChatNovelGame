using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 複数サブスレッドの切替UIを管理するコントローラ。
    /// 左サイドバーでスレッド一覧を表示し、種別グループごとに整理する。
    /// ハンバーガーボタンまたは通知バナータップで開閉する。
    /// </summary>
    public class ThreadSwitcherController : MonoBehaviour
    {
        [SerializeField] private ChatController m_ChatController;

        private ScenarioManager m_ScenarioManager;

        // サイドバー UI
        private GameObject m_SidebarPanel;
        private RectTransform m_SidebarRt;
        private Transform m_SidebarContent;
        private GameObject m_Overlay;
        private bool m_IsSidebarOpen;
        private Tween m_SidebarTween;

        // ハンバーガーボタン (左上)
        private GameObject m_HamburgerButton;
        private TextMeshProUGUI m_HamburgerBadge;

        // スレッド型ヘッダーバー (チャットエリア上部の色帯)
        private GameObject m_ThreadHeaderBar;
        private Image m_ThreadHeaderBarBg;
        private TextMeshProUGUI m_ThreadHeaderBarLabel;

        // スレッドエントリ管理
        private class ThreadEntry
        {
            public string ThreadId;
            public string DisplayName;
            public ThreadType Type;
            public GameObject ListItemObj;
            public TextMeshProUGUI NameLabel;
            public TextMeshProUGUI BadgeLabel;
            public TextMeshProUGUI TypeLabel;
            public Image ItemBg;
        }
        private readonly List<ThreadEntry> m_Threads = new List<ThreadEntry>();

        // サイドバー内のグループヘッダー (種別ごとに1つ)
        private readonly Dictionary<ThreadType, GameObject> m_GroupHeaders
            = new Dictionary<ThreadType, GameObject>();

        // 色定数
        private static readonly Color SidebarBgColor = new Color(0.10f, 0.12f, 0.18f, 0.98f);
        private static readonly Color OverlayColor = new Color(0f, 0f, 0f, 0.5f);
        private static readonly Color ItemColor = new Color(0.14f, 0.16f, 0.24f, 1f);
        private static readonly Color ActiveItemColor = new Color(0.22f, 0.26f, 0.38f, 1f);
        private static readonly Color BadgeColor = new Color(0.9f, 0.3f, 0.3f, 1f);
        private static readonly Color MainEntryColor = new Color(0.18f, 0.20f, 0.30f, 1f);
        private static readonly Color GroupHeaderColor = new Color(0.12f, 0.14f, 0.20f, 1f);

        // ThreadType 別色
        private static readonly Color TypeColorAnnotation = new Color(0.29f, 0.56f, 0.85f, 1f); // #4A90D9
        private static readonly Color TypeColorTracking = new Color(0.30f, 0.69f, 0.31f, 1f);   // #4CAF50
        private static readonly Color TypeColorScout = new Color(1f, 0.60f, 0f, 1f);             // #FF9800
        private static readonly Color TypeColorBranch = new Color(0.61f, 0.15f, 0.69f, 1f);      // #9C27B0

        private const float SidebarWidth = 240f;
        private const float SlideAnimDuration = 0.25f;

        // 通知バナー
        private GameObject m_NotificationBanner;
        private Image m_NotificationBg;
        private TextMeshProUGUI m_NotificationLabel;
        private CanvasGroup m_NotificationCanvasGroup;
        private string m_NotificationThreadId;
        private Tween m_NotificationTween;
        private const float NotificationDuration = 3.5f;
        private static readonly Color NotificationBgColor = new Color(0.12f, 0.14f, 0.22f, 0.95f);

        private void Start()
        {
            m_ScenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (m_ChatController == null)
            {
                m_ChatController = FindFirstObjectByType<ChatController>();
            }

            if (m_ScenarioManager != null)
            {
                m_ScenarioManager.OnThreadDeclared += OnThreadDeclared;
                m_ScenarioManager.OnThreadMessageAdded += OnThreadMessageAdded;
            }

            CreateUI();
            m_HamburgerButton.SetActive(false);
        }

        private void OnDestroy()
        {
            if (m_ScenarioManager != null)
            {
                m_ScenarioManager.OnThreadDeclared -= OnThreadDeclared;
                m_ScenarioManager.OnThreadMessageAdded -= OnThreadMessageAdded;
            }
            m_SidebarTween?.Kill();
            m_NotificationTween?.Kill();
        }

        #region UI Creation

        private void CreateUI()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("ThreadSwitcherController: Canvas not found.");
                return;
            }

            CreateHamburgerButton(canvas);
            CreateOverlay(canvas);
            CreateSidebar(canvas);
            CreateThreadHeaderBar(canvas);
            CreateNotificationBanner(canvas);
        }

        private void CreateHamburgerButton(Canvas canvas)
        {
            m_HamburgerButton = new GameObject("ThreadHamburger");
            m_HamburgerButton.transform.SetParent(canvas.transform, false);
            var rt = m_HamburgerButton.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(8f, -76f);
            rt.sizeDelta = new Vector2(40f, 36f);

            var bg = m_HamburgerButton.AddComponent<Image>();
            bg.color = new Color(0.18f, 0.22f, 0.32f, 0.9f);

            var btn = m_HamburgerButton.AddComponent<Button>();
            btn.onClick.AddListener(ToggleSidebar);

            // ハンバーガーアイコン (≡)
            var iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(m_HamburgerButton.transform, false);
            var iconRt = iconObj.AddComponent<RectTransform>();
            iconRt.anchorMin = Vector2.zero;
            iconRt.anchorMax = Vector2.one;
            iconRt.offsetMin = new Vector2(2f, 2f);
            iconRt.offsetMax = new Vector2(-2f, -2f);

            var iconLabel = iconObj.AddComponent<TextMeshProUGUI>();
            iconLabel.fontSize = 18f;
            iconLabel.alignment = TextAlignmentOptions.Center;
            iconLabel.color = Color.white;
            iconLabel.text = "\u2261"; // ≡
            AssignDefaultFont(iconLabel);

            // 未読合計バッジ (右上)
            var badgeObj = new GameObject("TotalBadge");
            badgeObj.transform.SetParent(m_HamburgerButton.transform, false);
            var badgeRt = badgeObj.AddComponent<RectTransform>();
            badgeRt.anchorMin = new Vector2(1f, 1f);
            badgeRt.anchorMax = new Vector2(1f, 1f);
            badgeRt.pivot = new Vector2(0.5f, 0.5f);
            badgeRt.anchoredPosition = new Vector2(2f, 2f);
            badgeRt.sizeDelta = new Vector2(18f, 18f);

            var badgeBg = badgeObj.AddComponent<Image>();
            badgeBg.color = BadgeColor;

            var badgeLabelObj = new GameObject("Label");
            badgeLabelObj.transform.SetParent(badgeObj.transform, false);
            var badgeLabelRt = badgeLabelObj.AddComponent<RectTransform>();
            badgeLabelRt.anchorMin = Vector2.zero;
            badgeLabelRt.anchorMax = Vector2.one;
            badgeLabelRt.offsetMin = Vector2.zero;
            badgeLabelRt.offsetMax = Vector2.zero;

            m_HamburgerBadge = badgeLabelObj.AddComponent<TextMeshProUGUI>();
            m_HamburgerBadge.fontSize = 9f;
            m_HamburgerBadge.alignment = TextAlignmentOptions.Center;
            m_HamburgerBadge.color = Color.white;
            m_HamburgerBadge.text = "";
            AssignDefaultFont(m_HamburgerBadge);
            badgeObj.SetActive(false);
        }

        private void CreateOverlay(Canvas canvas)
        {
            m_Overlay = new GameObject("SidebarOverlay");
            m_Overlay.transform.SetParent(canvas.transform, false);
            var rt = m_Overlay.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var bg = m_Overlay.AddComponent<Image>();
            bg.color = OverlayColor;

            var btn = m_Overlay.AddComponent<Button>();
            btn.onClick.AddListener(CloseSidebar);

            // 透明ターゲットグラフィック
            bg.raycastTarget = true;

            m_Overlay.SetActive(false);
        }

        private void CreateSidebar(Canvas canvas)
        {
            m_SidebarPanel = new GameObject("ThreadSidebar");
            m_SidebarPanel.transform.SetParent(canvas.transform, false);
            m_SidebarRt = m_SidebarPanel.AddComponent<RectTransform>();
            m_SidebarRt.anchorMin = new Vector2(0f, 0f);
            m_SidebarRt.anchorMax = new Vector2(0f, 1f);
            m_SidebarRt.pivot = new Vector2(0f, 0.5f);
            m_SidebarRt.anchoredPosition = new Vector2(-SidebarWidth, 0f); // 初期: 画面外
            m_SidebarRt.sizeDelta = new Vector2(SidebarWidth, 0f);

            var bg = m_SidebarPanel.AddComponent<Image>();
            bg.color = SidebarBgColor;

            // ScrollView でスレッドが多くてもスクロール可能
            var scrollObj = new GameObject("SidebarScroll");
            scrollObj.transform.SetParent(m_SidebarPanel.transform, false);
            var scrollRt = scrollObj.AddComponent<RectTransform>();
            scrollRt.anchorMin = Vector2.zero;
            scrollRt.anchorMax = Vector2.one;
            scrollRt.offsetMin = new Vector2(0f, 0f);
            scrollRt.offsetMax = new Vector2(0f, -70f); // 上部にタイトル余白

            var scrollRect = scrollObj.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            // マスク
            var maskObj = new GameObject("Viewport");
            maskObj.transform.SetParent(scrollObj.transform, false);
            var maskRt = maskObj.AddComponent<RectTransform>();
            maskRt.anchorMin = Vector2.zero;
            maskRt.anchorMax = Vector2.one;
            maskRt.offsetMin = Vector2.zero;
            maskRt.offsetMax = Vector2.zero;
            var maskImg = maskObj.AddComponent<Image>();
            maskImg.color = Color.white;
            maskObj.AddComponent<Mask>().showMaskGraphic = false;

            scrollRect.viewport = maskRt;

            // コンテンツ
            var contentObj = new GameObject("Content");
            contentObj.transform.SetParent(maskObj.transform, false);
            var contentRt = contentObj.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0f, 1f);
            contentRt.anchorMax = new Vector2(1f, 1f);
            contentRt.pivot = new Vector2(0.5f, 1f);
            contentRt.anchoredPosition = Vector2.zero;
            contentRt.sizeDelta = new Vector2(0f, 0f);

            var vlg = contentObj.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(6, 6, 4, 8);
            vlg.spacing = 2f;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;

            var csf = contentObj.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentRt;
            m_SidebarContent = contentObj.transform;

            // サイドバータイトル
            var titleObj = new GameObject("SidebarTitle");
            titleObj.transform.SetParent(m_SidebarPanel.transform, false);
            var titleRt = titleObj.AddComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0f, 1f);
            titleRt.anchorMax = new Vector2(1f, 1f);
            titleRt.pivot = new Vector2(0.5f, 1f);
            titleRt.anchoredPosition = new Vector2(0f, -8f);
            titleRt.sizeDelta = new Vector2(0f, 50f);

            var titleLabel = titleObj.AddComponent<TextMeshProUGUI>();
            titleLabel.fontSize = 15f;
            titleLabel.alignment = TextAlignmentOptions.MidlineLeft;
            titleLabel.color = new Color(0.7f, 0.72f, 0.8f, 1f);
            titleLabel.text = "  Threads";
            titleLabel.fontStyle = FontStyles.Bold;
            AssignDefaultFont(titleLabel);

            m_SidebarPanel.SetActive(true); // 常にアクティブ (位置で可視制御)
            m_IsSidebarOpen = false;
        }

        private void CreateThreadHeaderBar(Canvas canvas)
        {
            m_ThreadHeaderBar = new GameObject("ThreadHeaderBar");
            m_ThreadHeaderBar.transform.SetParent(canvas.transform, false);
            var barRt = m_ThreadHeaderBar.AddComponent<RectTransform>();
            barRt.anchorMin = new Vector2(0f, 1f);
            barRt.anchorMax = new Vector2(1f, 1f);
            barRt.pivot = new Vector2(0.5f, 1f);
            barRt.anchoredPosition = new Vector2(0f, -70f);
            barRt.sizeDelta = new Vector2(0f, 28f);

            m_ThreadHeaderBarBg = m_ThreadHeaderBar.AddComponent<Image>();
            m_ThreadHeaderBarBg.color = Color.clear;

            var barLabelObj = new GameObject("BarLabel");
            barLabelObj.transform.SetParent(m_ThreadHeaderBar.transform, false);
            var barLabelRt = barLabelObj.AddComponent<RectTransform>();
            barLabelRt.anchorMin = Vector2.zero;
            barLabelRt.anchorMax = Vector2.one;
            barLabelRt.offsetMin = new Vector2(54f, 0f); // ハンバーガーボタン分右に寄せる
            barLabelRt.offsetMax = new Vector2(-12f, 0f);

            m_ThreadHeaderBarLabel = barLabelObj.AddComponent<TextMeshProUGUI>();
            m_ThreadHeaderBarLabel.fontSize = 12f;
            m_ThreadHeaderBarLabel.alignment = TextAlignmentOptions.MidlineLeft;
            m_ThreadHeaderBarLabel.color = Color.white;
            m_ThreadHeaderBarLabel.text = "";
            AssignDefaultFont(m_ThreadHeaderBarLabel);

            m_ThreadHeaderBar.SetActive(false);
        }

        private void CreateNotificationBanner(Canvas canvas)
        {
            m_NotificationBanner = new GameObject("ThreadNotificationBanner");
            m_NotificationBanner.transform.SetParent(canvas.transform, false);
            var notifRt = m_NotificationBanner.AddComponent<RectTransform>();
            notifRt.anchorMin = new Vector2(0.5f, 1f);
            notifRt.anchorMax = new Vector2(0.5f, 1f);
            notifRt.pivot = new Vector2(0.5f, 1f);
            notifRt.anchoredPosition = new Vector2(0f, -100f);
            notifRt.sizeDelta = new Vector2(280f, 36f);

            m_NotificationBg = m_NotificationBanner.AddComponent<Image>();
            m_NotificationBg.color = NotificationBgColor;

            m_NotificationCanvasGroup = m_NotificationBanner.AddComponent<CanvasGroup>();
            m_NotificationCanvasGroup.alpha = 0f;

            var notifBtn = m_NotificationBanner.AddComponent<Button>();
            notifBtn.onClick.AddListener(OnNotificationClicked);

            var notifLabelObj = new GameObject("NotifLabel");
            notifLabelObj.transform.SetParent(m_NotificationBanner.transform, false);
            var notifLabelRt = notifLabelObj.AddComponent<RectTransform>();
            notifLabelRt.anchorMin = Vector2.zero;
            notifLabelRt.anchorMax = Vector2.one;
            notifLabelRt.offsetMin = new Vector2(10f, 0f);
            notifLabelRt.offsetMax = new Vector2(-10f, 0f);

            m_NotificationLabel = notifLabelObj.AddComponent<TextMeshProUGUI>();
            m_NotificationLabel.fontSize = 12f;
            m_NotificationLabel.alignment = TextAlignmentOptions.Center;
            m_NotificationLabel.color = Color.white;
            m_NotificationLabel.text = "";
            AssignDefaultFont(m_NotificationLabel);

            m_NotificationBanner.SetActive(false);
        }

        #endregion

        #region Thread Events

        private void OnThreadDeclared(string threadId, string displayName)
        {
            // 重複チェック
            foreach (var entry in m_Threads)
            {
                if (entry.ThreadId == threadId) return;
            }

            // 初回スレッド宣言時にハンバーガーボタンを表示
            if (!m_HamburgerButton.activeSelf)
            {
                m_HamburgerButton.SetActive(true);
            }

            AddThreadEntry(threadId, displayName);
        }

        private void OnThreadMessageAdded(string threadId)
        {
            foreach (var entry in m_Threads)
            {
                if (entry.ThreadId == threadId)
                {
                    UpdateBadge(entry);
                    UpdateHamburgerBadge();

                    // 非アクティブスレッドへのメッセージなら通知バナーを表示
                    string activeId = m_ChatController?.ActiveThreadId;
                    if (threadId != activeId)
                    {
                        ShowNotificationBanner(entry);
                    }
                    break;
                }
            }
        }

        #endregion

        #region Sidebar Entries

        private void AddThreadEntry(string threadId, string displayName)
        {
            var threadData = m_ScenarioManager?.GetDeclaredThread(threadId);
            ThreadType threadType = threadData?.Type ?? ThreadType.Annotation;

            // グループヘッダーがなければ作成
            EnsureGroupHeader(threadType);

            var entry = new ThreadEntry
            {
                ThreadId = threadId,
                DisplayName = displayName,
                Type = threadType
            };

            // リストアイテム
            var itemObj = new GameObject($"ThreadItem_{threadId}");
            itemObj.transform.SetParent(m_SidebarContent, false);

            var itemLayout = itemObj.AddComponent<LayoutElement>();
            itemLayout.preferredHeight = 38f;

            var itemBg = itemObj.AddComponent<Image>();
            itemBg.color = ItemColor;
            entry.ItemBg = itemBg;

            var itemBtn = itemObj.AddComponent<Button>();
            string capturedId = threadId;
            itemBtn.onClick.AddListener(() => OnSelectThread(capturedId));

            // 型アイコンラベル (左端)
            var typeObj = new GameObject("TypeIcon");
            typeObj.transform.SetParent(itemObj.transform, false);
            var typeRt = typeObj.AddComponent<RectTransform>();
            typeRt.anchorMin = new Vector2(0f, 0f);
            typeRt.anchorMax = new Vector2(0f, 1f);
            typeRt.pivot = new Vector2(0f, 0.5f);
            typeRt.anchoredPosition = new Vector2(6f, 0f);
            typeRt.sizeDelta = new Vector2(24f, 0f);

            var typeLabel = typeObj.AddComponent<TextMeshProUGUI>();
            typeLabel.fontSize = 11f;
            typeLabel.alignment = TextAlignmentOptions.Center;
            typeLabel.text = GetTypeIcon(threadType);
            typeLabel.color = GetTypeColor(threadType);
            typeLabel.fontStyle = FontStyles.Bold;
            AssignDefaultFont(typeLabel);
            entry.TypeLabel = typeLabel;

            // スレッド名ラベル
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(itemObj.transform, false);
            var nameRt = nameObj.AddComponent<RectTransform>();
            nameRt.anchorMin = Vector2.zero;
            nameRt.anchorMax = Vector2.one;
            nameRt.offsetMin = new Vector2(34f, 2f);
            nameRt.offsetMax = new Vector2(-36f, -2f);

            var nameLabel = nameObj.AddComponent<TextMeshProUGUI>();
            nameLabel.fontSize = 13f;
            nameLabel.alignment = TextAlignmentOptions.MidlineLeft;
            nameLabel.color = new Color(0.85f, 0.85f, 0.9f, 1f);
            nameLabel.text = displayName;
            nameLabel.enableWordWrapping = false;
            nameLabel.overflowMode = TextOverflowModes.Ellipsis;
            AssignDefaultFont(nameLabel);
            entry.NameLabel = nameLabel;

            // 未読バッジ
            var badgeObj = new GameObject("Badge");
            badgeObj.transform.SetParent(itemObj.transform, false);
            var badgeRt = badgeObj.AddComponent<RectTransform>();
            badgeRt.anchorMin = new Vector2(1f, 0.5f);
            badgeRt.anchorMax = new Vector2(1f, 0.5f);
            badgeRt.pivot = new Vector2(1f, 0.5f);
            badgeRt.anchoredPosition = new Vector2(-6f, 0f);
            badgeRt.sizeDelta = new Vector2(24f, 20f);

            var badgeBg = badgeObj.AddComponent<Image>();
            badgeBg.color = BadgeColor;

            var badgeLabelObj = new GameObject("Label");
            badgeLabelObj.transform.SetParent(badgeObj.transform, false);
            var badgeLabelRt = badgeLabelObj.AddComponent<RectTransform>();
            badgeLabelRt.anchorMin = Vector2.zero;
            badgeLabelRt.anchorMax = Vector2.one;
            badgeLabelRt.offsetMin = Vector2.zero;
            badgeLabelRt.offsetMax = Vector2.zero;

            var badgeLabel = badgeLabelObj.AddComponent<TextMeshProUGUI>();
            badgeLabel.fontSize = 10f;
            badgeLabel.alignment = TextAlignmentOptions.Center;
            badgeLabel.color = Color.white;
            badgeLabel.text = "";
            AssignDefaultFont(badgeLabel);

            entry.ListItemObj = itemObj;
            entry.BadgeLabel = badgeLabel;
            m_Threads.Add(entry);

            // グループヘッダーの直後に配置
            RepositionEntryAfterGroupHeader(entry);

            UpdateBadge(entry);
            UpdateHamburgerBadge();
        }

        private void EnsureMainEntry()
        {
            if (m_Threads.Count > 0 && m_Threads[0].ThreadId == null) return;

            var mainEntry = new ThreadEntry
            {
                ThreadId = null,
                DisplayName = "Main",
                Type = ThreadType.Annotation // ダミー
            };

            var itemObj = new GameObject("ThreadItem_Main");
            itemObj.transform.SetParent(m_SidebarContent, false);
            itemObj.transform.SetAsFirstSibling();

            var itemLayout = itemObj.AddComponent<LayoutElement>();
            itemLayout.preferredHeight = 42f;

            var itemBg = itemObj.AddComponent<Image>();
            itemBg.color = MainEntryColor;
            mainEntry.ItemBg = itemBg;

            var itemBtn = itemObj.AddComponent<Button>();
            itemBtn.onClick.AddListener(() => OnSelectThread(null));

            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(itemObj.transform, false);
            var nameRt = nameObj.AddComponent<RectTransform>();
            nameRt.anchorMin = Vector2.zero;
            nameRt.anchorMax = Vector2.one;
            nameRt.offsetMin = new Vector2(12f, 2f);
            nameRt.offsetMax = new Vector2(-12f, -2f);

            var nameLabel = nameObj.AddComponent<TextMeshProUGUI>();
            nameLabel.fontSize = 14f;
            nameLabel.alignment = TextAlignmentOptions.MidlineLeft;
            nameLabel.color = new Color(0.95f, 0.95f, 1f, 1f);
            nameLabel.text = "Main";
            nameLabel.fontStyle = FontStyles.Bold;
            AssignDefaultFont(nameLabel);
            mainEntry.NameLabel = nameLabel;

            mainEntry.ListItemObj = itemObj;
            mainEntry.BadgeLabel = null;
            m_Threads.Insert(0, mainEntry);
        }

        private void EnsureGroupHeader(ThreadType type)
        {
            if (m_GroupHeaders.ContainsKey(type)) return;

            var headerObj = new GameObject($"GroupHeader_{type}");
            headerObj.transform.SetParent(m_SidebarContent, false);

            var layout = headerObj.AddComponent<LayoutElement>();
            layout.preferredHeight = 26f;

            var bg = headerObj.AddComponent<Image>();
            bg.color = GroupHeaderColor;

            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(headerObj.transform, false);
            var labelRt = labelObj.AddComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = new Vector2(8f, 0f);
            labelRt.offsetMax = new Vector2(-8f, 0f);

            Color typeColor = GetTypeColor(type);
            var label = labelObj.AddComponent<TextMeshProUGUI>();
            label.fontSize = 10f;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.color = new Color(typeColor.r, typeColor.g, typeColor.b, 0.7f);
            label.text = $"{GetTypeIcon(type)} {GetTypeName(type)}";
            label.fontStyle = FontStyles.Bold;
            AssignDefaultFont(label);

            m_GroupHeaders[type] = headerObj;
        }

        private void RepositionEntryAfterGroupHeader(ThreadEntry entry)
        {
            if (!m_GroupHeaders.TryGetValue(entry.Type, out var header)) return;

            int headerIndex = header.transform.GetSiblingIndex();

            // グループ内の最後の位置を探す
            int targetIndex = headerIndex + 1;
            foreach (var other in m_Threads)
            {
                if (other == entry) continue;
                if (other.Type == entry.Type && other.ListItemObj != null)
                {
                    int otherIdx = other.ListItemObj.transform.GetSiblingIndex();
                    if (otherIdx >= targetIndex) targetIndex = otherIdx + 1;
                }
            }

            entry.ListItemObj.transform.SetSiblingIndex(targetIndex);
        }

        #endregion

        #region Sidebar Open / Close

        private void ToggleSidebar()
        {
            if (m_IsSidebarOpen)
                CloseSidebar();
            else
                OpenSidebar();
        }

        private void OpenSidebar()
        {
            EnsureMainEntry();
            UpdateAllHighlights();
            UpdateAllBadges();

            m_Overlay.SetActive(true);
            m_IsSidebarOpen = true;

            m_SidebarTween?.Kill();
            m_SidebarTween = m_SidebarRt.DOAnchorPosX(0f, SlideAnimDuration)
                .SetEase(Ease.OutCubic);
        }

        private void CloseSidebar()
        {
            m_IsSidebarOpen = false;

            m_SidebarTween?.Kill();
            m_SidebarTween = m_SidebarRt.DOAnchorPosX(-SidebarWidth, SlideAnimDuration)
                .SetEase(Ease.InCubic)
                .OnComplete(() => m_Overlay.SetActive(false));
        }

        #endregion

        #region Thread Selection

        private void OnSelectThread(string threadId)
        {
            if (m_ChatController == null) return;

            if (threadId == null)
            {
                m_ChatController.SwitchToThread(null);
            }
            else
            {
                var thread = m_ScenarioManager?.GetDeclaredThread(threadId);
                m_ChatController.SwitchToThread(threadId, thread?.ChatHistory);

                if (thread != null)
                {
                    thread.UnreadCount = 0;
                }
            }

            UpdateThreadHeaderBar(threadId);
            UpdateAllBadges();
            UpdateHamburgerBadge();
            CloseSidebar();

            if (m_NotificationThreadId == threadId)
            {
                HideNotificationBanner();
            }
        }

        #endregion

        #region Badge & Highlight Updates

        private void UpdateBadge(ThreadEntry entry)
        {
            if (entry.BadgeLabel == null) return;

            var thread = m_ScenarioManager?.GetDeclaredThread(entry.ThreadId);
            int unread = thread?.UnreadCount ?? 0;

            if (unread > 0)
            {
                entry.BadgeLabel.text = unread.ToString();
                entry.BadgeLabel.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                entry.BadgeLabel.transform.parent.gameObject.SetActive(false);
            }
        }

        private void UpdateAllBadges()
        {
            foreach (var entry in m_Threads)
            {
                UpdateBadge(entry);
            }
        }

        private void UpdateHamburgerBadge()
        {
            if (m_HamburgerBadge == null) return;

            int total = 0;
            foreach (var entry in m_Threads)
            {
                if (entry.ThreadId == null) continue; // Main は除外
                var thread = m_ScenarioManager?.GetDeclaredThread(entry.ThreadId);
                total += thread?.UnreadCount ?? 0;
            }

            var badgeObj = m_HamburgerBadge.transform.parent?.gameObject ?? m_HamburgerBadge.gameObject;
            if (total > 0)
            {
                m_HamburgerBadge.text = total > 99 ? "99+" : total.ToString();
                badgeObj.SetActive(true);
            }
            else
            {
                badgeObj.SetActive(false);
            }
        }

        private void UpdateAllHighlights()
        {
            string activeId = m_ChatController?.ActiveThreadId;
            foreach (var entry in m_Threads)
            {
                if (entry.ItemBg == null) continue;

                bool isActive = entry.ThreadId == activeId;
                if (entry.ThreadId == null)
                {
                    // Main エントリ
                    entry.ItemBg.color = isActive ? ActiveItemColor : MainEntryColor;
                }
                else
                {
                    entry.ItemBg.color = isActive ? ActiveItemColor : ItemColor;
                }

                // アクティブなスレッドの名前を明るく
                if (entry.NameLabel != null)
                {
                    entry.NameLabel.color = isActive
                        ? new Color(1f, 1f, 1f, 1f)
                        : new Color(0.85f, 0.85f, 0.9f, 1f);
                }
            }
        }

        private void UpdateThreadHeaderBar(string threadId)
        {
            if (m_ThreadHeaderBar == null) return;

            if (threadId == null)
            {
                m_ThreadHeaderBar.SetActive(false);
                return;
            }

            ThreadType type = ThreadType.Annotation;
            string displayName = threadId;
            foreach (var entry in m_Threads)
            {
                if (entry.ThreadId == threadId)
                {
                    type = entry.Type;
                    displayName = entry.DisplayName;
                    break;
                }
            }

            Color typeColor = GetTypeColor(type);
            m_ThreadHeaderBarBg.color = new Color(typeColor.r, typeColor.g, typeColor.b, 0.15f);
            m_ThreadHeaderBarLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(typeColor)}>{GetTypeIcon(type)}</color>  {displayName}";
            m_ThreadHeaderBarLabel.color = new Color(typeColor.r, typeColor.g, typeColor.b, 0.9f);
            m_ThreadHeaderBar.SetActive(true);
        }

        #endregion

        #region Notification Banner

        private void ShowNotificationBanner(ThreadEntry entry)
        {
            if (m_NotificationBanner == null) return;

            m_NotificationTween?.Kill();

            m_NotificationThreadId = entry.ThreadId;

            Color typeColor = GetTypeColor(entry.Type);
            string icon = GetTypeIcon(entry.Type);
            string colorHex = ColorUtility.ToHtmlStringRGB(typeColor);
            m_NotificationLabel.text = $"<color=#{colorHex}>{icon}</color> {entry.DisplayName} に新着";

            m_NotificationBg.color = new Color(
                NotificationBgColor.r + typeColor.r * 0.08f,
                NotificationBgColor.g + typeColor.g * 0.08f,
                NotificationBgColor.b + typeColor.b * 0.08f,
                NotificationBgColor.a);

            m_NotificationBanner.SetActive(true);
            m_NotificationCanvasGroup.alpha = 0f;

            m_NotificationTween = DOTween.Sequence()
                .Append(m_NotificationCanvasGroup.DOFade(1f, 0.25f))
                .AppendInterval(NotificationDuration)
                .Append(m_NotificationCanvasGroup.DOFade(0f, 0.4f))
                .OnComplete(() =>
                {
                    m_NotificationBanner.SetActive(false);
                    m_NotificationThreadId = null;
                });
        }

        private void HideNotificationBanner()
        {
            if (m_NotificationBanner == null || !m_NotificationBanner.activeSelf) return;

            m_NotificationTween?.Kill();
            m_NotificationCanvasGroup.alpha = 0f;
            m_NotificationBanner.SetActive(false);
            m_NotificationThreadId = null;
        }

        private void OnNotificationClicked()
        {
            if (m_NotificationThreadId == null) return;

            string threadId = m_NotificationThreadId;
            HideNotificationBanner();
            OnSelectThread(threadId);
        }

        #endregion

        #region Reset

        /// <summary>スレッド切替状態をリセット (シナリオ再開始時用)</summary>
        public void Reset()
        {
            foreach (var entry in m_Threads)
            {
                if (entry.ListItemObj != null)
                {
                    Destroy(entry.ListItemObj);
                }
            }
            m_Threads.Clear();

            foreach (var kv in m_GroupHeaders)
            {
                if (kv.Value != null) Destroy(kv.Value);
            }
            m_GroupHeaders.Clear();

            if (m_HamburgerButton != null)
            {
                m_HamburgerButton.SetActive(false);
            }

            CloseSidebar();

            if (m_ThreadHeaderBar != null)
            {
                m_ThreadHeaderBar.SetActive(false);
            }

            HideNotificationBanner();
            UpdateHamburgerBadge();
        }

        #endregion

        #region Static Helpers

        private static string GetTypeIcon(ThreadType type)
        {
            switch (type)
            {
                case ThreadType.Annotation: return "[A]";
                case ThreadType.Tracking:   return "[B]";
                case ThreadType.Scout:      return "[C]";
                case ThreadType.Branch:     return "[>]";
                default:                    return "[A]";
            }
        }

        private static string GetTypeName(ThreadType type)
        {
            switch (type)
            {
                case ThreadType.Annotation: return "Annotation";
                case ThreadType.Tracking:   return "Tracking";
                case ThreadType.Scout:      return "Scout";
                case ThreadType.Branch:     return "Branch";
                default:                    return "Other";
            }
        }

        private static Color GetTypeColor(ThreadType type)
        {
            switch (type)
            {
                case ThreadType.Annotation: return TypeColorAnnotation;
                case ThreadType.Tracking:   return TypeColorTracking;
                case ThreadType.Scout:      return TypeColorScout;
                case ThreadType.Branch:     return TypeColorBranch;
                default:                    return TypeColorAnnotation;
            }
        }

        private static void AssignDefaultFont(TMP_Text text)
        {
            if (text.font != null) return;
            TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            if (fonts.Length > 0) text.font = fonts[0];
        }

        #endregion
    }
}
