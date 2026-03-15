using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 複数サブスレッドの切替UIを管理するコントローラ。
    /// 右上のドロップダウンボタンで Main + 全サブスレッドを切替可能。
    /// 各スレッドの未読バッジを表示する。
    /// </summary>
    public class ThreadSwitcherController : MonoBehaviour
    {
        [SerializeField] private ChatController m_ChatController;

        private ScenarioManager m_ScenarioManager;

        // UI要素
        private GameObject m_HeaderButton;
        private TextMeshProUGUI m_HeaderLabel;
        private GameObject m_DropdownPanel;
        private Transform m_DropdownContent;
        private bool m_IsDropdownOpen;

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
            public TextMeshProUGUI BadgeLabel;
            public TextMeshProUGUI TypeLabel;
        }
        private readonly List<ThreadEntry> m_Threads = new List<ThreadEntry>();

        private static readonly Color HeaderColor = new Color(0.2f, 0.25f, 0.35f, 0.95f);
        private static readonly Color ItemColor = new Color(0.15f, 0.18f, 0.25f, 0.95f);
        private static readonly Color ItemHoverColor = new Color(0.25f, 0.3f, 0.4f, 0.95f);
        private static readonly Color ActiveItemColor = new Color(0.3f, 0.35f, 0.5f, 0.95f);
        private static readonly Color BadgeColor = new Color(0.9f, 0.3f, 0.3f, 1f);

        // ThreadType 別色
        private static readonly Color TypeColorAnnotation = new Color(0.29f, 0.56f, 0.85f, 1f); // #4A90D9
        private static readonly Color TypeColorTracking = new Color(0.30f, 0.69f, 0.31f, 1f);   // #4CAF50
        private static readonly Color TypeColorScout = new Color(1f, 0.60f, 0f, 1f);             // #FF9800
        private static readonly Color TypeColorBranch = new Color(0.61f, 0.15f, 0.69f, 1f);      // #9C27B0

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
            m_HeaderButton.SetActive(false);
        }

        private void OnDestroy()
        {
            if (m_ScenarioManager != null)
            {
                m_ScenarioManager.OnThreadDeclared -= OnThreadDeclared;
                m_ScenarioManager.OnThreadMessageAdded -= OnThreadMessageAdded;
            }
        }

        private void CreateUI()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("ThreadSwitcherController: Canvas not found.");
                return;
            }

            // ヘッダーボタン (右上)
            m_HeaderButton = new GameObject("ThreadSwitchHeader");
            m_HeaderButton.transform.SetParent(canvas.transform, false);
            var headerRt = m_HeaderButton.AddComponent<RectTransform>();
            headerRt.anchorMin = new Vector2(1f, 1f);
            headerRt.anchorMax = new Vector2(1f, 1f);
            headerRt.pivot = new Vector2(1f, 1f);
            headerRt.anchoredPosition = new Vector2(-12f, -80f);
            headerRt.sizeDelta = new Vector2(180f, 36f);

            var headerBg = m_HeaderButton.AddComponent<Image>();
            headerBg.color = HeaderColor;

            var headerBtn = m_HeaderButton.AddComponent<Button>();
            headerBtn.onClick.AddListener(ToggleDropdown);

            // ヘッダーラベル
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(m_HeaderButton.transform, false);
            var labelRt = labelObj.AddComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = new Vector2(10f, 2f);
            labelRt.offsetMax = new Vector2(-10f, -2f);

            m_HeaderLabel = labelObj.AddComponent<TextMeshProUGUI>();
            m_HeaderLabel.fontSize = 13f;
            m_HeaderLabel.alignment = TextAlignmentOptions.MidlineLeft;
            m_HeaderLabel.color = Color.white;
            m_HeaderLabel.text = "Main";
            AssignDefaultFont(m_HeaderLabel);

            // ドロップダウンパネル (ヘッダーの直下)
            m_DropdownPanel = new GameObject("ThreadDropdown");
            m_DropdownPanel.transform.SetParent(canvas.transform, false);
            var dropRt = m_DropdownPanel.AddComponent<RectTransform>();
            dropRt.anchorMin = new Vector2(1f, 1f);
            dropRt.anchorMax = new Vector2(1f, 1f);
            dropRt.pivot = new Vector2(1f, 1f);
            dropRt.anchoredPosition = new Vector2(-12f, -118f); // ヘッダーの下
            dropRt.sizeDelta = new Vector2(180f, 0f); // ContentSizeFitter で自動調整

            var dropBg = m_DropdownPanel.AddComponent<Image>();
            dropBg.color = new Color(0.12f, 0.14f, 0.2f, 0.98f);

            var vlg = m_DropdownPanel.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(0, 0, 2, 2);
            vlg.spacing = 1f;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;

            var csf = m_DropdownPanel.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            m_DropdownContent = m_DropdownPanel.transform;
            m_DropdownPanel.SetActive(false);
            m_IsDropdownOpen = false;

            // スレッド型ヘッダーバー (画面上部の横長色帯)
            m_ThreadHeaderBar = new GameObject("ThreadHeaderBar");
            m_ThreadHeaderBar.transform.SetParent(canvas.transform, false);
            var barRt = m_ThreadHeaderBar.AddComponent<RectTransform>();
            barRt.anchorMin = new Vector2(0f, 1f);
            barRt.anchorMax = new Vector2(1f, 1f);
            barRt.pivot = new Vector2(0.5f, 1f);
            barRt.anchoredPosition = new Vector2(0f, -70f); // ステータスバー下あたり
            barRt.sizeDelta = new Vector2(0f, 28f);

            m_ThreadHeaderBarBg = m_ThreadHeaderBar.AddComponent<Image>();
            m_ThreadHeaderBarBg.color = Color.clear;

            var barLabelObj = new GameObject("BarLabel");
            barLabelObj.transform.SetParent(m_ThreadHeaderBar.transform, false);
            var barLabelRt = barLabelObj.AddComponent<RectTransform>();
            barLabelRt.anchorMin = Vector2.zero;
            barLabelRt.anchorMax = Vector2.one;
            barLabelRt.offsetMin = new Vector2(12f, 0f);
            barLabelRt.offsetMax = new Vector2(-12f, 0f);

            m_ThreadHeaderBarLabel = barLabelObj.AddComponent<TextMeshProUGUI>();
            m_ThreadHeaderBarLabel.fontSize = 12f;
            m_ThreadHeaderBarLabel.alignment = TextAlignmentOptions.MidlineLeft;
            m_ThreadHeaderBarLabel.color = Color.white;
            m_ThreadHeaderBarLabel.text = "";
            AssignDefaultFont(m_ThreadHeaderBarLabel);

            m_ThreadHeaderBar.SetActive(false);
        }

        private void OnThreadDeclared(string threadId, string displayName)
        {
            // 重複チェック
            foreach (var entry in m_Threads)
            {
                if (entry.ThreadId == threadId) return;
            }

            // 初回スレッド宣言時にヘッダーを表示
            if (!m_HeaderButton.activeSelf)
            {
                m_HeaderButton.SetActive(true);
            }

            AddThreadEntry(threadId, displayName);
        }

        private void OnThreadMessageAdded(string threadId)
        {
            // ドロップダウン内の該当エントリのバッジを更新
            foreach (var entry in m_Threads)
            {
                if (entry.ThreadId == threadId)
                {
                    UpdateBadge(entry);
                    break;
                }
            }
        }

        private void AddThreadEntry(string threadId, string displayName)
        {
            // ScenarioManager から ThreadType を取得
            var threadData = m_ScenarioManager?.GetDeclaredThread(threadId);
            ThreadType threadType = threadData?.Type ?? ThreadType.Annotation;

            var entry = new ThreadEntry
            {
                ThreadId = threadId,
                DisplayName = displayName,
                Type = threadType
            };

            // リストアイテム
            var itemObj = new GameObject($"ThreadItem_{threadId}");
            itemObj.transform.SetParent(m_DropdownContent, false);

            var itemLayout = itemObj.AddComponent<LayoutElement>();
            itemLayout.preferredHeight = 32f;

            var itemBg = itemObj.AddComponent<Image>();
            itemBg.color = ItemColor;

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
            typeRt.anchoredPosition = new Vector2(4f, 0f);
            typeRt.sizeDelta = new Vector2(22f, 0f);

            var typeLabel = typeObj.AddComponent<TextMeshProUGUI>();
            typeLabel.fontSize = 10f;
            typeLabel.alignment = TextAlignmentOptions.Center;
            typeLabel.text = GetTypeIcon(threadType);
            typeLabel.color = GetTypeColor(threadType);
            typeLabel.fontStyle = FontStyles.Bold;
            AssignDefaultFont(typeLabel);
            entry.TypeLabel = typeLabel;

            // スレッド名ラベル (型アイコンの右)
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(itemObj.transform, false);
            var nameRt = nameObj.AddComponent<RectTransform>();
            nameRt.anchorMin = Vector2.zero;
            nameRt.anchorMax = Vector2.one;
            nameRt.offsetMin = new Vector2(28f, 2f); // 型アイコン分を右にずらす
            nameRt.offsetMax = new Vector2(-36f, -2f); // 右側にバッジ分のスペース

            var nameLabel = nameObj.AddComponent<TextMeshProUGUI>();
            nameLabel.fontSize = 12f;
            nameLabel.alignment = TextAlignmentOptions.MidlineLeft;
            nameLabel.color = new Color(0.85f, 0.85f, 0.9f, 1f);
            nameLabel.text = displayName;
            nameLabel.enableWordWrapping = false;
            nameLabel.overflowMode = TextOverflowModes.Ellipsis;
            AssignDefaultFont(nameLabel);

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

            var badgeLabel = badgeObj.AddComponent<TextMeshProUGUI>();
            badgeLabel.fontSize = 10f;
            badgeLabel.alignment = TextAlignmentOptions.Center;
            badgeLabel.color = Color.white;
            badgeLabel.text = "";
            AssignDefaultFont(badgeLabel);

            entry.ListItemObj = itemObj;
            entry.BadgeLabel = badgeLabel;
            m_Threads.Add(entry);

            UpdateBadge(entry);
        }

        private void OnSelectThread(string threadId)
        {
            if (m_ChatController == null) return;

            // "Main" 選択 (threadId == null)
            if (threadId == null)
            {
                m_ChatController.SwitchToThread(null);
            }
            else
            {
                var thread = m_ScenarioManager?.GetDeclaredThread(threadId);
                m_ChatController.SwitchToThread(threadId, thread?.ChatHistory);

                // 未読リセット
                if (thread != null)
                {
                    thread.UnreadCount = 0;
                }
            }

            UpdateHeaderLabel();
            UpdateThreadHeaderBar(threadId);
            UpdateAllBadges();
            CloseDropdown();
        }

        private void ToggleDropdown()
        {
            if (m_IsDropdownOpen)
            {
                CloseDropdown();
            }
            else
            {
                OpenDropdown();
            }
        }

        private void OpenDropdown()
        {
            // ドロップダウンを開く前に Main エントリを先頭に挿入（なければ）
            EnsureMainEntry();
            UpdateAllHighlights();
            m_DropdownPanel.SetActive(true);
            m_IsDropdownOpen = true;
        }

        private void CloseDropdown()
        {
            m_DropdownPanel.SetActive(false);
            m_IsDropdownOpen = false;
        }

        private void EnsureMainEntry()
        {
            // "Main" エントリが先頭にあるか確認
            if (m_Threads.Count > 0 && m_Threads[0].ThreadId == null) return;

            // Main エントリを作成して先頭に挿入
            var mainEntry = new ThreadEntry
            {
                ThreadId = null,
                DisplayName = "Main"
            };

            var itemObj = new GameObject("ThreadItem_Main");
            itemObj.transform.SetParent(m_DropdownContent, false);
            itemObj.transform.SetAsFirstSibling();

            var itemLayout = itemObj.AddComponent<LayoutElement>();
            itemLayout.preferredHeight = 32f;

            var itemBg = itemObj.AddComponent<Image>();
            itemBg.color = ItemColor;

            var itemBtn = itemObj.AddComponent<Button>();
            itemBtn.onClick.AddListener(() => OnSelectThread(null));

            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(itemObj.transform, false);
            var nameRt = nameObj.AddComponent<RectTransform>();
            nameRt.anchorMin = Vector2.zero;
            nameRt.anchorMax = Vector2.one;
            nameRt.offsetMin = new Vector2(10f, 2f);
            nameRt.offsetMax = new Vector2(-10f, -2f);

            var nameLabel = nameObj.AddComponent<TextMeshProUGUI>();
            nameLabel.fontSize = 12f;
            nameLabel.alignment = TextAlignmentOptions.MidlineLeft;
            nameLabel.color = new Color(0.95f, 0.95f, 1f, 1f);
            nameLabel.text = "Main";
            AssignDefaultFont(nameLabel);

            mainEntry.ListItemObj = itemObj;
            mainEntry.BadgeLabel = null; // Main には未読バッジなし
            m_Threads.Insert(0, mainEntry);
        }

        private void UpdateHeaderLabel()
        {
            if (m_HeaderLabel == null) return;

            string activeId = m_ChatController?.ActiveThreadId;
            if (activeId == null)
            {
                m_HeaderLabel.text = "Main \u25BC";
            }
            else
            {
                foreach (var entry in m_Threads)
                {
                    if (entry.ThreadId == activeId)
                    {
                        string icon = GetTypeIcon(entry.Type);
                        m_HeaderLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(GetTypeColor(entry.Type))}>{icon}</color> {entry.DisplayName} \u25BC";
                        return;
                    }
                }
                m_HeaderLabel.text = activeId + " \u25BC";
            }
        }

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

        private void UpdateAllHighlights()
        {
            string activeId = m_ChatController?.ActiveThreadId;
            foreach (var entry in m_Threads)
            {
                if (entry.ListItemObj == null) continue;
                var bg = entry.ListItemObj.GetComponent<Image>();
                if (bg != null)
                {
                    bool isActive = entry.ThreadId == activeId;
                    bg.color = isActive ? ActiveItemColor : ItemColor;
                }
            }
        }

        private void UpdateThreadHeaderBar(string threadId)
        {
            if (m_ThreadHeaderBar == null) return;

            if (threadId == null)
            {
                // Mainスレッド → ヘッダーバー非表示
                m_ThreadHeaderBar.SetActive(false);
                return;
            }

            // サブスレッドの型情報を取得
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

        /// <summary>スレッド切替状態をリセット（シナリオ再開始時用）</summary>
        public void Reset()
        {
            // ドロップダウン内のエントリを破棄
            foreach (var entry in m_Threads)
            {
                if (entry.ListItemObj != null)
                {
                    Destroy(entry.ListItemObj);
                }
            }
            m_Threads.Clear();

            if (m_HeaderButton != null)
            {
                m_HeaderButton.SetActive(false);
            }
            CloseDropdown();

            if (m_HeaderLabel != null)
            {
                m_HeaderLabel.text = "Main";
            }

            if (m_ThreadHeaderBar != null)
            {
                m_ThreadHeaderBar.SetActive(false);
            }
        }

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
    }
}
