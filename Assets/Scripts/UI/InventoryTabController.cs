#if YARN_SPINNER
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// ダッシュボード内インベントリタブ。
    /// 断片/記録/トピックの3サブタブを切り替えて表示する。
    /// DashboardController から BuildUI() で生成される。
    /// </summary>
    public class InventoryTabController : MonoBehaviour
    {
        public enum InventorySubTab { Fragments, Records, Topics }
        private enum InventoryItemCategory { Fragment, Record, Topic }

        private GameObject m_RootPanel;
        private GameObject m_FragmentPanel;
        private GameObject m_RecordPanel;
        private GameObject m_TopicPanel;
        private Transform m_FragmentContent;
        private Transform m_TopicContent;
        private GameObject m_EmptyFragmentLabel;
        private GameObject m_EmptyTopicLabel;
        private TextMeshProUGUI[] m_SubTabLabels;
        private InventorySubTab m_CurrentSubTab = InventorySubTab.Fragments;
        private bool m_IsBuilt;
        private bool m_IsShowing;

        private static readonly Color ActiveTabColor = new Color(0.85f, 0.85f, 0.9f);
        private static readonly Color InactiveTabColor = new Color(0.4f, 0.4f, 0.45f);
        private static readonly Color CardBgColor = new Color(0.12f, 0.12f, 0.16f, 0.9f);
        private static readonly Color TitleColor = new Color(0.85f, 0.87f, 0.95f);
        private static readonly Color DescColor = new Color(0.55f, 0.55f, 0.6f);
        private static readonly Color ChapterBadgeColor = new Color(0.5f, 0.6f, 0.8f);
        private static readonly Color PlaceholderColor = new Color(0.45f, 0.45f, 0.5f);

        private static readonly Regex ChapterRegex = new Regex(@"fragment_ch(\d+)_", RegexOptions.Compiled);

        public bool IsShowing => m_IsShowing;
        public bool IsBuilt => m_IsBuilt;
        public InventorySubTab CurrentSubTab => m_CurrentSubTab;
        public int FragmentItemCount => CountContentItems(m_FragmentContent, m_EmptyFragmentLabel);
        public int TopicItemCount => CountContentItems(m_TopicContent, m_EmptyTopicLabel);

        #region Public Methods

        public void BuildUI(Transform parent)
        {
            if (m_IsBuilt) return;

            // Root container (fills parent, positioned below tab bar)
            m_RootPanel = new GameObject("InventoryRoot", typeof(RectTransform));
            AssignUILayer(m_RootPanel);
            m_RootPanel.transform.SetParent(parent, false);

            RectTransform rootRect = m_RootPanel.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = new Vector2(20f, 20f);
            rootRect.offsetMax = new Vector2(-20f, -150f);

            // Sub-tab bar
            BuildSubTabBar(m_RootPanel.transform);

            // Content panels (below sub-tab bar)
            m_FragmentPanel = BuildScrollPanel(m_RootPanel.transform, "FragmentScroll", out m_FragmentContent);
            m_RecordPanel = BuildPlaceholderPanel(m_RootPanel.transform, "RecordPanel");
            m_TopicPanel = BuildScrollPanel(m_RootPanel.transform, "TopicScroll", out m_TopicContent);

            // Empty labels
            m_EmptyFragmentLabel = CreateEmptyLabel(m_FragmentContent, "No fragments collected yet.");
            m_EmptyTopicLabel = CreateEmptyLabel(m_TopicContent, "No topics unlocked yet.");

            m_IsBuilt = true;
            SwitchSubTab(InventorySubTab.Fragments);
        }

        public void Show()
        {
            if (m_RootPanel == null) return;
            Refresh();
            m_RootPanel.SetActive(true);
            m_IsShowing = true;
        }

        public void Hide()
        {
            if (m_RootPanel != null)
            {
                m_RootPanel.SetActive(false);
            }
            m_IsShowing = false;
        }

        public void Refresh()
        {
            if (m_CurrentSubTab == InventorySubTab.Fragments)
                RefreshFragments();
            else if (m_CurrentSubTab == InventorySubTab.Topics)
                RefreshTopics();
        }

        public void SelectSubTab(InventorySubTab tab)
        {
            if (!m_IsBuilt) return;
            SwitchSubTab(tab);
        }

        #endregion

        #region Sub-Tab Switching

        private void SwitchSubTab(InventorySubTab tab)
        {
            m_CurrentSubTab = tab;

            m_FragmentPanel.SetActive(tab == InventorySubTab.Fragments);
            m_RecordPanel.SetActive(tab == InventorySubTab.Records);
            m_TopicPanel.SetActive(tab == InventorySubTab.Topics);

            for (int i = 0; i < m_SubTabLabels.Length; i++)
            {
                m_SubTabLabels[i].color = i == (int)tab ? ActiveTabColor : InactiveTabColor;
            }

            if (tab == InventorySubTab.Fragments)
                RefreshFragments();
            else if (tab == InventorySubTab.Topics)
                RefreshTopics();
        }

        #endregion

        #region Data Refresh

        private void RefreshFragments()
        {
            if (m_FragmentContent == null) return;
            ClearDynamicChildren(m_FragmentContent, m_EmptyFragmentLabel);

            DeductionBoard board = DeductionBoard.Instance;
            IReadOnlyList<TopicData> topics = board != null ? board.UnlockedTopics : null;

            var fragments = FilterTopics(topics, InventoryItemCategory.Fragment);

            if (fragments.Count == 0)
            {
                if (m_EmptyFragmentLabel != null) m_EmptyFragmentLabel.SetActive(true);
                return;
            }

            if (m_EmptyFragmentLabel != null) m_EmptyFragmentLabel.SetActive(false);

            foreach (TopicData topic in fragments)
            {
                CreateFragmentCard(topic);
            }
        }

        private void RefreshTopics()
        {
            if (m_TopicContent == null) return;
            ClearDynamicChildren(m_TopicContent, m_EmptyTopicLabel);

            DeductionBoard board = DeductionBoard.Instance;
            IReadOnlyList<TopicData> topics = board != null ? board.UnlockedTopics : null;

            var filtered = FilterTopics(topics, InventoryItemCategory.Topic);

            if (filtered.Count == 0)
            {
                if (m_EmptyTopicLabel != null) m_EmptyTopicLabel.SetActive(true);
                return;
            }

            if (m_EmptyTopicLabel != null) m_EmptyTopicLabel.SetActive(false);

            foreach (TopicData topic in filtered)
            {
                CreateTopicItem(topic);
            }
        }

        private static List<TopicData> FilterTopics(IReadOnlyList<TopicData> topics, InventoryItemCategory category)
        {
            var result = new List<TopicData>();
            if (topics == null) return result;

            for (int i = 0; i < topics.Count; i++)
            {
                TopicData topic = topics[i];
                if (topic != null && GetCategory(topic) == category)
                    result.Add(topics[i]);
            }

            return result;
        }

        private static InventoryItemCategory GetCategory(TopicData topic)
        {
            if (topic == null || string.IsNullOrWhiteSpace(topic.TopicID))
                return InventoryItemCategory.Topic;

            string topicId = topic.TopicID;

            if (topicId.StartsWith("fragment_", StringComparison.OrdinalIgnoreCase))
                return InventoryItemCategory.Fragment;

            if (topicId.StartsWith("record_", StringComparison.OrdinalIgnoreCase))
                return InventoryItemCategory.Record;

            return InventoryItemCategory.Topic;
        }

        #endregion

        #region UI Construction

        private void BuildSubTabBar(Transform parent)
        {
            GameObject barObj = new GameObject("SubTabBar", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup));
            AssignUILayer(barObj);
            barObj.transform.SetParent(parent, false);

            RectTransform barRect = barObj.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0f, 1f);
            barRect.anchorMax = new Vector2(1f, 1f);
            barRect.pivot = new Vector2(0.5f, 1f);
            barRect.offsetMin = new Vector2(0f, -40f);
            barRect.offsetMax = Vector2.zero;

            Image barBg = barObj.GetComponent<Image>();
            barBg.color = new Color(0.08f, 0.08f, 0.1f, 0.6f);

            HorizontalLayoutGroup hlg = barObj.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 4f;
            hlg.padding = new RectOffset(8, 8, 4, 4);
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;

            string[] tabNames = { "Fragments", "Records", "Topics" };
            m_SubTabLabels = new TextMeshProUGUI[3];

            for (int i = 0; i < 3; i++)
            {
                int tabIndex = i;
                GameObject tabBtn = new GameObject($"SubTab_{tabNames[i]}", typeof(RectTransform), typeof(Image), typeof(Button));
                AssignUILayer(tabBtn);
                tabBtn.transform.SetParent(barObj.transform, false);

                Image tabBg = tabBtn.GetComponent<Image>();
                tabBg.color = new Color(0.1f, 0.1f, 0.13f, 0.8f);
                tabBg.raycastTarget = true;

                Button btn = tabBtn.GetComponent<Button>();
                btn.transition = Selectable.Transition.ColorTint;
                btn.targetGraphic = tabBg;

                ColorBlock cb = btn.colors;
                cb.highlightedColor = new Color(0.18f, 0.18f, 0.24f, 0.9f);
                cb.pressedColor = new Color(0.14f, 0.14f, 0.2f, 1f);
                btn.colors = cb;

                btn.onClick.AddListener(() => SwitchSubTab((InventorySubTab)tabIndex));

                GameObject textObj = new GameObject("Text", typeof(RectTransform));
                AssignUILayer(textObj);
                textObj.transform.SetParent(tabBtn.transform, false);

                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = new Vector2(4f, 2f);
                textRect.offsetMax = new Vector2(-4f, -2f);

                TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
                label.text = tabNames[i];
                label.fontSize = UIFontConfig.Instance.captionFontSize;
                label.alignment = TextAlignmentOptions.Center;
                label.raycastTarget = false;
                AssignDefaultFont(label);

                m_SubTabLabels[i] = label;
            }
        }

        private GameObject BuildScrollPanel(Transform parent, string name, out Transform contentRoot)
        {
            // Container (below sub-tab bar)
            GameObject container = new GameObject(name, typeof(RectTransform));
            AssignUILayer(container);
            container.transform.SetParent(parent, false);

            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = new Vector2(0f, -44f); // below sub-tab bar

            // ScrollView
            GameObject scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            AssignUILayer(scrollObj);
            scrollObj.transform.SetParent(container.transform, false);

            RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
            scrollRect.anchorMin = Vector2.zero;
            scrollRect.anchorMax = Vector2.one;
            scrollRect.offsetMin = Vector2.zero;
            scrollRect.offsetMax = Vector2.zero;

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
            vlg.padding = new RectOffset(12, 12, 8, 8);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;

            ContentSizeFitter fitter = contentObj.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            contentRoot = contentObj.transform;

            ScrollRect scroll = scrollObj.GetComponent<ScrollRect>();
            scroll.content = contentRect;
            scroll.viewport = viewportRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30f;

            return container;
        }

        private GameObject BuildPlaceholderPanel(Transform parent, string name)
        {
            GameObject container = new GameObject(name, typeof(RectTransform));
            AssignUILayer(container);
            container.transform.SetParent(parent, false);

            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = new Vector2(0f, -44f);

            GameObject labelObj = new GameObject("PlaceholderText", typeof(RectTransform));
            AssignUILayer(labelObj);
            labelObj.transform.SetParent(container.transform, false);

            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.1f, 0.4f);
            labelRect.anchorMax = new Vector2(0.9f, 0.6f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = "Coming soon...";
            label.fontSize = UIFontConfig.Instance.headingFontSize;
            label.fontStyle = FontStyles.Italic;
            label.color = PlaceholderColor;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            AssignDefaultFont(label);

            return container;
        }

        private GameObject CreateEmptyLabel(Transform parent, string text)
        {
            GameObject labelObj = new GameObject("EmptyLabel", typeof(RectTransform), typeof(LayoutElement));
            AssignUILayer(labelObj);
            labelObj.transform.SetParent(parent, false);

            LayoutElement layout = labelObj.GetComponent<LayoutElement>();
            layout.minHeight = 60f;
            layout.flexibleWidth = 1f;

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = UIFontConfig.Instance.subheadingFontSize;
            label.fontStyle = FontStyles.Italic;
            label.color = PlaceholderColor;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            AssignDefaultFont(label);

            return labelObj;
        }

        private void CreateFragmentCard(TopicData topic)
        {
            GameObject cardObj = new GameObject($"Fragment_{topic.TopicID}",
                typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            AssignUILayer(cardObj);
            cardObj.transform.SetParent(m_FragmentContent, false);

            Image cardBg = cardObj.GetComponent<Image>();
            cardBg.color = CardBgColor;
            cardBg.raycastTarget = true;

            LayoutElement layout = cardObj.GetComponent<LayoutElement>();
            layout.minHeight = 80f;
            layout.preferredHeight = 100f;
            layout.flexibleWidth = 1f;

            VerticalLayoutGroup innerVlg = cardObj.AddComponent<VerticalLayoutGroup>();
            innerVlg.spacing = 4f;
            innerVlg.padding = new RectOffset(16, 16, 10, 10);
            innerVlg.childForceExpandWidth = true;
            innerVlg.childForceExpandHeight = false;
            innerVlg.childControlWidth = true;
            innerVlg.childControlHeight = true;

            // Title row (title + chapter badge)
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(LayoutElement));
            AssignUILayer(titleObj);
            titleObj.transform.SetParent(cardObj.transform, false);
            titleObj.GetComponent<LayoutElement>().preferredHeight = 28f;

            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            string chapterBadge = ExtractChapter(topic.TopicID);
            string displayTitle = GetDisplayTitle(topic);
            titleText.text = chapterBadge != null
                ? $"{displayTitle}  <size={UIFontConfig.Instance.smallFontSize:F0}><color=#{ColorUtility.ToHtmlStringRGB(ChapterBadgeColor)}>{chapterBadge}</color></size>"
                : displayTitle;
            titleText.fontSize = UIFontConfig.Instance.headingFontSize;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = TitleColor;
            titleText.alignment = TextAlignmentOptions.MidlineLeft;
            titleText.richText = true;
            titleText.raycastTarget = false;
            AssignDefaultFont(titleText);

            // Description
            if (!string.IsNullOrEmpty(topic.Description))
            {
                GameObject descObj = new GameObject("Description", typeof(RectTransform), typeof(LayoutElement));
                AssignUILayer(descObj);
                descObj.transform.SetParent(cardObj.transform, false);
                descObj.GetComponent<LayoutElement>().preferredHeight = 48f;

                TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
                descText.text = topic.Description;
                descText.fontSize = UIFontConfig.Instance.captionFontSize;
                descText.color = DescColor;
                descText.alignment = TextAlignmentOptions.TopLeft;
                descText.maxVisibleLines = 3;
                descText.overflowMode = TextOverflowModes.Ellipsis;
                descText.raycastTarget = false;
                AssignDefaultFont(descText);
            }
        }

        private void CreateTopicItem(TopicData topic)
        {
            GameObject itemObj = new GameObject($"Topic_{topic.TopicID}",
                typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            AssignUILayer(itemObj);
            itemObj.transform.SetParent(m_TopicContent, false);

            Image itemBg = itemObj.GetComponent<Image>();
            itemBg.color = CardBgColor;
            itemBg.raycastTarget = true;

            LayoutElement layout = itemObj.GetComponent<LayoutElement>();
            layout.minHeight = 48f;
            layout.preferredHeight = 56f;
            layout.flexibleWidth = 1f;

            // Title text (centered vertically)
            GameObject textObj = new GameObject("Title", typeof(RectTransform));
            AssignUILayer(textObj);
            textObj.transform.SetParent(itemObj.transform, false);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(16f, 4f);
            textRect.offsetMax = new Vector2(-16f, -4f);

            TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = GetDisplayTitle(topic);
            label.fontSize = UIFontConfig.Instance.subheadingFontSize;
            label.color = TitleColor;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.raycastTarget = false;
            AssignDefaultFont(label);
        }

        #endregion

        #region Helpers

        private static string ExtractChapter(string topicID)
        {
            Match m = ChapterRegex.Match(topicID);
            return m.Success ? $"Ch.{m.Groups[1].Value}" : null;
        }

        private static string GetDisplayTitle(TopicData topic)
        {
            if (topic == null)
                return "Untitled";

            return string.IsNullOrWhiteSpace(topic.Title) ? topic.TopicID : topic.Title;
        }

        private static void ClearDynamicChildren(Transform parent, GameObject preservedChild)
        {
            if (parent == null) return;

            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                if (child == null) continue;
                if (preservedChild != null && child.gameObject == preservedChild) continue;

                SafeDestroy(child.gameObject);
            }
        }

        private static int CountContentItems(Transform parent, GameObject preservedChild)
        {
            if (parent == null) return 0;

            int count = 0;
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child == null) continue;
                if (preservedChild != null && child.gameObject == preservedChild) continue;
                count++;
            }

            return count;
        }

        private static void SafeDestroy(UnityEngine.Object obj)
        {
            if (obj == null) return;

            if (Application.isPlaying)
                UnityEngine.Object.Destroy(obj);
            else
                UnityEngine.Object.DestroyImmediate(obj);
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
