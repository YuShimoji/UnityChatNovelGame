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
                // Enterキーでも送信できるようにする (Optional)
                m_InputField.onSubmit.AddListener((text) => OnSubmit());
            }
        }

        private void Update()
        {
            // TODO: スクロール位置の監視とユーザー操作の検知
            CheckUserScrollInput();
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
        }

        /// <summary>
        /// ユーザーが手動でスクロールしているかを検知
        /// </summary>
        private void CheckUserScrollInput()
        {
            if (m_ScrollRect == null)
            {
                return;
            }

            float currentScrollPosition = m_ScrollRect.verticalNormalizedPosition;

            // スクロール位置が下から一定以上離れている場合、ユーザーが過去ログを見ていると判定
            if (currentScrollPosition < (1.0f - m_AutoScrollThreshold))
            {
                m_IsUserScrolling = true;
            }
            // スクロール位置が1.0に近い場合、ユーザーは最新メッセージを見ている
            else if (currentScrollPosition >= 0.99f)
            {
                m_IsUserScrolling = false;
            }

            m_LastScrollPosition = currentScrollPosition;
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

            // CharacterDatabaseからプロファイルを取得し、プレイヤー判定とテーマカラーを決定
            bool isPlayer = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.IsPlayer(charID)
                : charID == "player";

            Color themeColor = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.GetThemeColor(charID)
                : (isPlayer ? new Color(0.2f, 0.6f, 1.0f) : new Color(0.85f, 0.85f, 0.85f));

            // charIDに応じて右寄せ/左寄せを設定
            RectTransform rectTransform = messageBubble.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (isPlayer)
                {
                    // 右寄せ: Anchorを右側に設定
                    rectTransform.anchorMin = new Vector2(1.0f, 1.0f);
                    rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                    rectTransform.pivot = new Vector2(1.0f, 1.0f);
                }
                else
                {
                    // 左寄せ: Anchorを左側に設定
                    rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                    rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                    rectTransform.pivot = new Vector2(0.0f, 1.0f);
                }
            }

            // バブル背景にテーマカラーを適用
            Image bubbleBackground = messageBubble.GetComponent<Image>();
            if (bubbleBackground != null)
            {
                bubbleBackground.color = themeColor;
            }

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

            // プレイヤー判定とテーマカラー
            bool isPlayer = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.IsPlayer(charID)
                : charID == "player";

            Color themeColor = CharacterDatabase.Instance != null
                ? CharacterDatabase.Instance.GetThemeColor(charID)
                : (isPlayer ? new Color(0.2f, 0.6f, 1.0f) : new Color(0.85f, 0.85f, 0.85f));

            // 右寄せ/左寄せ設定
            RectTransform rectTransform = imageBubble.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (isPlayer)
                {
                    rectTransform.anchorMin = new Vector2(1.0f, 1.0f);
                    rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                    rectTransform.pivot = new Vector2(1.0f, 1.0f);
                }
                else
                {
                    rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                    rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                    rectTransform.pivot = new Vector2(0.0f, 1.0f);
                }
            }

            // バブル背景にテーマカラーを適用
            Image bubbleBackground = imageBubble.GetComponent<Image>();
            if (bubbleBackground != null)
            {
                bubbleBackground.color = themeColor;
            }

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
