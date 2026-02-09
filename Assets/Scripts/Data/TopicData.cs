using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// 推論ボードで使用するトピック（手がかり）のデータ定義
    /// ScriptableObjectとして作成し、エディタから管理可能にする
    /// </summary>
    [CreateAssetMenu(fileName = "NewTopic", menuName = "Project FoundPhone/Topic Data", order = 1)]
    public class TopicData : ScriptableObject
    {
        #region Private Fields
        [SerializeField] private string m_TopicID;
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private string m_Title;
        [SerializeField, TextArea(3, 10)] private string m_Description;
        #endregion

        #region Public Properties
        /// <summary>
        /// トピックの一意な識別子
        /// </summary>
        public string TopicID => m_TopicID;

        /// <summary>
        /// トピックのアイコン画像
        /// </summary>
        public Sprite Icon => m_Icon;

        /// <summary>
        /// トピックのタイトル
        /// </summary>
        public string Title => m_Title;

        /// <summary>
        /// トピックの詳細説明
        /// </summary>
        public string Description => m_Description;
        #endregion

        #region Unity Lifecycle
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(m_TopicID))
            {
                Debug.LogWarning($"TopicData '{name}': TopicID is empty. Please set a unique ID.");
            }
            else if (m_TopicID.Contains(" "))
            {
                Debug.LogWarning($"TopicData '{name}': TopicID '{m_TopicID}' contains spaces. Use underscores instead.");
            }

            if (string.IsNullOrEmpty(m_Title))
            {
                Debug.LogWarning($"TopicData '{name}': Title is empty.");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// トピックが有効かどうかを判定
        /// </summary>
        /// <returns>有効な場合true</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(m_TopicID) && !string.IsNullOrEmpty(m_Title);
        }
        #endregion
    }
}
