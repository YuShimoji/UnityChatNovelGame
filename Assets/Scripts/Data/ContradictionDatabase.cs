using System.Collections.Generic;
using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// 全矛盾ペアを管理するデータベース（ScriptableObject）
    /// </summary>
    [CreateAssetMenu(fileName = "ContradictionDatabase", menuName = "Project FoundPhone/Contradiction Database", order = 3)]
    public class ContradictionDatabase : ScriptableObject
    {
        [SerializeField] private List<ContradictionPair> m_Pairs = new List<ContradictionPair>();

        public IReadOnlyList<ContradictionPair> Pairs => m_Pairs;

        /// <summary>
        /// 2つの LineTag に該当する矛盾ペアを検索
        /// </summary>
        public ContradictionPair FindMatch(string tagA, string tagB)
        {
            foreach (var pair in m_Pairs)
            {
                if (pair != null && pair.Matches(tagA, tagB))
                {
                    return pair;
                }
            }
            return null;
        }

        /// <summary>
        /// 指定チャプターまでに検出可能な全ペアを返す
        /// </summary>
        public List<ContradictionPair> GetAvailablePairs(int currentChapter)
        {
            var result = new List<ContradictionPair>();
            foreach (var pair in m_Pairs)
            {
                if (pair != null && pair.Chapter <= currentChapter)
                {
                    result.Add(pair);
                }
            }
            return result;
        }

        /// <summary>
        /// LineTag がいずれかの矛盾ペアに含まれるかチェック
        /// </summary>
        public bool IsContradictionTag(string lineTag, int currentChapter)
        {
            foreach (var pair in m_Pairs)
            {
                if (pair == null || pair.Chapter > currentChapter) continue;
                if (pair.SourceLineTag == lineTag || pair.TargetLineTag == lineTag)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
