using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// トピック同士を合成して新しいトピックを生成するレシピの定義
    /// Topic A + Topic B = Topic C (Result) の関係を表現
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
        /// 合成に必要な最初の材料トピック
        /// </summary>
        public TopicData IngredientA => m_IngredientA;

        /// <summary>
        /// 合成に必要な2番目の材料トピック
        /// </summary>
        public TopicData IngredientB => m_IngredientB;

        /// <summary>
        /// 合成結果として生成されるトピック
        /// </summary>
        public TopicData Result => m_Result;
        #endregion

        #region Unity Lifecycle
        private void OnValidate()
        {
            if (m_IngredientA == null)
            {
                Debug.LogWarning($"SynthesisRecipe '{name}': IngredientA is not set.");
            }

            if (m_IngredientB == null)
            {
                Debug.LogWarning($"SynthesisRecipe '{name}': IngredientB is not set.");
            }

            if (m_Result == null)
            {
                Debug.LogWarning($"SynthesisRecipe '{name}': Result is not set.");
            }

            // 自己参照チェック: 材料と結果が同じトピックでないか
            if (m_IngredientA != null && m_Result != null && m_IngredientA == m_Result)
            {
                Debug.LogError($"SynthesisRecipe '{name}': IngredientA and Result reference the same TopicData.");
            }

            if (m_IngredientB != null && m_Result != null && m_IngredientB == m_Result)
            {
                Debug.LogError($"SynthesisRecipe '{name}': IngredientB and Result reference the same TopicData.");
            }

            // 同一材料チェック
            if (m_IngredientA != null && m_IngredientB != null && m_IngredientA == m_IngredientB)
            {
                Debug.LogWarning($"SynthesisRecipe '{name}': IngredientA and IngredientB are the same TopicData.");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 指定された2つのトピックがこのレシピの材料と一致するか判定
        /// 順序は問わない（A+B と B+A の両方を許容）
        /// </summary>
        /// <param name="topicA">1つ目のトピック</param>
        /// <param name="topicB">2つ目のトピック</param>
        /// <returns>一致する場合true</returns>
        public bool Matches(TopicData topicA, TopicData topicB)
        {
            if (topicA == null || topicB == null || m_IngredientA == null || m_IngredientB == null)
            {
                return false;
            }

            // A+B または B+A の組み合わせで一致するかチェック
            bool case1 = topicA.TopicID == m_IngredientA.TopicID && topicB.TopicID == m_IngredientB.TopicID;
            bool case2 = topicA.TopicID == m_IngredientB.TopicID && topicB.TopicID == m_IngredientA.TopicID;

            return case1 || case2;
        }

        /// <summary>
        /// レシピが有効かどうかを判定
        /// </summary>
        /// <returns>有効な場合true</returns>
        public bool IsValid()
        {
            return m_IngredientA != null && m_IngredientB != null && m_Result != null
                && m_IngredientA.IsValid() && m_IngredientB.IsValid() && m_Result.IsValid();
        }
        #endregion
    }
}
