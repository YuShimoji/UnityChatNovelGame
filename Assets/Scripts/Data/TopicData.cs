using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// 謗ｨ隲悶・繝ｼ繝峨〒菴ｿ逕ｨ縺吶ｋ繝医ヴ繝・け・域焔縺後°繧奇ｼ峨・繝・・繧ｿ螳夂ｾｩ
    /// ScriptableObject縺ｨ縺励※菴懈・縺励√お繝・ぅ繧ｿ縺九ｉ邂｡逅・庄閭ｽ縺ｫ縺吶ｋ
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
        /// 繝医ヴ繝・け縺ｮ荳諢上↑隴伜挨蟄・        /// </summary>
        public string TopicID => m_TopicID;

        /// <summary>
        /// 繝医ヴ繝・け縺ｮ繧｢繧､繧ｳ繝ｳ逕ｻ蜒・        /// </summary>
        public Sprite Icon => m_Icon;

        /// <summary>
        /// 繝医ヴ繝・け縺ｮ繧ｿ繧､繝医Ν
        /// </summary>
        public string Title => m_Title;

        /// <summary>
        /// 繝医ヴ繝・け縺ｮ隧ｳ邏ｰ隱ｬ譏・        /// </summary>
        public string Description => m_Description;
        #endregion

        #region Unity Lifecycle
        private void OnValidate()
        {
            // TODO: TopicID縺ｮ驥崎､・メ繧ｧ繝・け繧・ヰ繝ｪ繝・・繧ｷ繝ｧ繝ｳ蜃ｦ逅・ｒ螳溯｣・        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 繝医ヴ繝・け縺梧怏蜉ｹ縺九←縺・°繧貞愛螳・        /// </summary>
        /// <returns>譛牙柑縺ｪ蝣ｴ蜷・rue</returns>
        public bool IsValid()
        {
            // TODO: TopicID縲ゝitle縲．escription縺碁←蛻・↓險ｭ螳壹＆繧後※縺・ｋ縺九メ繧ｧ繝・け
            return !string.IsNullOrEmpty(m_TopicID) && !string.IsNullOrEmpty(m_Title);
        }
        #endregion
    }
}
