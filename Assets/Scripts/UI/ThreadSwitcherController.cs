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

        // スレッドエントリ管理
        private class ThreadEntry
        {
            public string ThreadId;
            public string DisplayName;
            public GameObject ListItemObj;
            public TextMeshProUGUI BadgeLabel;
        }
        private readonly List<ThreadEntry> m_Threads = new List<ThreadEntry>();

        private static readonly Color HeaderColor = new Color(0.2f, 0.25f, 0.35f, 0.95f);
        private static readonly Color ItemColor = new Color(0.15f, 0.18f, 0.25f, 0.95f);
        private static readonly Color ItemHoverColor = new Color(0.25f, 0.3f, 0.4f, 0.95f);
        private static readonly Color ActiveItemColor = new Color(0.3f, 0.35f, 0.5f, 0.95f);
        private static readonly Color BadgeColor = new Color(0.9f, 0.3f, 0.3f, 1f);

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
            var entry = new ThreadEntry
            {
                ThreadId = threadId,
                DisplayName = displayName
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

            // スレッド名ラベル
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(itemObj.transform, false);
            var nameRt = nameObj.AddComponent<RectTransform>();
            nameRt.anchorMin = Vector2.zero;
            nameRt.anchorMax = Vector2.one;
            nameRt.offsetMin = new Vector2(10f, 2f);
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
                        m_HeaderLabel.text = entry.DisplayName + " \u25BC";
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
        }

        private static void AssignDefaultFont(TMP_Text text)
        {
            if (text.font != null) return;
            TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            if (fonts.Length > 0) text.font = fonts[0];
        }
    }
}
