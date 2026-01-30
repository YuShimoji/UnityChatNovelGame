using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 謗ｨ隲悶・繝ｼ繝峨↓陦ｨ遉ｺ縺吶ｋ蛟句挨縺ｮ繝医ヴ繝・け繧ｫ繝ｼ繝蔚I
    /// TopicData縺ｮ諠・ｱ・医い繧､繧ｳ繝ｳ縲√ち繧､繝医Ν縲∬ｪｬ譏趣ｼ峨ｒ陦ｨ遉ｺ縺吶ｋ
    /// 繝峨Λ繝・げ・・ラ繝ｭ繝・・縺ｫ繧医ｋ蜷域・謫堺ｽ懊ｒ繧ｵ繝昴・繝・
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
        /// 縺薙・繧ｫ繝ｼ繝峨↓險ｭ螳壹＆繧後※縺・ｋTopicData
        /// </summary>
        private TopicData m_TopicData;

        private Transform m_OriginalParent;
        private Vector3 m_OriginalPosition;
        private int m_OriginalSiblingIndex;
        private Canvas m_ParentCanvas;
        #endregion

        #region Public Properties
        /// <summary>
        /// 縺薙・繧ｫ繝ｼ繝峨↓險ｭ螳壹＆繧後※縺・ｋTopicData繧貞叙蠕・
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
        /// TopicData繧偵き繝ｼ繝峨↓險ｭ螳壹＠縲ゞI繧呈峩譁ｰ縺吶ｋ
        /// </summary>
        /// <param name="topicData">陦ｨ遉ｺ縺吶ｋ繝医ヴ繝・け繝・・繧ｿ</param>
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
        /// UI繧呈峩譁ｰ縺吶ｋ
        /// </summary>
        public void UpdateUI()
        {
            if (m_TopicData == null)
            {
                return;
            }

            // 繧｢繧､繧ｳ繝ｳ繧定ｨｭ螳・
            if (m_IconImage != null)
            {
                m_IconImage.sprite = m_TopicData.Icon;
                m_IconImage.enabled = m_TopicData.Icon != null;
            }

            // 繧ｿ繧､繝医Ν繧定ｨｭ螳・
            if (m_TitleText != null)
            {
                m_TitleText.text = m_TopicData.Title;
            }

            // 隱ｬ譏弱ｒ險ｭ螳夲ｼ医が繝励す繝ｧ繝ｳ・・
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

            // Canvas繧呈､懃ｴ｢縺励※譛蜑埼擇縺ｫ謠冗判縺吶ｋ縺溘ａ縺ｫ繝ｫ繝ｼ繝医↓遘ｻ蜍包ｼ医∪縺溘・繧ｪ繝ｼ繝舌・繝ｬ繧､逕ｨ繝ｬ繧､繝､繝ｼ縺ｸ・・
            // 縺薙％縺ｧ縺ｯ邁｡譏鍋噪縺ｫDeductionBoard縺ｮTransform繧定ｦｪ縺ｫ縺吶ｋ縺九，anvas縺ｮ繝ｫ繝ｼ繝医↓縺吶ｋ
            m_ParentCanvas = GetComponentInParent<Canvas>();
            
            // 繝峨Λ繝・げ荳ｭ縺ｯRactCast繧堤┌蜉ｹ蛹悶＠縺ｦ繝峨Ο繝・・蜈医・讀懷・繧貞庄閭ｽ縺ｫ縺吶ｋ
            m_CanvasGroup.blocksRaycasts = false;
            m_CanvasGroup.alpha = 0.6f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_TopicData == null) return;

            // 繝槭え繧ｹ菴咲ｽｮ縺ｫ霑ｽ蠕・
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

            // 繝峨Ο繝・・蜃ｦ逅・
            // UI荳翫・莉悶・繧ｪ繝悶ず繧ｧ繧ｯ繝茨ｼ・opicCard・峨↓繝峨Ο繝・・縺輔ｌ縺溘°遒ｺ隱・
            GameObject droppedObject = eventData.pointerEnter;
            
            bool handled = false;

            if (droppedObject != null)
            {
                TopicCard targetCard = droppedObject.GetComponent<TopicCard>();
                // 隕ｪ縺ｪ縺ｩ繧定ｾｿ縺｣縺ｦTopicCard繧呈爾縺・
                if (targetCard == null)
                {
                    targetCard = droppedObject.GetComponentInParent<TopicCard>();
                }

                if (targetCard != null && targetCard != this)
                {
                    // DeductionBoard縺ｫ蜷域・繝ｪ繧ｯ繧ｨ繧ｹ繝・
                    if (DeductionBoard.Instance != null)
                    {
                        handled = DeductionBoard.Instance.OnTopicDropped(this, targetCard);
                    }
                }
            }

            // 蜷域・縺ｫ螟ｱ謨励√∪縺溘・辟｡蜉ｹ縺ｪ蝣ｴ謇縺ｪ繧牙・縺ｮ菴咲ｽｮ縺ｫ謌ｻ縺・
            if (!handled)
            {
                // transform.position = m_OriginalPosition;
               // 繝ｬ繧､繧｢繧ｦ繝医げ繝ｫ繝ｼ繝励′蜉ｹ縺・※縺・ｋ蝣ｴ蜷医∽ｽ咲ｽｮ繝ｪ繧ｻ繝・ヨ縺ｯ閾ｪ蜍輔〒陦後ｏ繧後ｋ縺薙→縺悟､壹＞縺・
               // 蠑ｷ蛻ｶ逧・↓謌ｻ縺吶↑繧我ｻ･荳・
               transform.position = m_OriginalPosition;
            }
        }
        #endregion
    }
}
