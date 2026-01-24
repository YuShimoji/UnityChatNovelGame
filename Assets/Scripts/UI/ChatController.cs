using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

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

        [Header("Choice Settings")]
        [SerializeField] private GameObject m_ChoiceButtonPrefab;
        [SerializeField] private Transform m_ChoiceContainer;


        private bool m_IsUserScrolling = false;
        private float m_LastScrollPosition = 1.0f;
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

            // charIDに応じて右寄せ/左寄せを設定（"player"の場合は右寄せ、それ以外は左寄せ）
            RectTransform rectTransform = messageBubble.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (charID == "player")
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
                int index = i; // キャプチャ用
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
                    btn.onClick.AddListener(() =>
                    {
                        HideChoices();
                        onSelected?.Invoke(index);
                    });
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

            // Canvasの更新を待ってからスクロールするためにコルーチンか遅延実行を使うのが一般的だが、
            // ここでは簡易的にDOTweenで遅延させる
            DOVirtual.DelayedCall(0.1f, () => {
                if(m_ScrollRect == null) return;
                
                // DOTweenを使用したスクロールアニメーション（0.3秒）
                DOTween.To(
                    () => m_ScrollRect.verticalNormalizedPosition,
                    x => m_ScrollRect.verticalNormalizedPosition = x,
                    0.0f, // 0.0f is bottom for vertical scroll rect
                    0.3f
                ).OnComplete(() =>
                {
                    // スクロール完了後にm_LastScrollPositionを更新
                    m_LastScrollPosition = 0.0f;
                });
            });
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
