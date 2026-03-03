#if YARN_SPINNER
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.DebugTools
{
    /// <summary>
    /// Runtime debug hub — lists all Yarn nodes and allows one-click playback.
    /// Toggle with F12. Precursor to the future dashboard UI.
    /// </summary>
    public class DebugHubController : MonoBehaviour
    {
        [SerializeField] private DialogueRunner m_DialogueRunner;
        [SerializeField] private bool m_ShowOnStart = false;

        private GameObject m_HubPanel;
        private GameObject m_BackButton;
        private Transform m_NodeListContent;
        private bool m_IsShowing;

        #region Unity Lifecycle
        private void Start()
        {
            if (m_DialogueRunner == null)
            {
                m_DialogueRunner = FindFirstObjectByType<DialogueRunner>();
            }

            if (m_ShowOnStart)
            {
                Show();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                if (m_IsShowing)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
        }
        #endregion

        #region Public Methods
        public void Show()
        {
            // Stop any running dialogue first
            if (m_DialogueRunner != null && m_DialogueRunner.IsDialogueRunning)
            {
                ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
                if (scenarioManager != null)
                {
                    scenarioManager.StopScenario();
                }
                else
                {
                    m_DialogueRunner.Stop();
                }
            }

            if (m_HubPanel == null)
            {
                BuildHubUI();
            }

            PopulateNodeList();
            m_HubPanel.SetActive(true);

            if (m_BackButton != null)
            {
                m_BackButton.SetActive(false);
            }

            m_IsShowing = true;
        }

        public void Hide()
        {
            if (m_HubPanel != null)
            {
                m_HubPanel.SetActive(false);
            }

            m_IsShowing = false;
        }

        public void ReturnToHub()
        {
            ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null)
            {
                scenarioManager.StopScenario();
            }
            else if (m_DialogueRunner != null)
            {
                m_DialogueRunner.Stop();
            }

            ChatController chatController = FindFirstObjectByType<ChatController>();
            if (chatController != null)
            {
                chatController.ClearMessages();
            }

            Show();
        }
        #endregion

        #region Private Methods
        private void BuildHubUI()
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                UnityEngine.Debug.LogError("DebugHubController: No Canvas found in scene.");
                return;
            }

            // --- Hub Panel (full-screen overlay) ---
            m_HubPanel = new GameObject("DebugHubPanel", typeof(RectTransform), typeof(Image));
            AssignUILayer(m_HubPanel);
            m_HubPanel.transform.SetParent(canvas.transform, false);

            RectTransform panelRect = m_HubPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelBg = m_HubPanel.GetComponent<Image>();
            panelBg.color = new Color(0.05f, 0.05f, 0.08f, 0.95f);
            panelBg.raycastTarget = true;

            // --- Title ---
            GameObject titleObj = new GameObject("Title", typeof(RectTransform));
            AssignUILayer(titleObj);
            titleObj.transform.SetParent(m_HubPanel.transform, false);

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.offsetMin = new Vector2(20f, -80f);
            titleRect.offsetMax = new Vector2(-20f, -16f);

            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Debug Hub";
            titleText.fontSize = 36f;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(0.8f, 0.8f, 0.85f);
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.raycastTarget = false;
            if (TMP_Settings.defaultFontAsset != null)
            {
                titleText.font = TMP_Settings.defaultFontAsset;
            }

            // --- Subtitle ---
            GameObject subtitleObj = new GameObject("Subtitle", typeof(RectTransform));
            AssignUILayer(subtitleObj);
            subtitleObj.transform.SetParent(m_HubPanel.transform, false);

            RectTransform subtitleRect = subtitleObj.GetComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0f, 1f);
            subtitleRect.anchorMax = new Vector2(1f, 1f);
            subtitleRect.pivot = new Vector2(0.5f, 1f);
            subtitleRect.offsetMin = new Vector2(20f, -110f);
            subtitleRect.offsetMax = new Vector2(-20f, -80f);

            TextMeshProUGUI subtitleText = subtitleObj.AddComponent<TextMeshProUGUI>();
            subtitleText.text = "Select a Yarn node to play  |  F12 to toggle";
            subtitleText.fontSize = 18f;
            subtitleText.fontStyle = FontStyles.Italic;
            subtitleText.color = new Color(0.5f, 0.5f, 0.55f);
            subtitleText.alignment = TextAlignmentOptions.Center;
            subtitleText.raycastTarget = false;
            if (TMP_Settings.defaultFontAsset != null)
            {
                subtitleText.font = TMP_Settings.defaultFontAsset;
            }

            // --- ScrollView ---
            GameObject scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            AssignUILayer(scrollObj);
            scrollObj.transform.SetParent(m_HubPanel.transform, false);

            RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0f, 0f);
            scrollRect.anchorMax = new Vector2(1f, 1f);
            scrollRect.offsetMin = new Vector2(20f, 20f);
            scrollRect.offsetMax = new Vector2(-20f, -120f);

            Image scrollBg = scrollObj.GetComponent<Image>();
            scrollBg.color = new Color(0.08f, 0.08f, 0.1f, 0.6f);

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
            vlg.spacing = 6f;
            vlg.padding = new RectOffset(12, 12, 8, 8);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;

            ContentSizeFitter fitter = contentObj.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            m_NodeListContent = contentObj.transform;

            // Wire up ScrollRect
            ScrollRect scroll = scrollObj.GetComponent<ScrollRect>();
            scroll.content = contentRect;
            scroll.viewport = viewportRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.scrollSensitivity = 30f;

            // --- Back to Hub button (floating, top-left, hidden initially) ---
            m_BackButton = new GameObject("BackToHubButton", typeof(RectTransform), typeof(Image), typeof(Button));
            AssignUILayer(m_BackButton);
            m_BackButton.transform.SetParent(canvas.transform, false);

            RectTransform backRect = m_BackButton.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0f, 1f);
            backRect.anchorMax = new Vector2(0f, 1f);
            backRect.pivot = new Vector2(0f, 1f);
            backRect.anchoredPosition = new Vector2(12f, -12f);
            backRect.sizeDelta = new Vector2(160f, 40f);

            Image backBg = m_BackButton.GetComponent<Image>();
            backBg.color = new Color(0.2f, 0.2f, 0.25f, 0.85f);
            backBg.raycastTarget = true;

            Button backBtn = m_BackButton.GetComponent<Button>();
            backBtn.transition = Selectable.Transition.ColorTint;
            backBtn.targetGraphic = backBg;
            backBtn.onClick.AddListener(ReturnToHub);

            GameObject backTextObj = new GameObject("Text", typeof(RectTransform));
            AssignUILayer(backTextObj);
            backTextObj.transform.SetParent(m_BackButton.transform, false);

            RectTransform backTextRect = backTextObj.GetComponent<RectTransform>();
            backTextRect.anchorMin = Vector2.zero;
            backTextRect.anchorMax = Vector2.one;
            backTextRect.offsetMin = new Vector2(8f, 4f);
            backTextRect.offsetMax = new Vector2(-8f, -4f);

            TextMeshProUGUI backLabel = backTextObj.AddComponent<TextMeshProUGUI>();
            backLabel.text = "Back to Hub";
            backLabel.fontSize = 20f;
            backLabel.color = new Color(0.7f, 0.7f, 0.75f);
            backLabel.alignment = TextAlignmentOptions.Center;
            backLabel.raycastTarget = false;
            if (TMP_Settings.defaultFontAsset != null)
            {
                backLabel.font = TMP_Settings.defaultFontAsset;
            }

            m_BackButton.SetActive(false);
        }

        private void PopulateNodeList()
        {
            if (m_NodeListContent == null) return;

            // Clear existing buttons
            for (int i = m_NodeListContent.childCount - 1; i >= 0; i--)
            {
                Destroy(m_NodeListContent.GetChild(i).gameObject);
            }

            if (m_DialogueRunner == null || m_DialogueRunner.YarnProject == null)
            {
                CreateInfoLabel("No YarnProject found. Assign a DialogueRunner.");
                return;
            }

            string[] nodeNames = m_DialogueRunner.YarnProject.NodeNames;
            if (nodeNames == null || nodeNames.Length == 0)
            {
                CreateInfoLabel("No Yarn nodes found in the project.");
                return;
            }

            string[] sorted = nodeNames
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .OrderBy(n => n, System.StringComparer.Ordinal)
                .ToArray();

            CreateInfoLabel($"{sorted.Length} nodes available");

            foreach (string nodeName in sorted)
            {
                CreateNodeButton(nodeName);
            }
        }

        private void OnNodeButtonClicked(string nodeName)
        {
            Hide();

            if (m_BackButton != null)
            {
                m_BackButton.SetActive(true);
            }

            ChatController chatController = FindFirstObjectByType<ChatController>();
            if (chatController != null)
            {
                chatController.ClearMessages();
            }

            ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null)
            {
                scenarioManager.StartScenario(nodeName);
            }
            else if (m_DialogueRunner != null)
            {
                m_DialogueRunner.StartDialogue(nodeName);
            }
        }

        private void CreateNodeButton(string nodeName)
        {
            GameObject btnObj = new GameObject($"Node_{nodeName}", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            AssignUILayer(btnObj);
            btnObj.transform.SetParent(m_NodeListContent, false);

            Image btnBg = btnObj.GetComponent<Image>();
            btnBg.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
            btnBg.raycastTarget = true;

            Button btn = btnObj.GetComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;
            btn.targetGraphic = btnBg;

            ColorBlock cb = btn.colors;
            cb.highlightedColor = new Color(0.25f, 0.45f, 0.8f, 0.9f);
            cb.pressedColor = new Color(0.15f, 0.35f, 0.7f, 1.0f);
            btn.colors = cb;

            string capturedName = nodeName;
            btn.onClick.AddListener(() => OnNodeButtonClicked(capturedName));

            LayoutElement layout = btnObj.GetComponent<LayoutElement>();
            layout.minHeight = 48f;
            layout.preferredHeight = 52f;
            layout.flexibleWidth = 1f;

            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            AssignUILayer(textObj);
            textObj.transform.SetParent(btnObj.transform, false);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(16f, 4f);
            textRect.offsetMax = new Vector2(-16f, -4f);

            TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = nodeName;
            label.fontSize = 24f;
            label.color = new Color(0.85f, 0.85f, 0.9f);
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.raycastTarget = false;
            if (TMP_Settings.defaultFontAsset != null)
            {
                label.font = TMP_Settings.defaultFontAsset;
            }
        }

        private void CreateInfoLabel(string text)
        {
            GameObject labelObj = new GameObject("InfoLabel", typeof(RectTransform), typeof(LayoutElement));
            AssignUILayer(labelObj);
            labelObj.transform.SetParent(m_NodeListContent, false);

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
            if (TMP_Settings.defaultFontAsset != null)
            {
                label.font = TMP_Settings.defaultFontAsset;
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
