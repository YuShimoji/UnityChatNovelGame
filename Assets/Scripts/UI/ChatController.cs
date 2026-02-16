using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using System.Collections.Generic;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// チャット画面のUI制御を行うコントローラー
    /// ScrollRect + VerticalLayoutGroup + ContentSizeFitterを使用したメッセージ表示システム
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ChatController : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private ScrollRect m_ScrollRect;
        [SerializeField] private VerticalLayoutGroup m_LayoutGroup;
        [SerializeField] private GameObject m_MessageBubblePrefab;
        [SerializeField] private GameObject m_TypingIndicator;
        [SerializeField] private TMP_InputField m_InputField;
        [SerializeField] private Button m_SendButton;
        [SerializeField] private float m_AutoScrollThreshold = 0.1f; // 自動スクロールを実行する閾値（0.0-1.0）

        [Header("Image Bubble Settings")]
        [SerializeField] private GameObject m_ImageBubblePrefab;
        [SerializeField] private float m_ImageMaxWidth = 300f;
        [SerializeField] private float m_ImageMaxHeight = 200f;

        [Header("Choice Settings")]
        [SerializeField] private GameObject m_ChoiceButtonPrefab;
        [SerializeField] private Transform m_ChoiceContainer;


        private bool m_IsUserScrolling = false;
        private float m_LastScrollPosition = 1.0f;

        private bool m_AutoScrollScheduled = false;
        private Tween m_ScrollTween;

        private GameObject m_RuntimeChoiceButtonTemplate;
        private GameObject m_RuntimeImageBubbleTemplate;
        private TMP_InputField m_RuntimeInputField;
        private Button m_RuntimeSendButton;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            if (m_SendButton != null)
            {
                m_SendButton.onClick.AddListener(OnSubmit);
            }

            if (m_InputField != null)
            {
                m_InputField.onSubmit.AddListener((text) => OnSubmit());
            }

            // スクロール位置の変更をイベントで監視（毎フレームUpdate不要）
            if (m_ScrollRect != null)
            {
                m_ScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 必要なコンポーネントの初期化
        /// </summary>
        private void InitializeComponents()
        {
            if (m_ScrollRect == null)
            {
                m_ScrollRect = GetComponent<ScrollRect>();
            }

            if (m_LayoutGroup == null && m_ScrollRect != null && m_ScrollRect.content != null)
            {
                m_LayoutGroup = m_ScrollRect.content.GetComponent<VerticalLayoutGroup>();
            }

            // m_MessageBubblePrefab、m_TypingIndicatorのnullチェックと警告
            if (m_MessageBubblePrefab == null)
            {
                Debug.LogWarning("ChatController: m_MessageBubblePrefab is not assigned. Message bubbles cannot be created.");
            }

            if (m_TypingIndicator == null)
            {
                Debug.LogWarning("ChatController: m_TypingIndicator is not assigned. Typing indicator will not be displayed.");
            }

            EnsureChoiceUIElements();
            EnsureImageBubbleTemplate();
            EnsureInputControls();
        }

        /// <summary>
        /// ScrollRect.onValueChanged イベントハンドラ
        /// ユーザーが手動でスクロールしているかを検知する
        /// </summary>
        private void OnScrollValueChanged(Vector2 scrollPosition)
        {
            float verticalPos = scrollPosition.y;

            // スクロール位置が下から一定以上離れている場合、ユーザーが過去ログを見ていると判定
            if (verticalPos < (1.0f - m_AutoScrollThreshold))
            {
                m_IsUserScrolling = true;
            }
            // スクロール位置が最下部に近い場合、ユーザーは最新メッセージを見ている
            else if (verticalPos >= 0.99f)
            {
                m_IsUserScrolling = false;
            }

            m_LastScrollPosition = verticalPos;
        }

        /// <summary>
        /// バブルGameObjectにキャラクターベースのレイアウトとテーマカラーを適用する
        /// </summary>
        /// <param name="bubble">設定対象のバブルGameObject</param>
        /// <param name="charID">キャラクターID</param>
        private void ConfigureBubble(GameObject bubble, string charID)
        {
            bool isPlayer = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.IsPlayer(charID)
                : charID == "player";

            Color themeColor = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.GetThemeColor(charID)
                : (isPlayer ? new Color(0.2f, 0.6f, 1.0f) : new Color(0.85f, 0.85f, 0.85f));

            // 右寄せ（プレイヤー）/ 左寄せ（NPC）を設定
            RectTransform rectTransform = bubble.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float anchorX = isPlayer ? 1.0f : 0.0f;
                rectTransform.anchorMin = new Vector2(anchorX, 1.0f);
                rectTransform.anchorMax = new Vector2(anchorX, 1.0f);
                rectTransform.pivot = new Vector2(anchorX, 1.0f);
            }

            // バブル背景にテーマカラーを適用
            Image bubbleBackground = bubble.GetComponent<Image>();
            if (bubbleBackground != null)
            {
                bubbleBackground.color = themeColor;
            }
        }

        /// <summary>
        /// メッセージバブルのPrefabをインスタンス化
        /// </summary>
        /// <param name="charID">キャラクターID（自分/相手の判定に使用）</param>
        /// <param name="text">メッセージテキスト</param>
        /// <returns>生成されたGameObject</returns>
        private GameObject CreateMessageBubble(string charID, string text)
        {
            if (m_MessageBubblePrefab == null)
            {
                Debug.LogError("ChatController: Cannot create message bubble. Prefab is not assigned.");
                return null;
            }

            if (m_ScrollRect == null || m_ScrollRect.content == null)
            {
                Debug.LogError("ChatController: Cannot create message bubble. ScrollRect or content is not assigned.");
                return null;
            }

            // Prefabからインスタンスを生成
            GameObject messageBubble = Instantiate(m_MessageBubblePrefab, m_ScrollRect.content);

            // プレイヤー判定・テーマカラー・配置を共通処理で設定
            ConfigureBubble(messageBubble, charID);

            // TextMeshProコンポーネントにtextを設定
            TextMeshProUGUI textComponent = messageBubble.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = text;
            }
            else
            {
                Debug.LogWarning("ChatController: TextMeshProUGUI component not found in message bubble prefab.");
            }

            // ContentSizeFitterで高さを自動調整
            ContentSizeFitter sizeFitter = messageBubble.GetComponent<ContentSizeFitter>();
            if (sizeFitter != null)
            {
                sizeFitter.SetLayoutVertical();
            }

            return messageBubble;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 新しいメッセージをチャットに追加
        /// </summary>
        /// <param name="charID">キャラクターID（例: "player", "npc_001"）</param>
        /// <param name="text">メッセージテキスト</param>
        public void AddMessage(string charID, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("ChatController: Attempted to add empty message.");
                return;
            }

            // メッセージバブルの生成と追加

            // CreateMessageBubble()でメッセージバブルを生成（既にcontentの子として追加済み）
            GameObject messageBubble = CreateMessageBubble(charID, text);
            if (messageBubble == null)
            {
                return;
            }

            // ユーザーが過去ログを見ていない場合のみAutoScroll()を実行
            if (!m_IsUserScrolling)
            {
                AutoScroll();
            }
        }

        /// <summary>
        /// 画像メッセージをチャットに追加
        /// </summary>
        /// <param name="charID">キャラクターID</param>
        /// <param name="imageSprite">表示する画像Sprite</param>
        public void AddImageMessage(string charID, Sprite imageSprite)
        {
            if (imageSprite == null)
            {
                Debug.LogWarning("ChatController: Attempted to add image message with null sprite.");
                return;
            }

            if (m_ScrollRect == null || m_ScrollRect.content == null)
            {
                Debug.LogError("ChatController: Cannot create image bubble. ScrollRect or content is not assigned.");
                return;
            }

            // ImageBubblePrefabが設定されていない場合はテキストバブルにフォールバック
            GameObject prefab = m_ImageBubblePrefab != null ? m_ImageBubblePrefab : m_MessageBubblePrefab;
            if (prefab == null)
            {
                Debug.LogError("ChatController: No prefab available for image message.");
                return;
            }

            GameObject imageBubble = Instantiate(prefab, m_ScrollRect.content);
            imageBubble.SetActive(true);

            // プレイヤー判定・テーマカラー・配置を共通処理で設定
            ConfigureBubble(imageBubble, charID);

            // 画像を表示するImageコンポーネントを検索して設定
            // ImageBubblePrefab内に "ImageContent" という名前の子オブジェクトを想定
            Transform imageContentTransform = imageBubble.transform.Find("ImageContent");
            Image imageContent = imageContentTransform != null
                ? imageContentTransform.GetComponent<Image>()
                : null;

            if (imageContent == null)
            {
                // 子階層から最初のImage（背景以外）を探す
                Image[] images = imageBubble.GetComponentsInChildren<Image>();
                foreach (var img in images)
                {
                    if (img.gameObject != imageBubble)
                    {
                        imageContent = img;
                        break;
                    }
                }
            }

            if (imageContent != null)
            {
                imageContent.sprite = imageSprite;
                imageContent.preserveAspect = true;

                // 画像サイズを制限
                RectTransform imgRect = imageContent.GetComponent<RectTransform>();
                if (imgRect != null)
                {
                    float aspectRatio = (float)imageSprite.texture.width / imageSprite.texture.height;
                    float width = Mathf.Min(m_ImageMaxWidth, imageSprite.texture.width);
                    float height = width / aspectRatio;
                    if (height > m_ImageMaxHeight)
                    {
                        height = m_ImageMaxHeight;
                        width = height * aspectRatio;
                    }
                    imgRect.sizeDelta = new Vector2(width, height);
                }
            }
            else
            {
                // フォールバック: テキストとして画像名を表示
                TextMeshProUGUI textComponent = imageBubble.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = $"[Image: {imageSprite.name}]";
                }
            }

            if (!m_IsUserScrolling)
            {
                AutoScroll();
            }
        }

        /// <summary>
        /// システムメッセージ（通知）をチャットに追加
        /// キャラクターの発言ではなく、中央揃えのグレーテキストで表示する
        /// 例: 「グループに参加しました」「新しいトピックが解放されました」
        /// </summary>
        /// <param name="text">システムメッセージのテキスト</param>
        public void AddSystemMessage(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (m_ScrollRect == null || m_ScrollRect.content == null)
            {
                Debug.LogError("ChatController: Cannot create system message. ScrollRect or content is not assigned.");
                return;
            }

            if (m_MessageBubblePrefab == null)
            {
                Debug.LogError("ChatController: Cannot create system message. MessageBubblePrefab is not assigned.");
                return;
            }

            GameObject systemBubble = Instantiate(m_MessageBubblePrefab, m_ScrollRect.content);

            // 中央揃え
            RectTransform rectTransform = systemBubble.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 1.0f);
                rectTransform.anchorMax = new Vector2(0.5f, 1.0f);
                rectTransform.pivot = new Vector2(0.5f, 1.0f);
            }

            // 背景を半透明グレーに設定
            Image bubbleBackground = systemBubble.GetComponent<Image>();
            if (bubbleBackground != null)
            {
                bubbleBackground.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
            }

            // テキストを中央揃え・小さめフォントで設定
            TextMeshProUGUI textComponent = systemBubble.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = text;
                textComponent.alignment = TextAlignmentOptions.Center;
                textComponent.fontStyle = FontStyles.Italic;
                textComponent.fontSize = textComponent.fontSize * 0.85f;
                textComponent.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            }

            if (!m_IsUserScrolling)
            {
                AutoScroll();
            }
        }

        /// <summary>
        /// 入力欄と送信ボタンの有効/無効を切り替える
        /// ScenarioManagerの入力ロック連動で使用
        /// </summary>
        /// <param name="enabled">有効にする場合true</param>
        public void SetInputEnabled(bool enabled)
        {
            if (m_InputField != null)
            {
                m_InputField.interactable = enabled;
            }
            if (m_SendButton != null)
            {
                m_SendButton.interactable = enabled;
            }
        }

        /// <summary>
        /// タイピングインジケーターの表示/非表示を切り替え
        /// </summary>
        /// <param name="show">表示する場合true</param>
        public void ShowTypingIndicator(bool show)
        {
            if (m_TypingIndicator != null)
            {
                m_TypingIndicator.SetActive(show);
                
                if (show)
                {
                    // 常に最後尾に表示
                    m_TypingIndicator.transform.SetAsLastSibling();
                    AutoScroll();
                }
            }
        }



        /// <summary>
        /// 選択肢を表示する
        /// </summary>
        /// <param name="options">選択肢のテキストリスト</param>
        /// <param name="onSelected">選択時のコールバック (index)</param>
        public void ShowChoices(List<string> options, System.Action<int> onSelected)
        {
            if (m_ChoiceButtonPrefab == null || m_ChoiceContainer == null)
            {
                Debug.LogError("ChatController: ChoiceButtonPrefab or ChoiceContainer is not assigned.");
                return;
            }

            // 既存の選択肢をクリア
            HideChoices();

            m_ChoiceContainer.gameObject.SetActive(true);

            // 入力欄を非表示にする（オプション）
            if (m_InputField != null) m_InputField.gameObject.SetActive(false);
            if (m_SendButton != null) m_SendButton.gameObject.SetActive(false);

            for (int i = 0; i < options.Count; i++)
            {
                GameObject buttonObj = Instantiate(m_ChoiceButtonPrefab, m_ChoiceContainer);
                buttonObj.SetActive(true);
                buttonObj.name = "Choice" + (char)('A' + i); // For Test Automation (MVPTestHelper)
                
                // ボタンのテキスト設定
                TextMeshProUGUI btnText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.text = options[i];
                }

                // クリックイベント設定
                Button btn = buttonObj.GetComponent<Button>();
                if (btn != null)
                {
                    ChoiceButtonHandler handler = buttonObj.GetComponent<ChoiceButtonHandler>();
                    if (handler == null)
                    {
                        handler = buttonObj.AddComponent<ChoiceButtonHandler>();
                    }
                    handler.Initialize(this, i, onSelected);
                    btn.onClick.AddListener(handler.OnClick);
                }
            }

            // 選択肢が表示されたら最下部へスクロール
            AutoScroll();
        }

        /// <summary>
        /// 選択肢を非表示にする
        /// </summary>
        public void HideChoices()
        {
            if (m_ChoiceContainer != null)
            {
                // 子要素を全て削除
                foreach (Transform child in m_ChoiceContainer)
                {
                    Destroy(child.gameObject);
                }
                m_ChoiceContainer.gameObject.SetActive(false);
            }

            // 入力欄を再表示
            if (m_InputField != null) m_InputField.gameObject.SetActive(true);
            if (m_SendButton != null) m_SendButton.gameObject.SetActive(true);
        }

        public void AutoScroll()
        {
            if (m_ScrollRect == null || m_IsUserScrolling)
            {
                return;
            }

            if (m_AutoScrollScheduled)
            {
                return;
            }

            m_AutoScrollScheduled = true;

            // Canvasの更新を待ってからスクロールするためにコルーチンか遅延実行を使うのが一般的だが、
            // ここでは簡易的にDOTweenで遅延させる
            Invoke(nameof(PerformAutoScroll), 0.1f);
        }

        private void PerformAutoScroll()
        {
            m_AutoScrollScheduled = false;

            if (m_ScrollRect == null || m_IsUserScrolling)
            {
                return;
            }

            if (m_ScrollTween != null && m_ScrollTween.IsActive())
            {
                m_ScrollTween.Kill(false);
            }

            m_ScrollTween = DOTween.To(
                () => m_ScrollRect.verticalNormalizedPosition,
                x => m_ScrollRect.verticalNormalizedPosition = x,
                0.0f,
                0.3f
            ).OnComplete(() =>
            {
                m_LastScrollPosition = 0.0f;
            });
        }

        private void OnDisable()
        {
            if (m_AutoScrollScheduled)
            {
                CancelInvoke(nameof(PerformAutoScroll));
                m_AutoScrollScheduled = false;
            }

            if (m_ScrollTween != null && m_ScrollTween.IsActive())
            {
                m_ScrollTween.Kill(false);
            }
        }

        private void EnsureChoiceUIElements()
        {
            if (m_ChoiceContainer == null)
            {
                m_ChoiceContainer = CreateRuntimeChoiceContainer();
            }

            if (m_ChoiceButtonPrefab == null)
            {
                m_RuntimeChoiceButtonTemplate = CreateChoiceButtonTemplate();
                m_ChoiceButtonPrefab = m_RuntimeChoiceButtonTemplate;
            }
        }

        private void EnsureImageBubbleTemplate()
        {
            if (m_ImageBubblePrefab == null && m_RuntimeImageBubbleTemplate == null)
            {
                m_RuntimeImageBubbleTemplate = CreateImageBubbleTemplate();
                m_ImageBubblePrefab = m_RuntimeImageBubbleTemplate;
            }
        }

        private void EnsureInputControls()
        {
            if (m_InputField != null && m_SendButton != null)
            {
                return;
            }

            const float footerHeight = 160f;
            Transform parentForFooter = transform.parent != null ? transform.parent : transform;
            GameObject footer = new GameObject("AutoFooter", typeof(RectTransform), typeof(Image));
            AssignUILayer(footer);
            footer.transform.SetParent(parentForFooter, false);
            footer.transform.SetAsLastSibling();

            RectTransform footerRect = footer.GetComponent<RectTransform>();
            footerRect.anchorMin = new Vector2(0f, 0f);
            footerRect.anchorMax = new Vector2(1f, 0f);
            footerRect.pivot = new Vector2(0.5f, 0f);
            footerRect.offsetMin = new Vector2(0f, 0f);
            footerRect.offsetMax = new Vector2(0f, footerHeight);

            Image footerBg = footer.GetComponent<Image>();
            footerBg.color = new Color(0.09f, 0.09f, 0.11f, 0.95f);

            m_RuntimeInputField = CreateRuntimeInputField(footer.transform);
            m_RuntimeSendButton = CreateRuntimeSendButton(footer.transform);

            if (m_InputField == null)
            {
                m_InputField = m_RuntimeInputField;
            }

            if (m_SendButton == null)
            {
                m_SendButton = m_RuntimeSendButton;
            }

            if (m_ScrollRect != null && m_ScrollRect.viewport != null)
            {
                RectTransform viewport = m_ScrollRect.viewport;
                Vector2 offsetMin = viewport.offsetMin;
                offsetMin.y = Mathf.Max(offsetMin.y, footerHeight);
                viewport.offsetMin = offsetMin;
            }
        }

        private Transform CreateRuntimeChoiceContainer()
        {
            GameObject container = new GameObject("AutoChoiceContainer", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            container.transform.SetParent(transform, false);
            RectTransform rect = container.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.05f, 0f);
            rect.anchorMax = new Vector2(0.95f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.offsetMin = new Vector2(0f, 24f);
            rect.offsetMax = new Vector2(0f, 260f);
            container.transform.SetAsLastSibling();

            Image background = container.GetComponent<Image>();
            background.color = new Color(0.07f, 0.07f, 0.09f, 0.92f);

            VerticalLayoutGroup layoutGroup = container.GetComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 12f;
            layoutGroup.padding = new RectOffset(20, 20, 20, 20);
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;

            ContentSizeFitter fitter = container.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            container.SetActive(false);
            return container.transform;
        }

        private GameObject CreateChoiceButtonTemplate()
        {
            GameObject template = new GameObject("AutoChoiceButtonTemplate", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            template.transform.SetParent(transform, false);
            template.SetActive(false);

            Image buttonBackground = template.GetComponent<Image>();
            buttonBackground.color = new Color(0.18f, 0.18f, 0.22f, 0.95f);

            Button button = template.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = buttonBackground;

            LayoutElement layout = template.GetComponent<LayoutElement>();
            layout.minHeight = 60f;
            layout.preferredHeight = 70f;
            layout.flexibleWidth = 1f;

            RectTransform rect = template.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = new Vector2(0f, 0f);
            rect.offsetMax = new Vector2(0f, 0f);

            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            textObj.transform.SetParent(template.transform, false);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(20f, 10f);
            textRect.offsetMax = new Vector2(-20f, -10f);

            TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = "Choice";
            label.enableAutoSizing = true;
            label.fontSizeMin = 20f;
            label.fontSizeMax = 36f;
            label.alignment = TextAlignmentOptions.Midline;
            label.color = Color.white;
            label.raycastTarget = false;
            if (TMP_Settings.defaultFontAsset != null)
            {
                label.font = TMP_Settings.defaultFontAsset;
            }

            return template;
        }

        private GameObject CreateImageBubbleTemplate()
        {
            GameObject template = new GameObject("AutoImageBubblePrefab", typeof(RectTransform), typeof(Image), typeof(ContentSizeFitter));
            template.transform.SetParent(transform, false);
            template.SetActive(false);

            Image bubbleBackground = template.GetComponent<Image>();
            bubbleBackground.color = Color.white;
            bubbleBackground.raycastTarget = true;

            ContentSizeFitter sizeFitter = template.GetComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            GameObject imageContainer = new GameObject("ImageContent", typeof(RectTransform), typeof(Image));
            imageContainer.transform.SetParent(template.transform, false);
            RectTransform imageRect = imageContainer.GetComponent<RectTransform>();
            imageRect.anchorMin = Vector2.zero;
            imageRect.anchorMax = Vector2.one;
            imageRect.offsetMin = new Vector2(12f, 12f);
            imageRect.offsetMax = new Vector2(-12f, -12f);

            Image contentImage = imageContainer.GetComponent<Image>();
            contentImage.preserveAspect = true;

            return template;
        }

        private TMP_InputField CreateRuntimeInputField(Transform parent)
        {
            GameObject inputObj = new GameObject("InputField", typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
            AssignUILayer(inputObj);
            inputObj.transform.SetParent(parent, false);

            RectTransform inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0f, 0f);
            inputRect.anchorMax = new Vector2(1f, 1f);
            inputRect.offsetMin = new Vector2(20f, 20f);
            inputRect.offsetMax = new Vector2(-150f, -20f);

            Image inputBg = inputObj.GetComponent<Image>();
            inputBg.color = new Color(0.16f, 0.16f, 0.2f, 0.95f);
            inputBg.raycastTarget = true;

            TMP_InputField inputField = inputObj.GetComponent<TMP_InputField>();
            inputField.textViewport = CreateInputTextViewport(inputObj.transform, out TextMeshProUGUI textComponent, out TextMeshProUGUI placeholderComponent);
            inputField.textComponent = textComponent;
            inputField.placeholder = placeholderComponent;
            inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
            inputField.characterLimit = 0;

            return inputField;
        }

        private RectTransform CreateInputTextViewport(Transform parent, out TextMeshProUGUI textComponent, out TextMeshProUGUI placeholderComponent)
        {
            GameObject textArea = new GameObject("TextArea", typeof(RectTransform));
            AssignUILayer(textArea);
            textArea.transform.SetParent(parent, false);
            RectTransform textAreaRect = textArea.GetComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(10f, 8f);
            textAreaRect.offsetMax = new Vector2(-10f, -8f);

            GameObject viewportObj = new GameObject("TextViewport", typeof(RectTransform), typeof(RectMask2D));
            AssignUILayer(viewportObj);
            viewportObj.transform.SetParent(textArea.transform, false);
            RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            placeholderComponent = CreateTMPText(viewportObj.transform, "Placeholder", new Color(0.65f, 0.65f, 0.7f, 0.85f), FontStyles.Italic);
            placeholderComponent.text = "Type a message...";

            textComponent = CreateTMPText(viewportObj.transform, "Text", Color.white, FontStyles.Normal);
            textComponent.enableWordWrapping = true;
            textComponent.text = string.Empty;

            return viewportRect;
        }

        private Button CreateRuntimeSendButton(Transform parent)
        {
            GameObject buttonObj = new GameObject("SendButton", typeof(RectTransform), typeof(Image), typeof(Button));
            AssignUILayer(buttonObj);
            buttonObj.transform.SetParent(parent, false);

            RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1f, 0f);
            btnRect.anchorMax = new Vector2(1f, 1f);
            btnRect.pivot = new Vector2(1f, 0.5f);
            btnRect.offsetMin = new Vector2(-130f, 20f);
            btnRect.offsetMax = new Vector2(-20f, -20f);

            Image btnImage = buttonObj.GetComponent<Image>();
            btnImage.color = new Color(0.25f, 0.55f, 1.0f, 1.0f);
            btnImage.raycastTarget = true;

            Button button = buttonObj.GetComponent<Button>();
            button.targetGraphic = btnImage;
            button.transition = Selectable.Transition.ColorTint;

            TextMeshProUGUI label = CreateTMPText(buttonObj.transform, "Text", Color.white, FontStyles.Bold);
            RectTransform labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 20f;
            label.fontSizeMax = 40f;
            label.text = "Send";

            return button;
        }

        private TextMeshProUGUI CreateTMPText(Transform parent, string name, Color color, FontStyles fontStyle)
        {
            GameObject textObj = new GameObject(name, typeof(RectTransform));
            AssignUILayer(textObj);
            textObj.transform.SetParent(parent, false);

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.color = color;
            tmp.fontSize = 30f;
            tmp.fontStyle = fontStyle;
            tmp.text = string.Empty;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 16f;
            tmp.fontSizeMax = 48f;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.raycastTarget = false;

            if (TMP_Settings.defaultFontAsset != null)
            {
                tmp.font = TMP_Settings.defaultFontAsset;
            }

            return tmp;
        }

        private void AssignUILayer(GameObject go)
        {
            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer >= 0)
            {
                go.layer = uiLayer;
            }
        }

        private sealed class ChoiceButtonHandler : MonoBehaviour
        {
            private ChatController m_Owner;
            private int m_Index;
            private Action<int> m_OnSelected;

            public void Initialize(ChatController owner, int index, Action<int> onSelected)
            {
                m_Owner = owner;
                m_Index = index;
                m_OnSelected = onSelected;
            }

            public void OnClick()
            {
                if (m_Owner != null)
                {
                    m_Owner.HideChoices();
                }
                m_OnSelected?.Invoke(m_Index);
            }
        }

        public void OnSubmit()
        {
            if (m_InputField == null) return;

            string text = m_InputField.text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                AddMessage("player", text);
                m_InputField.text = "";
                
                // 入力欄にフォーカスを戻す
                m_InputField.ActivateInputField();
            }
        }

        /// <summary>
        /// チャット履歴をクリア
        /// </summary>
        public void ClearMessages()
        {
            if (m_ScrollRect == null || m_ScrollRect.content == null)
            {
                return;
            }

            // m_ScrollRect.contentの子オブジェクト（メッセージバブル）を全て削除
            int childCount = m_ScrollRect.content.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = m_ScrollRect.content.GetChild(i);
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        #endregion
    }
}
