using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// 繝医ヴ繝・け蜷悟｣ｫ繧貞粋謌舌＠縺ｦ譁ｰ縺励＞繝医ヴ繝・け繧堤函謌舌☆繧九Ξ繧ｷ繝斐・螳夂ｾｩ
    /// Topic A + Topic B = Topic C (Result) 縺ｮ髢｢菫ゅｒ陦ｨ迴ｾ
    /// </summary>
    [CreateAssetMenu(fileName = "NewSynthesisRecipe", menuName = "Project FoundPhone/Synthesis Recipe", order = 2)]
    public class SynthesisRecipe : ScriptableObject
    {
        #region Private Fields
        [SerializeField] private TopicData m_IngredientA;
        [SerializeField] private TopicData m_IngredientB;
        [SerializeField] private TopicData m_Result;
        #endregion

        #region Public Properties
        /// <summary>
        /// 蜷域・縺ｫ蠢・ｦ√↑譛蛻昴・譚先侭繝医ヴ繝・け
        /// </summary>
        public TopicData IngredientA => m_IngredientA;

        /// <summary>
        /// 蜷域・縺ｫ蠢・ｦ√↑2逡ｪ逶ｮ縺ｮ譚先侭繝医ヴ繝・け
        /// </summary>
        public TopicData IngredientB => m_IngredientB;

        /// <summary>
        /// 蜷域・邨先棡縺ｨ縺励※逕滓・縺輔ｌ繧九ヨ繝斐ャ繧ｯ
        /// </summary>
        public TopicData Result => m_Result;
        #endregion

        #region Unity Lifecycle
        private void OnValidate()
        {
            // TODO: IngredientA縲！ngredientB縲ヽesult縺碁←蛻・↓險ｭ螳壹＆繧後※縺・ｋ縺九メ繧ｧ繝・け
            // TODO: 蠕ｪ迺ｰ蜿ら・繧・・蟾ｱ蜿ら・縺ｮ繝√ぉ繝・け
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 謖・ｮ壹＆繧後◆2縺､縺ｮ繝医ヴ繝・け縺後％縺ｮ繝ｬ繧ｷ繝斐・譚先侭縺ｨ荳閾ｴ縺吶ｋ縺句愛螳・        /// 鬆・ｺ上・蝠上ｏ縺ｪ縺・ｼ・+B 縺ｨ B+A 縺ｮ荳｡譁ｹ繧定ｨｱ螳ｹ・・        /// </summary>
        /// <param name="topicA">1縺､逶ｮ縺ｮ繝医ヴ繝・け</param>
        /// <param name="topicB">2縺､逶ｮ縺ｮ繝医ヴ繝・け</param>
        /// <returns>荳閾ｴ縺吶ｋ蝣ｴ蜷・rue</returns>
        public bool Matches(TopicData topicA, TopicData topicB)
        {
            if (topicA == null || topicB == null || m_IngredientA == null || m_IngredientB == null)
            {
                return false;
            }

            // A+B 縺ｾ縺溘・ B+A 縺ｮ邨・∩蜷医ｏ縺帙〒荳閾ｴ縺吶ｋ縺九メ繧ｧ繝・け
            bool case1 = topicA.TopicID == m_IngredientA.TopicID && topicB.TopicID == m_IngredientB.TopicID;
            bool case2 = topicA.TopicID == m_IngredientB.TopicID && topicB.TopicID == m_IngredientA.TopicID;

            return case1 || case2;
        }

        /// <summary>
        /// 繝ｬ繧ｷ繝斐′譛牙柑縺九←縺・°繧貞愛螳・        /// </summary>
        /// <returns>譛牙柑縺ｪ蝣ｴ蜷・rue</returns>
        public bool IsValid()
        {
            // TODO: IngredientA縲！ngredientB縲ヽesult縺悟・縺ｦnull縺ｧ縺ｪ縺・°繝√ぉ繝・け
            // TODO: 蜷・ヨ繝斐ャ繧ｯ縺栗sValid()繧呈ｺ縺溘＠縺ｦ縺・ｋ縺九メ繧ｧ繝・け
            return m_IngredientA != null && m_IngredientB != null && m_Result != null;
        }
        #endregion
    }
}
