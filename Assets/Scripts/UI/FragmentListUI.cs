using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 収集済みフラグメント（断片）の一覧表示UI。
    /// DeductionBoard.UnlockedTopics を参照してスクロールリストを動的生成する。
    /// </summary>
    public class FragmentListUI : MonoBehaviour
    {
        private GameObject m_Panel;
        private Transform m_ContentRoot;
        private GameObject m_EmptyLabel;
        private bool m_IsShowing;

        public bool IsShowing => m_IsShowing;

        public void Show()
        {
            if (m_Panel == null)
            {
                BuildUI();
            }

            RefreshList();
            m_Panel.SetActive(true);
            m_IsShowing = true;
        }

        public void Hide()
        {
            if (m_Panel != null)
            {
                m_Panel.SetActive(false);
            }
            m_IsShowing = false;
        }

        public void Toggle()
        {
            if (m_IsShowing) Hide();
            else Show();
        }

        /// <summary>
        /// DeductionBoard.UnlockedTopics から一覧を再構築する。
        /// テスト用に public。
        /// </summary>
        public void RefreshList()
        {
            if (m_ContentRoot == null) return;

            // Clear existing items
            for (int i = m_ContentRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(m_ContentRoot.GetChild(i).gameObject);
            }

            DeductionBoard board = DeductionBoard.Instance;
            List<TopicData> topics = board != null ? board.UnlockedTopics : null;

            if (topics == null || topics.Count == 0)
            {
                if (m_EmptyLabel != null) m_EmptyLabel.SetActive(true);
                return;
            }

            if (m_EmptyLabel != null) m_EmptyLabel.SetActive(false);

            foreach (TopicData topic in topics)
            {
                if (topic == null) continue;
                CreateFragmentItem(topic);
            }
        }

        /// <summary>
        /// テスト用: パネルが構築済みか
        /// </summary>
        public bool IsPanelBuilt => m_Panel != null;

        /// <summary>
        /// テスト用: コンテンツ配下のアイテム数
        /// </summary>
        public int ItemCount => m_ContentRoot != null ? m_ContentRoot.childCount : 0;

        #region UI Construction

        private void BuildUI()
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("FragmentListUI: No Canvas found in scene.");
                return;
            }

            // --- Panel ---
            m_Panel = new GameObject("FragmentListPanel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            AssignUILayer(m_Panel);
            m_Panel.transform.SetParent(canvas.transform, false);

            RectTransform panelRect = m_Panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.05f, 0.05f);
            panelRect.anchorMax = new Vector2(0.95f, 0.95f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelBg = m_Panel.GetComponent<Image>();
            panelBg.color = new Color(0.04f, 0.04f, 0.07f, 0.96f);
            panelBg.raycastTarget = true;

            // --- Header ---
            GameObject headerObj = new GameObject("Header", typeof(RectTransform));
            AssignUILayer(headerObj);
            headerObj.transform.SetParent(m_Panel.transform, false);

            RectTransform headerRect = headerObj.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.offsetMin = new Vector2(20f, -60f);
            headerRect.offsetMax = new Vector2(-20f, 0f);

            TextMeshProUGUI titleText = headerObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Fragment Archive";
            titleText.fontSize = 32f;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(0.8f, 0.82f, 0.9f);
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.raycastTarget = false;
            if (TMP_Settings.defaultFontAsset != null)
                titleText.font = TMP_Settings.defaultFontAsset;

            // --- Close Button ---
            CreateCloseButton(m_Panel.transform);

            // --- Empty Label ---
            m_EmptyLabel = new GameObject("EmptyLabel", typeof(RectTransform));
            AssignUILayer(m_EmptyLabel);
            m_EmptyLabel.transform.SetParent(m_Panel.transform, false);

            RectTransform emptyRect = m_EmptyLabel.GetComponent<RectTransform>();
            emptyRect.anchorMin = new Vector2(0.1f, 0.4f);
            emptyRect.anchorMax = new Vector2(0.9f, 0.6f);
            emptyRect.offsetMin = Vector2.zero;
            emptyRect.offsetMax = Vector2.zero;

            TextMeshProUGUI emptyText = m_EmptyLabel.AddComponent<TextMeshProUGUI>();
            emptyText.text = "No fragments collected yet.";
            emptyText.fontSize = 22f;
            emptyText.fontStyle = FontStyles.Italic;
            emptyText.color = new Color(0.4f, 0.4f, 0.45f);
            emptyText.alignment = TextAlignmentOptions.Center;
            emptyText.raycastTarget = false;
            if (TMP_Settings.defaultFontAsset != null)
                emptyText.font = TMP_Settings.defaultFontAsset;

            // --- ScrollView ---
            BuildScrollView(m_Panel.transform);
        }

        private void BuildScrollView(Transform parent)
        {
            // ScrollView container
            GameObject scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            AssignUILayer(scrollObj);
            scrollObj.transform.SetParent(parent, false);

            RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
            scrollRect.anchorMin = Vector2.zero;
            scrollRect.anchorMax = Vector2.one;
            scrollRect.offsetMin = new Vector2(16f, 16f);
            scrollRect.offsetMax = new Vector2(-16f, -70f);

            Image scrollBg = scrollObj.GetComponent<Image>();
            scrollBg.color = new Color(0.06f, 0.06f, 0.09f, 0.5f);

            // Viewport
            GameObject viewportObj = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
            AssignUILayer(viewportObj);
            viewportObj.transform.SetParent(scrollObj.transform, false);

            RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            // Content
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
            vlg.spacing = 8f;
            vlg.padding = new RectOffset(16, 16, 12, 12);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;

            ContentSizeFitter fitter = contentObj.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            m_ContentRoot = contentObj.transform;

            // Wire ScrollRect
            ScrollRect scroll = scrollObj.GetComponent<ScrollRect>();
            scroll.content = contentRect;
            scroll.viewport = viewportRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30f;
        }

        private void CreateCloseButton(Transform parent)
        {
            GameObject btnObj = new GameObject("CloseButton", typeof(RectTransform), typeof(Image), typeof(Button));
            AssignUILayer(btnObj);
            btnObj.transform.SetParent(parent, false);

            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1f, 1f);
            btnRect.anchorMax = new Vector2(1f, 1f);
            btnRect.pivot = new Vector2(1f, 1f);
            btnRect.anchoredPosition = new Vector2(-12f, -12f);
            btnRect.sizeDelta = new Vector2(44f, 44f);

            Image btnBg = btnObj.GetComponent<Image>();
            btnBg.color = new Color(0.3f, 0.15f, 0.15f, 0.85f);
            btnBg.raycastTarget = true;

            Button btn = btnObj.GetComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;
            btn.targetGraphic = btnBg;

            ColorBlock cb = btn.colors;
            cb.highlightedColor = new Color(0.5f, 0.2f, 0.2f, 0.9f);
            cb.pressedColor = new Color(0.6f, 0.15f, 0.15f, 1f);
            btn.colors = cb;

            btn.onClick.AddListener(Hide);

            // "X" text
            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            AssignUILayer(textObj);
            textObj.transform.SetParent(btnObj.transform, false);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = "X";
            label.fontSize = 24f;
            label.fontStyle = FontStyles.Bold;
            label.color = new Color(0.9f, 0.7f, 0.7f);
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            if (TMP_Settings.defaultFontAsset != null)
                label.font = TMP_Settings.defaultFontAsset;
        }

        private void CreateFragmentItem(TopicData topic)
        {
            GameObject itemObj = new GameObject($"Fragment_{topic.TopicID}", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            AssignUILayer(itemObj);
            itemObj.transform.SetParent(m_ContentRoot, false);

            Image itemBg = itemObj.GetComponent<Image>();
            itemBg.color = new Color(0.12f, 0.12f, 0.16f, 0.9f);
            itemBg.raycastTarget = true;

            LayoutElement layout = itemObj.GetComponent<LayoutElement>();
            layout.minHeight = 80f;
            layout.preferredHeight = 100f;
            layout.flexibleWidth = 1f;

            // Inner layout
            VerticalLayoutGroup innerVlg = itemObj.AddComponent<VerticalLayoutGroup>();
            innerVlg.spacing = 4f;
            innerVlg.padding = new RectOffset(16, 16, 10, 10);
            innerVlg.childForceExpandWidth = true;
            innerVlg.childForceExpandHeight = false;
            innerVlg.childControlWidth = true;
            innerVlg.childControlHeight = true;

            // Title
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(LayoutElement));
            AssignUILayer(titleObj);
            titleObj.transform.SetParent(itemObj.transform, false);

            titleObj.GetComponent<LayoutElement>().preferredHeight = 28f;

            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = topic.Title;
            titleText.fontSize = 22f;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(0.85f, 0.87f, 0.95f);
            titleText.alignment = TextAlignmentOptions.MidlineLeft;
            titleText.raycastTarget = false;
            if (TMP_Settings.defaultFontAsset != null)
                titleText.font = TMP_Settings.defaultFontAsset;

            // Description
            if (!string.IsNullOrEmpty(topic.Description))
            {
                GameObject descObj = new GameObject("Description", typeof(RectTransform), typeof(LayoutElement));
                AssignUILayer(descObj);
                descObj.transform.SetParent(itemObj.transform, false);

                descObj.GetComponent<LayoutElement>().preferredHeight = 48f;

                TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
                descText.text = topic.Description;
                descText.fontSize = 16f;
                descText.color = new Color(0.55f, 0.55f, 0.6f);
                descText.alignment = TextAlignmentOptions.TopLeft;
                descText.maxVisibleLines = 3;
                descText.overflowMode = TextOverflowModes.Ellipsis;
                descText.raycastTarget = false;
                if (TMP_Settings.defaultFontAsset != null)
                    descText.font = TMP_Settings.defaultFontAsset;
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
