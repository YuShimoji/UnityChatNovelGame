using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 推論ボードに表示する個別のトピックカードUI
    /// TopicDataの情報（アイコン、タイトル、説明）を表示する
    /// ドラッグ＆ドロップによる合成操作をサポート
    /// </summary>
    public class TopicCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Private Fields
        [Header("UI References")]
        [SerializeField] private Image m_IconImage;
        [SerializeField] private TextMeshProUGUI m_TitleText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;

        [Header("Drag Settings")]
        [SerializeField] private CanvasGroup m_CanvasGroup;

        /// <summary>
        /// このカードに設定されているTopicData
        /// </summary>
        private TopicData m_TopicData;

        private Transform m_OriginalParent;
        private Vector3 m_OriginalPosition;
        private int m_OriginalSiblingIndex;
        private Canvas m_ParentCanvas;
        #endregion

        #region Public Properties
        /// <summary>
        /// このカードに設定されているTopicDataを取得
        /// </summary>
        public TopicData TopicData => m_TopicData;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (m_CanvasGroup == null)
            {
                m_CanvasGroup = GetComponent<CanvasGroup>();
                if (m_CanvasGroup == null)
                {
                    m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// TopicDataをカードに設定し、UIを更新する
        /// </summary>
        /// <param name="topicData">表示するトピックデータ</param>
        public void Setup(TopicData topicData)
        {
            if (topicData == null)
            {
                Debug.LogWarning("TopicCard: Attempted to setup with null TopicData.");
                return;
            }

            m_TopicData = topicData;
            UpdateUI();
        }

        /// <summary>
        /// UIを更新する
        /// </summary>
        public void UpdateUI()
        {
            if (m_TopicData == null)
            {
                return;
            }

            // アイコンを設定
            if (m_IconImage != null)
            {
                m_IconImage.sprite = m_TopicData.Icon;
                m_IconImage.enabled = m_TopicData.Icon != null;
            }

            // タイトルを設定
            if (m_TitleText != null)
            {
                m_TitleText.text = m_TopicData.Title;
            }

            // 説明を設定（オプション）
            if (m_DescriptionText != null)
            {
                m_DescriptionText.text = m_TopicData.Description;
            }
        }
        #endregion

        #region Drag Implementation
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (m_TopicData == null) return;

            m_OriginalParent = transform.parent;
            m_OriginalPosition = transform.position;
            m_OriginalSiblingIndex = transform.GetSiblingIndex();

            // Canvasを検索して最前面に描画するためにルートに移動（またはオーバーレイ用レイヤーへ）
            // ここでは簡易的にDeductionBoardのTransformを親にするか、Canvasのルートにする
            m_ParentCanvas = GetComponentInParent<Canvas>();
            
            // ドラッグ中はRactCastを無効化してドロップ先の検出を可能にする
            m_CanvasGroup.blocksRaycasts = false;
            m_CanvasGroup.alpha = 0.6f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_TopicData == null) return;

            // マウス位置に追従
            if (m_ParentCanvas != null && m_ParentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                transform.position = eventData.position;
            }
            else
            {
                // World Space or Camera Space
                Vector3 worldPoint;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    transform.parent as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out worldPoint))
                {
                    transform.position = worldPoint;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (m_TopicData == null) return;

            m_CanvasGroup.blocksRaycasts = true;
            m_CanvasGroup.alpha = 1.0f;

            // ドロップ処理
            // UI上の他のオブジェクト（TopicCard）にドロップされたか確認
            GameObject droppedObject = eventData.pointerEnter;
            
            bool handled = false;

            if (droppedObject != null)
            {
                TopicCard targetCard = droppedObject.GetComponent<TopicCard>();
                // 親などを辿ってTopicCardを探す
                if (targetCard == null)
                {
                    targetCard = droppedObject.GetComponentInParent<TopicCard>();
                }

                if (targetCard != null && targetCard != this)
                {
                    // DeductionBoardに合成リクエスト
                    if (DeductionBoard.Instance != null)
                    {
                        handled = DeductionBoard.Instance.OnTopicDropped(this, targetCard);
                    }
                }
            }

            // 合成に失敗、または無効な場所なら元の位置に戻す
            if (!handled)
            {
                // transform.position = m_OriginalPosition;
               // レイアウトグループが効いている場合、位置リセットは自動で行われることが多いが
               // 強制的に戻すなら以下
               transform.position = m_OriginalPosition;
            }
        }
        #endregion
    }
}
