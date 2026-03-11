#if YARN_SPINNER
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// Production dashboard -- channel list, HalluciCoin display, chapter navigation.
    /// Overlay panel in ContentAuthoring scene. Default view for players.
    /// </summary>
    public class DashboardController : MonoBehaviour
    {
        [SerializeField] private bool m_ShowOnStart = true;

        private ScenarioManager m_ScenarioManager;
        private ChatController m_ChatController;
        private ContradictionManager m_ContradictionManager;

        private GameObject m_DashboardPanel;
        private GameObject m_BackButton;
        private Transform m_ChannelListContent;
        private TextMeshProUGUI m_CoinDisplay;
        private bool m_IsShowing;

        private ChannelData[] m_Channels;

        #region Unity Lifecycle
        private void Start()
        {
            m_ScenarioManager = FindFirstObjectByType<ScenarioManager>();
            m_ChatController = FindFirstObjectByType<ChatController>();
            m_ContradictionManager = FindFirstObjectByType<ContradictionManager>();

            if (m_ScenarioManager == null)
                Debug.LogWarning("DashboardController: ScenarioManager not found in scene.");
            if (m_ChatController == null)
                Debug.LogWarning("DashboardController: ChatController not found in scene.");

            m_Channels = LoadChannelData();

            if (m_ShowOnStart)
            {
                Show();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !m_IsShowing)
            {
                ReturnToDashboard();
            }
        }
        #endregion

        #region Public Methods
        public void Show()
        {
            if (m_DashboardPanel == null)
            {
                BuildDashboardUI();
            }

            if (m_DashboardPanel == null) return;

            RefreshChannelList();
            RefreshCoinDisplay();
            m_DashboardPanel.SetActive(true);

            if (m_BackButton != null)
            {
                m_BackButton.SetActive(false);
            }

            m_IsShowing = true;
        }

        public void Hide()
        {
            if (m_DashboardPanel != null)
            {
                m_DashboardPanel.SetActive(false);
            }
            m_IsShowing = false;
        }

        public bool IsShowing => m_IsShowing;

        public void ReturnToDashboard()
        {
            if (m_ScenarioManager != null)
            {
                m_ScenarioManager.StopScenario();
            }

            if (m_ChatController != null)
            {
                m_ChatController.ClearMessages();
            }

            Show();
        }
        #endregion

        #region Data
        private ChannelData[] LoadChannelData()
        {
            ChannelData[] all = Resources.LoadAll<ChannelData>("Channels");
            if (all == null || all.Length == 0)
            {
                Debug.LogWarning("DashboardController: No ChannelData found in Resources/Channels/");
                return new ChannelData[0];
            }
            return all.OrderBy(c => c.ChapterNumber).ToArray();
        }

        private ChannelStatus GetChannelStatus(ChannelData channel)
        {
            SaveData saveData = SaveManager.Instance != null ? SaveManager.Instance.CurrentSaveData : null;

            if (!string.IsNullOrEmpty(channel.RequiredCompletedChannelID))
            {
                if (saveData == null || !saveData.CompletedChannelIDs.Contains(channel.RequiredCompletedChannelID))
                {
                    return ChannelStatus.Locked;
                }
            }

            if (saveData != null)
            {
                if (saveData.CompletedChannelIDs.Contains(channel.ChannelID))
                    return ChannelStatus.Completed;
                if (!string.IsNullOrEmpty(saveData.CurrentNodeName)
                    && YarnNodeHelper.BelongsToChapter(saveData.CurrentNodeName, channel.ChapterNumber))
                    return ChannelStatus.InProgress;
            }

            return ChannelStatus.Available;
        }
        #endregion

        #region UI Construction
        private void BuildDashboardUI()
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("DashboardController: No Canvas found in scene.");
                return;
            }

            // --- Dashboard Panel (full-screen overlay) ---
            m_DashboardPanel = new GameObject("DashboardPanel", typeof(RectTransform), typeof(Image));
            AssignUILayer(m_DashboardPanel);
            m_DashboardPanel.transform.SetParent(canvas.transform, false);

            RectTransform panelRect = m_DashboardPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelBg = m_DashboardPanel.GetComponent<Image>();
            panelBg.color = new Color(0.06f, 0.06f, 0.09f, 0.98f);
            panelBg.raycastTarget = true;

            // --- Title ---
            GameObject titleObj = new GameObject("Title", typeof(RectTransform));
            AssignUILayer(titleObj);
            titleObj.transform.SetParent(m_DashboardPanel.transform, false);

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.offsetMin = new Vector2(20f, -72f);
            titleRect.offsetMax = new Vector2(-20f, -16f);

            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "HALLUCINATION SIMULATOR";
            titleText.fontSize = 28f;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(0.7f, 0.7f, 0.75f);
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.raycastTarget = false;
            AssignDefaultFont(titleText);

            // --- Subtitle ---
            GameObject subtitleObj = new GameObject("Subtitle", typeof(RectTransform));
            AssignUILayer(subtitleObj);
            subtitleObj.transform.SetParent(m_DashboardPanel.transform, false);

            RectTransform subtitleRect = subtitleObj.GetComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0f, 1f);
            subtitleRect.anchorMax = new Vector2(1f, 1f);
            subtitleRect.pivot = new Vector2(0.5f, 1f);
            subtitleRect.offsetMin = new Vector2(20f, -100f);
            subtitleRect.offsetMax = new Vector2(-20f, -72f);

            TextMeshProUGUI subtitleText = subtitleObj.AddComponent<TextMeshProUGUI>();
            subtitleText.text = "select channel_";
            subtitleText.fontSize = 16f;
            subtitleText.fontStyle = FontStyles.Italic;
            subtitleText.color = new Color(0.4f, 0.4f, 0.45f);
            subtitleText.alignment = TextAlignmentOptions.Center;
            subtitleText.raycastTarget = false;
            AssignDefaultFont(subtitleText);

            // --- HalluciCoin display (top-right) ---
            GameObject coinObj = new GameObject("CoinDisplay", typeof(RectTransform));
            AssignUILayer(coinObj);
            coinObj.transform.SetParent(m_DashboardPanel.transform, false);

            RectTransform coinRect = coinObj.GetComponent<RectTransform>();
            coinRect.anchorMin = new Vector2(1f, 1f);
            coinRect.anchorMax = new Vector2(1f, 1f);
            coinRect.pivot = new Vector2(1f, 1f);
            coinRect.anchoredPosition = new Vector2(-20f, -20f);
            coinRect.sizeDelta = new Vector2(140f, 30f);

            m_CoinDisplay = coinObj.AddComponent<TextMeshProUGUI>();
            m_CoinDisplay.text = "HC: 0";
            m_CoinDisplay.fontSize = 20f;
            m_CoinDisplay.color = new Color(0.8f, 0.75f, 0.4f);
            m_CoinDisplay.alignment = TextAlignmentOptions.MidlineRight;
            m_CoinDisplay.raycastTarget = false;
            AssignDefaultFont(m_CoinDisplay);

            // --- ScrollView ---
            GameObject scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            AssignUILayer(scrollObj);
            scrollObj.transform.SetParent(m_DashboardPanel.transform, false);

            RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0f, 0f);
            scrollRect.anchorMax = new Vector2(1f, 1f);
            scrollRect.offsetMin = new Vector2(20f, 20f);
            scrollRect.offsetMax = new Vector2(-20f, -110f);

            Image scrollBg = scrollObj.GetComponent<Image>();
            scrollBg.color = new Color(0.08f, 0.08f, 0.1f, 0.4f);

            // --- Viewport ---
            GameObject viewportObj = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
            AssignUILayer(viewportObj);
            viewportObj.transform.SetParent(scrollObj.transform, false);

            RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            // --- Content ---
            GameObject contentObj = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            AssignUILayer(contentObj);
            contentObj.transform.SetParent(viewportObj.transform, false);

            RectTransform contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;

            VerticalLayoutGroup vlg = contentObj.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 12f;
            vlg.padding = new RectOffset(16, 16, 12, 12);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;

            ContentSizeFitter fitter = contentObj.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            m_ChannelListContent = contentObj.transform;

            ScrollRect scroll = scrollObj.GetComponent<ScrollRect>();
            scroll.content = contentRect;
            scroll.viewport = viewportRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.scrollSensitivity = 30f;

            // --- Back to Dashboard button (floating, top-left, hidden initially) ---
            m_BackButton = new GameObject("BackToDashboard", typeof(RectTransform), typeof(Image), typeof(Button));
            AssignUILayer(m_BackButton);
            m_BackButton.transform.SetParent(canvas.transform, false);

            RectTransform backRect = m_BackButton.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0f, 1f);
            backRect.anchorMax = new Vector2(0f, 1f);
            backRect.pivot = new Vector2(0f, 1f);
            backRect.anchoredPosition = new Vector2(12f, -12f);
            backRect.sizeDelta = new Vector2(160f, 40f);

            Image backBg = m_BackButton.GetComponent<Image>();
            backBg.color = new Color(0.15f, 0.15f, 0.2f, 0.85f);
            backBg.raycastTarget = true;

            Button backBtn = m_BackButton.GetComponent<Button>();
            backBtn.transition = Selectable.Transition.ColorTint;
            backBtn.targetGraphic = backBg;
            backBtn.onClick.AddListener(ReturnToDashboard);

            GameObject backTextObj = new GameObject("Text", typeof(RectTransform));
            AssignUILayer(backTextObj);
            backTextObj.transform.SetParent(m_BackButton.transform, false);

            RectTransform backTextRect = backTextObj.GetComponent<RectTransform>();
            backTextRect.anchorMin = Vector2.zero;
            backTextRect.anchorMax = Vector2.one;
            backTextRect.offsetMin = new Vector2(8f, 4f);
            backTextRect.offsetMax = new Vector2(-8f, -4f);

            TextMeshProUGUI backLabel = backTextObj.AddComponent<TextMeshProUGUI>();
            backLabel.text = "< dashboard";
            backLabel.fontSize = 18f;
            backLabel.color = new Color(0.6f, 0.6f, 0.65f);
            backLabel.alignment = TextAlignmentOptions.Center;
            backLabel.raycastTarget = false;
            AssignDefaultFont(backLabel);

            m_BackButton.SetActive(false);
        }

        private void RefreshChannelList()
        {
            if (m_ChannelListContent == null) return;

            for (int i = m_ChannelListContent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(m_ChannelListContent.GetChild(i).gameObject);
            }

            if (m_Channels.Length == 0)
            {
                CreateInfoLabel("No channels found.");
                return;
            }

            foreach (ChannelData channel in m_Channels)
            {
                ChannelStatus status = GetChannelStatus(channel);
                CreateChannelCard(channel, status);
            }
        }

        private void RefreshCoinDisplay()
        {
            if (m_CoinDisplay == null) return;

            int coin = 0;
            if (m_ContradictionManager != null)
            {
                coin = m_ContradictionManager.HalluciCoin;
            }
            else
            {
                SaveData saveData = SaveManager.Instance != null ? SaveManager.Instance.CurrentSaveData : null;
                if (saveData != null)
                {
                    coin = saveData.HalluciCoin;
                }
            }

            m_CoinDisplay.text = $"HC: {coin}";
        }

        private void CreateChannelCard(ChannelData channel, ChannelStatus status)
        {
            // Card container
            GameObject cardObj = new GameObject($"Channel_{channel.ChannelID}",
                typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            AssignUILayer(cardObj);
            cardObj.transform.SetParent(m_ChannelListContent, false);

            LayoutElement layout = cardObj.GetComponent<LayoutElement>();
            layout.minHeight = 100f;
            layout.preferredHeight = 110f;

            Image cardBg = cardObj.GetComponent<Image>();
            cardBg.color = GetCardColor(status);
            cardBg.raycastTarget = true;

            Button cardBtn = cardObj.GetComponent<Button>();
            cardBtn.transition = Selectable.Transition.ColorTint;
            cardBtn.targetGraphic = cardBg;

            if (status == ChannelStatus.Locked)
            {
                cardBtn.interactable = false;
            }
            else
            {
                ChannelData capturedChannel = channel;
                cardBtn.onClick.AddListener(() => OnChannelClicked(capturedChannel));
            }

            // Title text
            GameObject titleObj = new GameObject("Title", typeof(RectTransform));
            AssignUILayer(titleObj);
            titleObj.transform.SetParent(cardObj.transform, false);

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0.5f);
            titleRect.anchorMax = new Vector2(0.75f, 1f);
            titleRect.offsetMin = new Vector2(16f, 4f);
            titleRect.offsetMax = new Vector2(-8f, -12f);

            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = channel.DisplayName;
            titleText.fontSize = 22f;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = status == ChannelStatus.Locked
                ? new Color(0.35f, 0.35f, 0.4f)
                : new Color(0.85f, 0.85f, 0.9f);
            titleText.alignment = TextAlignmentOptions.MidlineLeft;
            titleText.raycastTarget = false;
            AssignDefaultFont(titleText);

            // Description text
            GameObject descObj = new GameObject("Description", typeof(RectTransform));
            AssignUILayer(descObj);
            descObj.transform.SetParent(cardObj.transform, false);

            RectTransform descRect = descObj.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0f, 0f);
            descRect.anchorMax = new Vector2(0.75f, 0.5f);
            descRect.offsetMin = new Vector2(16f, 8f);
            descRect.offsetMax = new Vector2(-8f, -4f);

            TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
            descText.text = channel.Description;
            descText.fontSize = 15f;
            descText.color = status == ChannelStatus.Locked
                ? new Color(0.3f, 0.3f, 0.33f)
                : new Color(0.5f, 0.5f, 0.55f);
            descText.alignment = TextAlignmentOptions.TopLeft;
            descText.raycastTarget = false;
            AssignDefaultFont(descText);

            // Status label (top-right of card)
            GameObject statusObj = new GameObject("Status", typeof(RectTransform));
            AssignUILayer(statusObj);
            statusObj.transform.SetParent(cardObj.transform, false);

            RectTransform statusRect = statusObj.GetComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0.75f, 0.5f);
            statusRect.anchorMax = new Vector2(1f, 1f);
            statusRect.offsetMin = new Vector2(4f, 4f);
            statusRect.offsetMax = new Vector2(-12f, -12f);

            TextMeshProUGUI statusText = statusObj.AddComponent<TextMeshProUGUI>();
            statusText.text = GetStatusLabel(status);
            statusText.fontSize = 14f;
            statusText.color = GetStatusColor(status);
            statusText.alignment = TextAlignmentOptions.MidlineRight;
            statusText.raycastTarget = false;
            AssignDefaultFont(statusText);
        }

        private void OnChannelClicked(ChannelData channel)
        {
            Hide();

            if (m_BackButton != null)
            {
                m_BackButton.SetActive(true);
            }

            // StopScenario を ClearMessages より先に実行
            // （実行中ダイアログの非同期タスクが ClearMessages 後にバブルを追加するのを防止）
            if (m_ScenarioManager != null)
            {
                m_ScenarioManager.StopScenario();
            }

            // 矛盾指摘モードのリセット + チャプター/ヒントポリシー更新
            if (m_ContradictionManager != null)
            {
                m_ContradictionManager.ClearSelection();
                m_ContradictionManager.SetCurrentChannel(channel);
            }

            if (m_ChatController != null)
            {
                m_ChatController.ClearMessages();
            }

            if (m_ScenarioManager != null)
            {
                m_ScenarioManager.StartScenario(channel.StartNodeName);
            }
        }

        private void CreateInfoLabel(string text)
        {
            GameObject labelObj = new GameObject("InfoLabel", typeof(RectTransform), typeof(LayoutElement));
            AssignUILayer(labelObj);
            labelObj.transform.SetParent(m_ChannelListContent, false);

            LayoutElement layout = labelObj.GetComponent<LayoutElement>();
            layout.minHeight = 32f;
            layout.flexibleWidth = 1f;

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = 18f;
            label.fontStyle = FontStyles.Italic;
            label.color = new Color(0.45f, 0.45f, 0.5f);
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            AssignDefaultFont(label);
        }
        #endregion

        #region Helpers
        private static Color GetCardColor(ChannelStatus status)
        {
            return status switch
            {
                ChannelStatus.Locked     => new Color(0.12f, 0.12f, 0.15f, 0.6f),
                ChannelStatus.Available  => new Color(0.15f, 0.15f, 0.2f, 0.9f),
                ChannelStatus.InProgress => new Color(0.15f, 0.2f, 0.3f, 0.9f),
                ChannelStatus.Completed  => new Color(0.12f, 0.18f, 0.12f, 0.9f),
                _ => new Color(0.15f, 0.15f, 0.2f, 0.9f),
            };
        }

        private static string GetStatusLabel(ChannelStatus status)
        {
            return status switch
            {
                ChannelStatus.Locked     => "[LOCKED]",
                ChannelStatus.Available  => "[AVAILABLE]",
                ChannelStatus.InProgress => "[IN PROGRESS]",
                ChannelStatus.Completed  => "[COMPLETED]",
                _ => "",
            };
        }

        private static Color GetStatusColor(ChannelStatus status)
        {
            return status switch
            {
                ChannelStatus.Locked     => new Color(0.35f, 0.35f, 0.4f),
                ChannelStatus.Available  => new Color(0.5f, 0.6f, 0.8f),
                ChannelStatus.InProgress => new Color(0.6f, 0.7f, 0.4f),
                ChannelStatus.Completed  => new Color(0.4f, 0.6f, 0.4f),
                _ => Color.gray,
            };
        }

        private static void AssignDefaultFont(TextMeshProUGUI tmp)
        {
            if (TMP_Settings.defaultFontAsset != null)
            {
                tmp.font = TMP_Settings.defaultFontAsset;
            }
        }

        private void AssignUILayer(GameObject go)
        {
            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer >= 0)
            {
                go.layer = uiLayer;
            }
        }
        #endregion
    }
}
#endif
