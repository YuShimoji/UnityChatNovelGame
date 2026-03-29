#if YARN_SPINNER
using System.Linq;
using DG.Tweening;
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

        private enum DashboardTab { Channels, Inventory }

        private GameObject m_DashboardPanel;
        private GameObject m_BackButton;
        private Transform m_ChannelListContent;
        private TextMeshProUGUI m_CoinDisplay;
        private TextMeshProUGUI m_SubtitleText;
        private bool m_IsShowing;
        private int m_LastCoinValue = -1;
        private Tween m_CoinPulseTween;

        private ChannelData[] m_Channels;

        // Tab switching
        private DashboardTab m_CurrentTab = DashboardTab.Channels;
        private GameObject m_TabBar;
        private TextMeshProUGUI m_ChannelTabLabel;
        private TextMeshProUGUI m_InventoryTabLabel;
        private GameObject m_ChannelContainer;
        private InventoryTabController m_InventoryTab;
        private GameObject m_CloseOverlayButton;
        private bool m_IsInventoryOverlayMode;
        private ProgressSummaryUI m_ProgressSummary;
        private bool m_RestoreBackButtonAfterOverlay;
        private DashboardTab m_TabBeforeOverlay = DashboardTab.Channels;

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

        private void OnDestroy()
        {
            m_CoinPulseTween?.Kill();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
                return;

            if (m_IsShowing)
            {
                if (m_IsInventoryOverlayMode)
                {
                    HideInventoryOverlay();
                }

                return;
            }

            ReturnToDashboard();
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

            SetOverlayMode(false);
            RefreshCoinDisplay();
            RefreshProgressSummary();

            if (m_CurrentTab == DashboardTab.Channels)
            {
                RefreshChannelList();
            }
            else if (m_InventoryTab != null)
            {
                m_InventoryTab.Refresh();
            }

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

            if (m_CloseOverlayButton != null)
            {
                m_CloseOverlayButton.SetActive(false);
            }

            m_IsInventoryOverlayMode = false;
            m_IsShowing = false;
        }

        public bool IsShowing => m_IsShowing;

        public void ShowChannels()
        {
            Show();
            SwitchTab(DashboardTab.Channels);
        }

        public void ShowInventory()
        {
            Show();
            SwitchTab(DashboardTab.Inventory);
        }

        public void ShowInventoryOverlay()
        {
            if (m_DashboardPanel == null)
            {
                BuildDashboardUI();
            }

            if (m_DashboardPanel == null) return;

            m_TabBeforeOverlay = m_CurrentTab;
            m_RestoreBackButtonAfterOverlay = m_BackButton != null && m_BackButton.activeSelf;

            Show();
            SetOverlayMode(true);
            SwitchTab(DashboardTab.Inventory);
        }

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

            ShowChannels();
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

            // HalluciCoin ゲート: 必要コイン数に満たない場合はLocked
            if (channel.RequiredHalluciCoin > 0)
            {
                int currentCoins = ContradictionManager.Instance != null ? ContradictionManager.Instance.HalluciCoin : 0;
                if (currentCoins < channel.RequiredHalluciCoin)
                {
                    return ChannelStatus.Locked;
                }
            }

            if (saveData != null)
            {
                if (saveData.CompletedChannelIDs.Contains(channel.ChannelID))
                    return ChannelStatus.Completed;

                // マルチDayチャプター: Day進捗がある場合もInProgress
                if (channel.TotalDays > 1
                    && saveData.ChannelDayProgress.ContainsKey(channel.ChannelID))
                    return ChannelStatus.InProgress;

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
            titleText.fontSize = UIFontConfig.Instance.titleFontSize;
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

            m_SubtitleText = subtitleObj.AddComponent<TextMeshProUGUI>();
            m_SubtitleText.text = "select channel_";
            m_SubtitleText.fontSize = UIFontConfig.Instance.captionFontSize;
            m_SubtitleText.fontStyle = FontStyles.Italic;
            m_SubtitleText.color = new Color(0.4f, 0.4f, 0.45f);
            m_SubtitleText.alignment = TextAlignmentOptions.Center;
            m_SubtitleText.raycastTarget = false;
            AssignDefaultFont(m_SubtitleText);

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
            m_CoinDisplay.fontSize = UIFontConfig.Instance.subheadingFontSize;
            m_CoinDisplay.color = new Color(0.8f, 0.75f, 0.4f);
            m_CoinDisplay.alignment = TextAlignmentOptions.MidlineRight;
            m_CoinDisplay.raycastTarget = false;
            AssignDefaultFont(m_CoinDisplay);

            // --- Progress Summary (between subtitle and tab bar) ---
            BuildProgressSummary(m_DashboardPanel.transform);

            // --- Tab Bar (Channels | Inventory) ---
            BuildTabBar(m_DashboardPanel.transform);

            // --- Channel Container (wraps ScrollView, toggleable) ---
            m_ChannelContainer = new GameObject("ChannelContainer", typeof(RectTransform));
            AssignUILayer(m_ChannelContainer);
            m_ChannelContainer.transform.SetParent(m_DashboardPanel.transform, false);

            RectTransform channelContRect = m_ChannelContainer.GetComponent<RectTransform>();
            channelContRect.anchorMin = Vector2.zero;
            channelContRect.anchorMax = Vector2.one;
            channelContRect.offsetMin = Vector2.zero;
            channelContRect.offsetMax = Vector2.zero;

            // --- ScrollView ---
            GameObject scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            AssignUILayer(scrollObj);
            scrollObj.transform.SetParent(m_ChannelContainer.transform, false);

            RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0f, 0f);
            scrollRect.anchorMax = new Vector2(1f, 1f);
            scrollRect.offsetMin = new Vector2(20f, 20f);
            scrollRect.offsetMax = new Vector2(-20f, -200f);

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

            // --- Inventory Tab ---
            m_InventoryTab = gameObject.AddComponent<InventoryTabController>();
            m_InventoryTab.BuildUI(m_DashboardPanel.transform);
            m_InventoryTab.Hide();

            // --- Overlay close button (shown only when opened from chat) ---
            m_CloseOverlayButton = CreateOverlayCloseButton(m_DashboardPanel.transform);
            m_CloseOverlayButton.SetActive(false);

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
            backLabel.fontSize = UIFontConfig.Instance.bodyFontSize;
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

            // 値が増加した場合にパルスアニメーション
            if (m_LastCoinValue >= 0 && coin > m_LastCoinValue)
            {
                PulseCoinDisplay();
            }
            m_LastCoinValue = coin;
        }

        /// <summary>HC 表示をパルスさせて増加を視覚通知する</summary>
        private void PulseCoinDisplay()
        {
            if (m_CoinDisplay == null) return;
            m_CoinPulseTween?.Kill();

            Color originalColor = new Color(0.8f, 0.75f, 0.4f);
            Color highlightColor = new Color(1f, 0.9f, 0.3f);
            Transform coinTransform = m_CoinDisplay.transform;

            m_CoinPulseTween = DOTween.Sequence()
                .Append(coinTransform.DOScale(1.3f, 0.15f).SetEase(Ease.OutBack))
                .Join(m_CoinDisplay.DOColor(highlightColor, 0.15f))
                .Append(coinTransform.DOScale(1f, 0.25f).SetEase(Ease.InOutSine))
                .Join(m_CoinDisplay.DOColor(originalColor, 0.25f));
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

            // ColorBlockを明示設定（UnityデフォルトのdisabledColorが背景色を上書きする問題を防止）
            Color baseColor = GetCardColor(status);
            ColorBlock cb = cardBtn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1.15f, 1.15f, 1.2f, 1f);
            cb.pressedColor = new Color(0.85f, 0.85f, 0.9f, 1f);
            cb.disabledColor = Color.white; // interactable=false時もImage色をそのまま表示
            cb.colorMultiplier = 1f;
            cardBtn.colors = cb;

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
            titleText.fontSize = UIFontConfig.Instance.headingFontSize;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = status == ChannelStatus.Locked
                ? new Color(0.45f, 0.45f, 0.5f)
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
            descText.fontSize = UIFontConfig.Instance.captionFontSize;
            descText.color = status == ChannelStatus.Locked
                ? new Color(0.38f, 0.38f, 0.42f)
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
            statusText.fontSize = UIFontConfig.Instance.smallFontSize;
            statusText.alignment = TextAlignmentOptions.MidlineRight;
            statusText.raycastTarget = false;
            AssignDefaultFont(statusText);

            // HC ゲート表示: Locked かつ RequiredHalluciCoin > 0 の場合、必要コイン数を表示
            if (status == ChannelStatus.Locked && channel.RequiredHalluciCoin > 0)
            {
                int currentCoins = ContradictionManager.Instance != null ? ContradictionManager.Instance.HalluciCoin : 0;
                statusText.text = $"HC {currentCoins}/{channel.RequiredHalluciCoin}";
                statusText.color = new Color(0.9f, 0.7f, 0.2f); // ゴールド
            }
            else
            {
                statusText.text = GetStatusLabel(status);
                statusText.color = GetStatusColor(status);
            }
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
                // 再開ノードの決定
                string resumeNode = channel.StartNodeName;
                var saveData = SaveManager.Instance?.CurrentSaveData;

                if (saveData != null && channel.TotalDays > 1)
                {
                    if (saveData.ChannelDayProgress.TryGetValue(channel.ChannelID, out int completedDay))
                    {
                        int nextDay = completedDay + 1;
                        if (nextDay <= channel.TotalDays
                            && channel.DayStartNodeNames != null
                            && channel.DayStartNodeNames.Length >= nextDay)
                        {
                            resumeNode = channel.DayStartNodeNames[nextDay - 1];
                            Debug.Log($"DashboardController: Resuming channel '{channel.ChannelID}' from day {nextDay} node '{resumeNode}'");
                        }
                    }
                }

                m_ScenarioManager.SetCurrentChannel(channel);
                m_ScenarioManager.StartScenario(resumeNode);
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
            label.fontSize = UIFontConfig.Instance.bodyFontSize;
            label.fontStyle = FontStyles.Italic;
            label.color = new Color(0.45f, 0.45f, 0.5f);
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            AssignDefaultFont(label);
        }
        private void BuildTabBar(Transform parent)
        {
            m_TabBar = new GameObject("TabBar", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup));
            AssignUILayer(m_TabBar);
            m_TabBar.transform.SetParent(parent, false);

            RectTransform barRect = m_TabBar.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0f, 1f);
            barRect.anchorMax = new Vector2(1f, 1f);
            barRect.pivot = new Vector2(0.5f, 1f);
            barRect.offsetMin = new Vector2(20f, -190f);
            barRect.offsetMax = new Vector2(-20f, -150f);

            Image barBg = m_TabBar.GetComponent<Image>();
            barBg.color = new Color(0.08f, 0.08f, 0.1f, 0.6f);

            HorizontalLayoutGroup hlg = m_TabBar.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 4f;
            hlg.padding = new RectOffset(8, 8, 4, 4);
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;

            m_ChannelTabLabel = CreateTabButton(m_TabBar.transform, "Channels", () => SwitchTab(DashboardTab.Channels));
            m_InventoryTabLabel = CreateTabButton(m_TabBar.transform, "Inventory", () => SwitchTab(DashboardTab.Inventory));

            m_ChannelTabLabel.color = new Color(0.85f, 0.85f, 0.9f);
            m_InventoryTabLabel.color = new Color(0.4f, 0.4f, 0.45f);
        }

        private TextMeshProUGUI CreateTabButton(Transform parent, string text, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btnObj = new GameObject($"Tab_{text}", typeof(RectTransform), typeof(Image), typeof(Button));
            AssignUILayer(btnObj);
            btnObj.transform.SetParent(parent, false);

            Image btnBg = btnObj.GetComponent<Image>();
            btnBg.color = new Color(0.1f, 0.1f, 0.13f, 0.8f);
            btnBg.raycastTarget = true;

            Button btn = btnObj.GetComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;
            btn.targetGraphic = btnBg;

            ColorBlock cb = btn.colors;
            cb.highlightedColor = new Color(0.18f, 0.18f, 0.24f, 0.9f);
            cb.pressedColor = new Color(0.14f, 0.14f, 0.2f, 1f);
            btn.colors = cb;

            btn.onClick.AddListener(onClick);

            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            AssignUILayer(textObj);
            textObj.transform.SetParent(btnObj.transform, false);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(4f, 2f);
            textRect.offsetMax = new Vector2(-4f, -2f);

            TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = UIFontConfig.Instance.bodyFontSize;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            AssignDefaultFont(label);

            return label;
        }

        private GameObject CreateOverlayCloseButton(Transform parent)
        {
            GameObject buttonObj = new GameObject("CloseOverlayButton", typeof(RectTransform), typeof(Image), typeof(Button));
            AssignUILayer(buttonObj);
            buttonObj.transform.SetParent(parent, false);

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(20f, -20f);
            rect.sizeDelta = new Vector2(140f, 34f);

            Image bg = buttonObj.GetComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
            bg.raycastTarget = true;

            Button button = buttonObj.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = bg;
            button.onClick.AddListener(HideInventoryOverlay);

            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            AssignUILayer(textObj);
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 4f);
            textRect.offsetMax = new Vector2(-8f, -4f);

            TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = "< return";
            label.fontSize = UIFontConfig.Instance.bodyFontSize;
            label.color = new Color(0.6f, 0.6f, 0.65f);
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            AssignDefaultFont(label);

            return buttonObj;
        }

        private void SetOverlayMode(bool isOverlay)
        {
            m_IsInventoryOverlayMode = isOverlay;

            if (m_TabBar != null)
            {
                m_TabBar.SetActive(!isOverlay);
            }

            if (m_CloseOverlayButton != null)
            {
                m_CloseOverlayButton.SetActive(isOverlay);
            }
        }

        private void HideInventoryOverlay()
        {
            if (!m_IsInventoryOverlayMode)
            {
                Hide();
                return;
            }

            SetOverlayMode(false);
            Hide();
            SwitchTab(m_TabBeforeOverlay);

            if (m_BackButton != null && m_RestoreBackButtonAfterOverlay)
            {
                m_BackButton.SetActive(true);
            }

            m_RestoreBackButtonAfterOverlay = false;
        }

        private void SwitchTab(DashboardTab tab)
        {
            if (m_IsInventoryOverlayMode && tab != DashboardTab.Inventory)
            {
                return;
            }

            m_CurrentTab = tab;

            m_ChannelTabLabel.color = tab == DashboardTab.Channels
                ? new Color(0.85f, 0.85f, 0.9f)
                : new Color(0.4f, 0.4f, 0.45f);
            m_InventoryTabLabel.color = tab == DashboardTab.Inventory
                ? new Color(0.85f, 0.85f, 0.9f)
                : new Color(0.4f, 0.4f, 0.45f);

            if (m_ChannelContainer != null)
                m_ChannelContainer.SetActive(tab == DashboardTab.Channels);

            if (tab == DashboardTab.Inventory)
            {
                if (m_InventoryTab != null) m_InventoryTab.Show();
                if (m_SubtitleText != null) m_SubtitleText.text = "inventory_";
            }
            else
            {
                if (m_InventoryTab != null) m_InventoryTab.Hide();
                if (m_SubtitleText != null) m_SubtitleText.text = "select channel_";
                RefreshChannelList();
            }
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

        private void BuildProgressSummary(Transform parent)
        {
            GameObject summaryObj = new GameObject("ProgressSummary", typeof(RectTransform));
            AssignUILayer(summaryObj);
            summaryObj.transform.SetParent(parent, false);

            m_ProgressSummary = summaryObj.AddComponent<ProgressSummaryUI>();
            m_ProgressSummary.Build(parent, AssignDefaultFont);
        }

        private void RefreshProgressSummary()
        {
            if (m_ProgressSummary == null) return;

            var tracker = ProgressTracker.Instance;
            if (tracker == null) return;

            ProgressSnapshot snapshot = tracker.GetSnapshot();
            string nudge = NudgeSystem.GetNudge(snapshot, m_Channels, SaveManager.Instance?.CurrentSaveData);
            m_ProgressSummary.UpdateDisplay(snapshot, nudge);
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
