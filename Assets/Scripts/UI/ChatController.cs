using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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
        [SerializeField] private float m_AutoScrollThreshold = 0.1f; // 自動スクロールを実行する閾値（0.0-1.0）

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
            // TODO: 初期化処理（既存メッセージの読み込みなど）
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
            }

            // 表示時はAutoScroll()を実行してインジケーターが見えるようにする
            if (show)
            {
                AutoScroll();
            }
        }

        /// <summary>
        /// チャットを最下部に自動スクロール
        /// ユーザーが過去ログを見ている場合は強制スクロールしない
        /// </summary>
        public void AutoScroll()
        {
            if (m_ScrollRect == null || m_IsUserScrolling)
            {
                return;
            }

            // DOTweenを使用したスクロールアニメーション（0.3秒）
            DOTween.To(
                () => m_ScrollRect.verticalNormalizedPosition,
                x => m_ScrollRect.verticalNormalizedPosition = x,
                1.0f,
                0.3f
            ).OnComplete(() =>
            {
                // スクロール完了後にm_LastScrollPositionを更新
                m_LastScrollPosition = 1.0f;
            });
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
