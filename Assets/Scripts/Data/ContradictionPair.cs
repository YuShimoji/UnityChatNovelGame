using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// 矛盾ペアの定義。2つのメッセージが矛盾関係にあることを表す。
    /// </summary>
    [CreateAssetMenu(fileName = "NewContradiction", menuName = "Project FoundPhone/Contradiction Pair", order = 2)]
    public class ContradictionPair : ScriptableObject
    {
        [SerializeField] private string m_ContradictionID;
        [SerializeField, Tooltip("矛盾の1つ目を識別するタグ（Yarn #line: タグまたはカスタムID）")]
        private string m_SourceLineTag;
        [SerializeField, Tooltip("矛盾の2つ目を識別するタグ")]
        private string m_TargetLineTag;
        [SerializeField] private int m_Chapter;
        [SerializeField] private int m_RewardCoin = 1;
        [SerializeField, Tooltip("発見時にアンロックされるトピック（任意）")]
        private TopicData m_UnlockTopic;
        [SerializeField, Tooltip("発見時に再生される Yarn ノード名（任意）")]
        private string m_ReactionNodeName;
        [SerializeField, Range(1, 3), Tooltip("1=ヒント多 2=ヒント少 3=ヒントなし")]
        private int m_Difficulty = 1;
        [SerializeField, TextArea(2, 5)] private string m_Description;

        public string ContradictionID => m_ContradictionID;
        public string SourceLineTag => m_SourceLineTag;
        public string TargetLineTag => m_TargetLineTag;
        public int Chapter => m_Chapter;
        public int RewardCoin => m_RewardCoin;
        public TopicData UnlockTopic => m_UnlockTopic;
        public string ReactionNodeName => m_ReactionNodeName;
        public int Difficulty => m_Difficulty;
        public string Description => m_Description;

        /// <summary>
        /// 2つの LineTag がこの矛盾ペアに該当するか判定（順不同）
        /// </summary>
        public bool Matches(string tagA, string tagB)
        {
            return (tagA == m_SourceLineTag && tagB == m_TargetLineTag)
                || (tagA == m_TargetLineTag && tagB == m_SourceLineTag);
        }
    }
}
