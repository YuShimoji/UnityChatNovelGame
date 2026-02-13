using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace ProjectFoundPhone.MVP
{
    /// <summary>
    /// MVP縦切り用の自己完結型ゲームコントローラー。
    /// Title → Chat → Choice → End のフローをハードコードで実装。
    /// シーンに空のGameObjectを配置してアタッチするだけで動作する。
    /// </summary>
    public class MVPGameController : MonoBehaviour
    {
        #region Types
        private enum GameState { Title, Chat, Choice, End }

        [System.Serializable]
        private struct Line
        {
            public string speaker;
            public string text;
            public float delay;
        }
        #endregion

        #region Private Fields
        private GameState m_State = GameState.Title;
        private bool m_IsTransitioning = false;

        // UI References (created programmatically)
        private Canvas m_Canvas;
        private GameObject m_TitlePanel;
        private GameObject m_ChatPanel;
        private ScrollRect m_ChatScroll;
        private RectTransform m_ChatContent;
        private GameObject m_ChoicePanel;
        private GameObject m_EndPanel;
        private TextMeshProUGUI m_EndText;

        // Chat state
        private Coroutine m_ChatCoroutine;

        // Hardcoded conversation data
        private readonly List<Line> m_Conversation = new List<Line>
        {
            new Line { speaker = "???",    text = "...もしもし？",                     delay = 1.0f },
            new Line { speaker = "???",    text = "この番号で合ってるのかな...",       delay = 1.5f },
            new Line { speaker = "player", text = "誰ですか？",                       delay = 0.5f },
            new Line { speaker = "???",    text = "よかった、繋がった！",             delay = 1.0f },
            new Line { speaker = "???",    text = "お願い、助けて。私の名前は...",   delay = 2.0f },
        };

        // Branch A (信じる)
        private readonly List<Line> m_BranchA = new List<Line>
        {
            new Line { speaker = "player", text = "わかった、話を聞くよ。",           delay = 0.5f },
            new Line { speaker = "???",    text = "ありがとう...また連絡する。",     delay = 1.5f },
        };

        // Branch B (無視する)
        private readonly List<Line> m_BranchB = new List<Line>
        {
            new Line { speaker = "player", text = "悪いけど、関係ないよ。",           delay = 0.5f },
            new Line { speaker = "???",    text = "...そう。わかった。",             delay = 1.5f },
        };
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            SetupUI();
            TransitionTo(GameState.Title);
        }
        #endregion

        #region UI Setup
        private void SetupUI()
        {
            EnsureEventSystem();
            CreateCanvas();
            CreateTitlePanel();
            CreateChatPanel();
            CreateChoicePanel();
            CreateEndPanel();
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>();
            }
        }

        private void CreateCanvas()
        {
            GameObject canvasObj = new GameObject("MVPCanvas");
            canvasObj.transform.SetParent(transform);

            m_Canvas = canvasObj.AddComponent<Canvas>();
            m_Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            m_Canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
        }

        private void CreateTitlePanel()
        {
            m_TitlePanel = CreatePanel("TitlePanel", new Color(0.08f, 0.08f, 0.12f, 1f));

            // Title text
            CreateText(m_TitlePanel.transform, "TitleText",
                "Found Phone", 48, Color.white,
                new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.75f),
                TextAlignmentOptions.Center);

            // Subtitle
            CreateText(m_TitlePanel.transform, "SubtitleText",
                "― 見つけたスマホの物語 ―", 20, new Color(0.6f, 0.6f, 0.7f),
                new Vector2(0.1f, 0.45f), new Vector2(0.9f, 0.52f),
                TextAlignmentOptions.Center);

            // Start button
            CreateButton(m_TitlePanel.transform, "StartButton",
                "はじめる", new Color(0.2f, 0.5f, 1f),
                new Vector2(0.25f, 0.2f), new Vector2(0.75f, 0.28f),
                OnStartClicked);
        }

        private void CreateChatPanel()
        {
            m_ChatPanel = CreatePanel("ChatPanel", new Color(0.05f, 0.05f, 0.08f, 1f));

            // Header
            CreateText(m_ChatPanel.transform, "ChatHeader",
                "??? からのメッセージ", 18, new Color(0.7f, 0.7f, 0.8f),
                new Vector2(0.05f, 0.93f), new Vector2(0.95f, 0.98f),
                TextAlignmentOptions.Center);

            // Scroll view for messages
            GameObject scrollObj = new GameObject("ChatScrollView", typeof(RectTransform));
            scrollObj.transform.SetParent(m_ChatPanel.transform, false);
            SetAnchors(scrollObj, new Vector2(0.03f, 0.03f), new Vector2(0.97f, 0.92f));

            // Viewport
            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            viewport.transform.SetParent(scrollObj.transform, false);
            StretchToFill(viewport.GetComponent<RectTransform>());
            Image vpImg = viewport.GetComponent<Image>();
            vpImg.color = new Color(0.1f, 0.1f, 0.14f, 0.5f);

            // Content
            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(viewport.transform, false);
            m_ChatContent = content.GetComponent<RectTransform>();
            m_ChatContent.anchorMin = new Vector2(0, 1);
            m_ChatContent.anchorMax = new Vector2(1, 1);
            m_ChatContent.pivot = new Vector2(0.5f, 1);
            m_ChatContent.sizeDelta = new Vector2(0, 0);

            VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
            vlg.childControlHeight = false;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.spacing = 12;
            vlg.padding = new RectOffset(16, 16, 16, 16);

            ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // ScrollRect
            m_ChatScroll = scrollObj.AddComponent<ScrollRect>();
            m_ChatScroll.content = m_ChatContent;
            m_ChatScroll.viewport = viewport.GetComponent<RectTransform>();
            m_ChatScroll.horizontal = false;
            m_ChatScroll.vertical = true;
            m_ChatScroll.scrollSensitivity = 30;
            m_ChatScroll.movementType = ScrollRect.MovementType.Clamped;
        }

        private void CreateChoicePanel()
        {
            m_ChoicePanel = CreatePanel("ChoicePanel", new Color(0, 0, 0, 0.7f));

            CreateText(m_ChoicePanel.transform, "ChoicePrompt",
                "どうする？", 24, Color.white,
                new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.65f),
                TextAlignmentOptions.Center);

            // Choice A
            CreateButton(m_ChoicePanel.transform, "ChoiceA",
                "信じる", new Color(0.2f, 0.6f, 0.3f),
                new Vector2(0.15f, 0.38f), new Vector2(0.85f, 0.48f),
                () => OnChoiceSelected(0));

            // Choice B
            CreateButton(m_ChoicePanel.transform, "ChoiceB",
                "無視する", new Color(0.6f, 0.2f, 0.2f),
                new Vector2(0.15f, 0.25f), new Vector2(0.85f, 0.35f),
                () => OnChoiceSelected(1));
        }

        private void CreateEndPanel()
        {
            m_EndPanel = CreatePanel("EndPanel", new Color(0.02f, 0.02f, 0.05f, 1f));

            m_EndText = CreateText(m_EndPanel.transform, "EndMessage",
                "", 28, Color.white,
                new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.65f),
                TextAlignmentOptions.Center);

            CreateText(m_EndPanel.transform, "EndCredits",
                "Chapter 1 ― つづく", 20, new Color(0.5f, 0.5f, 0.6f),
                new Vector2(0.1f, 0.4f), new Vector2(0.9f, 0.48f),
                TextAlignmentOptions.Center);

            CreateButton(m_EndPanel.transform, "RestartButton",
                "はじめからやり直す", new Color(0.3f, 0.3f, 0.4f),
                new Vector2(0.2f, 0.15f), new Vector2(0.8f, 0.23f),
                OnRestartClicked);
        }
        #endregion

        #region State Management
        private void TransitionTo(GameState newState)
        {
            m_State = newState;

            m_TitlePanel.SetActive(newState == GameState.Title);
            m_ChatPanel.SetActive(newState == GameState.Chat || newState == GameState.Choice);
            m_ChoicePanel.SetActive(newState == GameState.Choice);
            m_EndPanel.SetActive(newState == GameState.End);
        }
        #endregion

        #region Button Handlers
        private void OnStartClicked()
        {
            if (m_IsTransitioning) return;
            m_IsTransitioning = true;

            // Clear previous chat content
            ClearChatContent();

            TransitionTo(GameState.Chat);
            m_ChatCoroutine = StartCoroutine(RunChatSequence());
        }

        private void OnChoiceSelected(int choiceIndex)
        {
            if (m_IsTransitioning) return;
            m_IsTransitioning = true;

            TransitionTo(GameState.Chat);
            StartCoroutine(RunBranchSequence(choiceIndex));
        }

        private void OnRestartClicked()
        {
            if (m_IsTransitioning) return;
            m_IsTransitioning = true;

            ClearChatContent();
            TransitionTo(GameState.Title);
            m_IsTransitioning = false;
        }

public void ForceTransitionToChat()
        {
            if (m_IsTransitioning) return;
            OnStartClicked();
        }

        public void ForceTransitionToChoice()
        {
            if (m_IsTransitioning) return;
            TransitionTo(GameState.Choice);
        }

        public void ForceTransitionToEnd()
        {
            if (m_IsTransitioning) return;
            m_EndText.text = "テストエンディング";
            TransitionTo(GameState.End);
        }

        #endregion

        #region Chat Flow
        private IEnumerator RunChatSequence()
        {
            for (int i = 0; i < m_Conversation.Count; i++)
            {
                Line line = m_Conversation[i];

                // Typing delay
                yield return new WaitForSeconds(line.delay);

                // Add message bubble
                AddBubble(line.speaker, line.text);
                ScrollToBottom();
            }

            // Brief pause before showing choices
            yield return new WaitForSeconds(1.0f);

            m_IsTransitioning = false;
            TransitionTo(GameState.Choice);
        }

        private IEnumerator RunBranchSequence(int choiceIndex)
        {
            List<Line> branch = choiceIndex == 0 ? m_BranchA : m_BranchB;

            for (int i = 0; i < branch.Count; i++)
            {
                Line line = branch[i];
                yield return new WaitForSeconds(line.delay);
                AddBubble(line.speaker, line.text);
                ScrollToBottom();
            }

            yield return new WaitForSeconds(1.5f);

            string endMessage = choiceIndex == 0
                ? "あなたは見知らぬ相手を信じた。\n物語は動き出す..."
                : "あなたは電話を切った。\nだが、何かが引っかかる...";

            m_EndText.text = endMessage;
            m_IsTransitioning = false;
            TransitionTo(GameState.End);
        }
        #endregion

        #region Chat Helpers
        private void AddBubble(string speaker, string text)
        {
            bool isPlayer = speaker == "player";
            float bubbleWidth = 0.7f;

            GameObject bubble = new GameObject("Bubble", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            bubble.transform.SetParent(m_ChatContent, false);

            RectTransform bubbleRT = bubble.GetComponent<RectTransform>();
            bubbleRT.sizeDelta = new Vector2(0, 0);

            Image bubbleBG = bubble.GetComponent<Image>();
            bubbleBG.color = isPlayer
                ? new Color(0.15f, 0.35f, 0.7f, 0.9f)
                : new Color(0.2f, 0.2f, 0.25f, 0.9f);

            // Horizontal layout to control alignment
            HorizontalLayoutGroup hlg = bubble.AddComponent<HorizontalLayoutGroup>();
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;
            hlg.padding = new RectOffset(16, 16, 10, 10);

            ContentSizeFitter bubbleCSF = bubble.AddComponent<ContentSizeFitter>();
            bubbleCSF.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            bubbleCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Alignment via LayoutElement on bubble
            LayoutElement bubbleLE = bubble.GetComponent<LayoutElement>();
            // Use preferredWidth to constrain bubble width
            float maxWidth = m_ChatContent.rect.width * bubbleWidth;
            if (maxWidth <= 0) maxWidth = 600f;

            // Text child
            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            textObj.transform.SetParent(bubble.transform, false);

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 22;
            tmp.color = Color.white;
            tmp.enableWordWrapping = true;
            tmp.overflowMode = TextOverflowModes.Overflow;

            LayoutElement textLE = textObj.AddComponent<LayoutElement>();
            textLE.preferredWidth = maxWidth - 32;
            textLE.flexibleWidth = 0;

            // Speaker label (for NPCs)
            if (!isPlayer)
            {
                tmp.text = $"<color=#88aaff><size=16>{speaker}</size></color>\n{text}";
            }

            // Alignment: player right, NPC left
            bubbleRT.pivot = isPlayer ? new Vector2(1, 1) : new Vector2(0, 1);

            // Use LayoutGroup padding to push bubble to left/right
            // We achieve alignment by setting the LayoutElement on the bubble
            if (isPlayer)
            {
                bubbleLE.minWidth = 0;
            }
        }

        private void ClearChatContent()
        {
            if (m_ChatContent == null) return;
            for (int i = m_ChatContent.childCount - 1; i >= 0; i--)
            {
                Destroy(m_ChatContent.GetChild(i).gameObject);
            }
        }

        private void ScrollToBottom()
        {
            StartCoroutine(ScrollToBottomNextFrame());
        }

        private IEnumerator ScrollToBottomNextFrame()
        {
            yield return null;
            yield return null;
            if (m_ChatScroll != null)
            {
                m_ChatScroll.verticalNormalizedPosition = 0f;
            }
        }
        #endregion

        #region UI Helpers
        private GameObject CreatePanel(string name, Color bgColor)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(m_Canvas.transform, false);
            StretchToFill(panel.GetComponent<RectTransform>());
            panel.GetComponent<Image>().color = bgColor;
            panel.SetActive(false);
            return panel;
        }

        private TextMeshProUGUI CreateText(Transform parent, string name,
            string text, float fontSize, Color color,
            Vector2 anchorMin, Vector2 anchorMax,
            TextAlignmentOptions alignment)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            SetAnchors(obj, anchorMin, anchorMax);

            TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.enableWordWrapping = true;
            return tmp;
        }

        private Button CreateButton(Transform parent, string name,
            string label, Color bgColor,
            Vector2 anchorMin, Vector2 anchorMax,
            UnityEngine.Events.UnityAction onClick)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent, false);
            SetAnchors(btnObj, anchorMin, anchorMax);

            Image btnImg = btnObj.GetComponent<Image>();
            btnImg.color = bgColor;

            // Button label
            GameObject labelObj = new GameObject("Label", typeof(RectTransform));
            labelObj.transform.SetParent(btnObj.transform, false);
            StretchToFill(labelObj.GetComponent<RectTransform>());

            TextMeshProUGUI tmp = labelObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 24;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.verticalAlignment = VerticalAlignmentOptions.Middle;

            Button btn = btnObj.GetComponent<Button>();
            btn.targetGraphic = btnImg;
            btn.onClick.AddListener(onClick);

            // Hover/pressed color
            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color(bgColor.r + 0.1f, bgColor.g + 0.1f, bgColor.b + 0.1f, 1f);
            colors.pressedColor = new Color(bgColor.r - 0.1f, bgColor.g - 0.1f, bgColor.b - 0.1f, 1f);
            btn.colors = colors;

            return btn;
        }

        private static void SetAnchors(GameObject obj, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void StretchToFill(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        #endregion
    }
}
