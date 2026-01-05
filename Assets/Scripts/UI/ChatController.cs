using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

            // TODO: m_MessageBubblePrefab、m_TypingIndicatorのnullチェックと警告
        }

        /// <summary>
        /// ユーザーが手動でスクロールしているかを検知
        /// </summary>
        private void CheckUserScrollInput()
        {
            // TODO: スクロール位置の変化を監視し、ユーザーが過去ログを見ているか判定
            // スクロール位置が下から一定以上離れている場合、m_IsUserScrolling = true
        }

        /// <summary>
        /// メッセージバブルのPrefabをインスタンス化
        /// </summary>
        /// <param name="charID">キャラクターID（自分/相手の判定に使用）</param>
        /// <param name="text">メッセージテキスト</param>
        /// <returns>生成されたGameObject</returns>
        private GameObject CreateMessageBubble(string charID, string text)
        {
            // TODO: m_MessageBubblePrefabからインスタンスを生成
            // TODO: charIDに応じて右寄せ/左寄せを設定
            // TODO: TextMeshProコンポーネントにtextを設定
            // TODO: ContentSizeFitterで高さを自動調整
            return null;
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
            // TODO: CreateMessageBubble()でメッセージバブルを生成
            // TODO: m_ScrollRect.contentの子として追加
            // TODO: ユーザーが過去ログを見ていない場合のみAutoScroll()を実行
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

            // TODO: 表示時はAutoScroll()を実行してインジケーターが見えるようにする
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

            // TODO: ScrollRectのverticalNormalizedPositionを1.0に設定
            // TODO: スクロールアニメーション（DOTweenを使用）を実装
            // TODO: スクロール完了後にm_LastScrollPositionを更新
        }

        /// <summary>
        /// チャット履歴をクリア
        /// </summary>
        public void ClearMessages()
        {
            // TODO: m_ScrollRect.contentの子オブジェクト（メッセージバブル）を全て削除
        }
        #endregion
    }
}
