using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 推論ボードに表示する個別のトピックカードUI
    /// TopicDataの情報（アイコン、タイトル、説明）を表示する
    /// </summary>
    public class TopicCard : MonoBehaviour
    {
        #region Private Fields
        [Header("UI References")]
        [SerializeField] private Image m_IconImage;
        [SerializeField] private TextMeshProUGUI m_TitleText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;

        /// <summary>
        /// このカードに設定されているTopicData
        /// </summary>
        private TopicData m_TopicData;
        #endregion

        #region Public Properties
        /// <summary>
        /// このカードに設定されているTopicDataを取得
        /// </summary>
        public TopicData TopicData => m_TopicData;
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
    }
}
