using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 分岐終了時の知識転送選択UI。
    /// TransferFlags内のトピックをチェックボックス付きで表示し、
    /// プレイヤーが「メインに持ち帰る」ものを選択する。
    /// </summary>
    public class TransferSelectionUI : MonoBehaviour
    {
        /// <summary>選択候補データ</summary>
        public struct TransferCandidate
        {
            public string TopicId;
            public string Title;
            public string Description;
        }

        // UI colors (InventoryTabController / ChatUIConfig 準拠)
        private static readonly Color PanelBgColor = new Color(0.12f, 0.12f, 0.16f, 0.95f);
        private static readonly Color ScrimColor = new Color(0f, 0f, 0f, 0.6f);
        private static readonly Color HeaderColor = new Color(0.75f, 0.78f, 0.88f, 1f);
        private static readonly Color TitleColor = new Color(0.85f, 0.87f, 0.95f, 1f);
        private static readonly Color DescColor = new Color(0.55f, 0.55f, 0.6f, 1f);
        private static readonly Color CountColor = new Color(0.6f, 0.65f, 0.75f, 1f);
        private static readonly Color ToggleOnColor = new Color(0.3f, 0.7f, 0.5f, 1f);
        private static readonly Color ToggleOffColor = new Color(0.3f, 0.3f, 0.35f, 1f);
        private static readonly Color ButtonBgColor = new Color(0.22f, 0.25f, 0.35f, 0.9f);
        private static readonly Color ButtonTextColor = new Color(0.82f, 0.85f, 0.95f, 1f);
        private static readonly Color ItemBgDefault = new Color(0.15f, 0.15f, 0.2f, 0.8f);
        private static readonly Color ItemBgSelected = new Color(0.18f, 0.22f, 0.3f, 0.9f);

        private CanvasGroup m_OverlayGroup;
        private TextMeshProUGUI m_CountLabel;
        private Button m_ConfirmButton;
        private readonly List<ToggleEntry> m_Entries = new List<ToggleEntry>();
        private Action<List<string>> m_OnConfirm;

        private struct ToggleEntry
        {
            public string TopicId;
            public bool IsSelected;
            public Image Background;
            public Image ToggleIndicator;
        }

        /// <summary>
        /// 選択UIを表示する。
        /// </summary>
        /// <param name="candidates">選択候補リスト</param>
        /// <param name="onConfirm">確定時コールバック (選択されたtopicIdリスト)</param>
        public void Show(List<TransferCandidate> candidates, Action<List<string>> onConfirm)
        {
            m_OnConfirm = onConfirm;
            m_Entries.Clear();

            // 既存子オブジェクトを破棄
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            BuildUI(candidates);

            // フェードイン
            m_OverlayGroup.alpha = 0f;
            gameObject.SetActive(true);
            m_OverlayGroup.DOFade(1f, 0.25f).SetEase(Ease.OutCubic).SetUpdate(true);
        }

        private void BuildUI(List<TransferCandidate> candidates)
        {
            // === Overlay (スクリム) ===
            var overlayObj = new GameObject("TransferOverlay", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            overlayObj.transform.SetParent(transform, false);
            var overlayRect = overlayObj.GetComponent<RectTransform>();
            StretchFull(overlayRect);
            overlayObj.GetComponent<Image>().color = ScrimColor;
            m_OverlayGroup = overlayObj.GetComponent<CanvasGroup>();
            m_OverlayGroup.blocksRaycasts = true;

            // スクリムクリックで閉じない (意図的に決定を強制)

            // === Panel (中央カード) ===
            var panelObj = new GameObject("TransferPanel", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
            panelObj.transform.SetParent(overlayObj.transform, false);
            var panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.1f, 0.15f);
            panelRect.anchorMax = new Vector2(0.9f, 0.85f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panelObj.GetComponent<Image>().color = PanelBgColor;
            var panelLayout = panelObj.GetComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(20, 20, 16, 16);
            panelLayout.spacing = 12f;
            panelLayout.childAlignment = TextAnchor.UpperCenter;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;

            // === Header ===
            var headerObj = CreateTextObject(panelObj.transform, "Header", "持ち帰る情報を選択", 22f, HeaderColor);
            var headerLayout = headerObj.AddComponent<LayoutElement>();
            headerLayout.preferredHeight = 40f;

            // === ScrollView ===
            var scrollObj = CreateScrollView(panelObj.transform, candidates);
            var scrollLayout = scrollObj.AddComponent<LayoutElement>();
            scrollLayout.flexibleHeight = 1f;
            scrollLayout.minHeight = 80f;

            // === Count Label ===
            var countObj = CreateTextObject(panelObj.transform, "CountLabel", "", 16f, CountColor);
            var countLayout = countObj.AddComponent<LayoutElement>();
            countLayout.preferredHeight = 28f;
            m_CountLabel = countObj.GetComponent<TextMeshProUGUI>();
            UpdateCountLabel();

            // === Confirm Button ===
            var btnObj = CreateButton(panelObj.transform, "ConfirmBtn", "決定", () => OnConfirmClicked());
            var btnLayout = btnObj.AddComponent<LayoutElement>();
            btnLayout.preferredHeight = 48f;
        }

        private GameObject CreateScrollView(Transform parent, List<TransferCandidate> candidates)
        {
            // ScrollView container
            var scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            scrollObj.transform.SetParent(parent, false);
            scrollObj.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.15f);

            var scrollRect = scrollObj.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 30f;

            // Viewport
            var viewportObj = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewportObj.transform.SetParent(scrollObj.transform, false);
            var viewportRect = viewportObj.GetComponent<RectTransform>();
            StretchFull(viewportRect);
            viewportObj.GetComponent<Image>().color = Color.white;
            viewportObj.GetComponent<Mask>().showMaskGraphic = false;
            scrollRect.viewport = viewportRect;

            // Content
            var contentObj = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            contentObj.transform.SetParent(viewportObj.transform, false);
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.offsetMin = new Vector2(0, 0);
            contentRect.offsetMax = new Vector2(0, 0);
            var contentLayout = contentObj.GetComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(8, 8, 8, 8);
            contentLayout.spacing = 8f;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;
            var contentFitter = contentObj.GetComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scrollRect.content = contentRect;

            // Topic items
            for (int i = 0; i < candidates.Count; i++)
            {
                CreateTopicItem(contentObj.transform, candidates[i], i);
            }

            return scrollObj;
        }

        private void CreateTopicItem(Transform parent, TransferCandidate candidate, int index)
        {
            // Item container
            var itemObj = new GameObject($"TopicItem_{index}", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(Button));
            itemObj.transform.SetParent(parent, false);
            var itemBg = itemObj.GetComponent<Image>();
            itemBg.color = ItemBgSelected; // デフォルトON
            var itemLayout = itemObj.GetComponent<HorizontalLayoutGroup>();
            itemLayout.padding = new RectOffset(12, 12, 8, 8);
            itemLayout.spacing = 12f;
            itemLayout.childAlignment = TextAnchor.MiddleLeft;
            itemLayout.childForceExpandWidth = false;
            itemLayout.childForceExpandHeight = false;
            var itemLayoutElem = itemObj.AddComponent<LayoutElement>();
            itemLayoutElem.preferredHeight = 72f;
            itemLayoutElem.minHeight = 56f;

            // Toggle indicator (チェックマーク風)
            var toggleObj = new GameObject("Toggle", typeof(RectTransform), typeof(Image));
            toggleObj.transform.SetParent(itemObj.transform, false);
            var toggleImg = toggleObj.GetComponent<Image>();
            toggleImg.color = ToggleOnColor;
            var toggleLayout = toggleObj.AddComponent<LayoutElement>();
            toggleLayout.preferredWidth = 28f;
            toggleLayout.preferredHeight = 28f;
            toggleLayout.minWidth = 28f;

            // Text container (Title + Description 縦並び)
            var textContainer = new GameObject("TextContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
            textContainer.transform.SetParent(itemObj.transform, false);
            var textLayout = textContainer.GetComponent<VerticalLayoutGroup>();
            textLayout.spacing = 2f;
            textLayout.childForceExpandWidth = true;
            textLayout.childForceExpandHeight = false;
            var textLayoutElem = textContainer.AddComponent<LayoutElement>();
            textLayoutElem.flexibleWidth = 1f;

            // Title
            var titleObj = CreateTextObject(textContainer.transform, "Title", candidate.Title, 18f, TitleColor);
            titleObj.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

            // Description (あれば)
            if (!string.IsNullOrEmpty(candidate.Description))
            {
                var descObj = CreateTextObject(textContainer.transform, "Desc", candidate.Description, 14f, DescColor);
                var descTmp = descObj.GetComponent<TextMeshProUGUI>();
                descTmp.maxVisibleLines = 2;
                descTmp.overflowMode = TextOverflowModes.Ellipsis;
            }

            // Entry 登録
            var entry = new ToggleEntry
            {
                TopicId = candidate.TopicId,
                IsSelected = true,
                Background = itemBg,
                ToggleIndicator = toggleImg
            };
            m_Entries.Add(entry);

            // ボタンクリック (トグル)
            int capturedIndex = index;
            var btn = itemObj.GetComponent<Button>();
            btn.transition = Selectable.Transition.None;
            btn.onClick.AddListener(() => ToggleItem(capturedIndex));
        }

        private void ToggleItem(int index)
        {
            var entry = m_Entries[index];
            entry.IsSelected = !entry.IsSelected;
            m_Entries[index] = entry;

            // 視覚更新
            entry.ToggleIndicator.color = entry.IsSelected ? ToggleOnColor : ToggleOffColor;
            entry.Background.color = entry.IsSelected ? ItemBgSelected : ItemBgDefault;

            // DOTween パルス
            entry.ToggleIndicator.transform.DOScale(1.2f, 0.1f)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .OnComplete(() => entry.ToggleIndicator.transform.DOScale(1f, 0.1f).SetUpdate(true));

            UpdateCountLabel();
        }

        private void UpdateCountLabel()
        {
            if (m_CountLabel == null) return;
            int selected = 0;
            foreach (var e in m_Entries)
            {
                if (e.IsSelected) selected++;
            }
            m_CountLabel.text = $"{selected}/{m_Entries.Count} 選択中";
        }

        /// <summary>
        /// StopScenario 等からの強制クローズ。コールバックを発火せずUIを即時非表示にする。
        /// </summary>
        public void ForceClose()
        {
            DOTween.Kill(m_OverlayGroup);
            m_OnConfirm = null;
            gameObject.SetActive(false);
        }

        private void OnConfirmClicked()
        {
            var selectedIds = new List<string>();
            foreach (var e in m_Entries)
            {
                if (e.IsSelected)
                    selectedIds.Add(e.TopicId);
            }

            // フェードアウト → コールバック
            m_OverlayGroup.DOFade(0f, 0.2f)
                .SetEase(Ease.InCubic)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    m_OnConfirm?.Invoke(selectedIds);
                });
        }

        // === Helper methods ===

        private static GameObject CreateTextObject(Transform parent, string name, string text, float fontSize, Color color)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            var tmp = obj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.textWrappingMode = TMPro.TextWrappingModes.Normal;
            return obj;
        }

        private static GameObject CreateButton(Transform parent, string name, string label, Action onClick)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            obj.transform.SetParent(parent, false);
            obj.GetComponent<Image>().color = ButtonBgColor;
            obj.GetComponent<Button>().onClick.AddListener(() => onClick());

            var textObj = CreateTextObject(obj.transform, "Label", label, 20f, ButtonTextColor);
            textObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            var textRect = textObj.GetComponent<RectTransform>();
            StretchFull(textRect);

            return obj;
        }

        private static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
