using System;
using System.Collections.Generic;
using UnityEngine;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// 矛盾指摘メカニクスの状態管理
    /// </summary>
    public class ContradictionManager : MonoBehaviour
    {
        #region Singleton
        private static ContradictionManager s_Instance;
        public static ContradictionManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<ContradictionManager>();
                }
                return s_Instance;
            }
        }
        #endregion

        [SerializeField] private ContradictionDatabase m_Database;
        [SerializeField] private int m_CurrentChapter = 1;
        [SerializeField] private float m_CooldownSeconds = 10f;

        private readonly HashSet<string> m_DiscoveredIDs = new HashSet<string>();
        private string m_SelectedLineTag;
        private float m_CooldownEndTime;
        private int m_HalluciCoin;
        private bool m_CurrentChannelEnableHints = true;
        private int m_CurrentChannelMaxHintDifficulty = 1;

        public event Action<ContradictionPair> OnContradictionDiscovered;
        public event Action<string> OnPointingFailed;
        public event Action<string> OnSelectionChanged;
        public event Action OnSelectionCleared;

        public bool IsInPointingMode => m_SelectedLineTag != null;
        public bool IsOnCooldown => Time.time < m_CooldownEndTime;
        public int HalluciCoin => m_HalluciCoin;
        public int CurrentChapter => m_CurrentChapter;
        public IReadOnlyCollection<string> DiscoveredIDs => m_DiscoveredIDs;

        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            s_Instance = this;
        }

        private void OnDestroy()
        {
            if (s_Instance == this) s_Instance = null;
        }

        /// <summary>
        /// バブル長押し時に呼ばれる。1つ目を選択して指摘モードに入る
        /// </summary>
        public bool SelectFirst(string lineTag)
        {
            if (string.IsNullOrEmpty(lineTag)) return false;
            if (IsOnCooldown)
            {
                Debug.Log($"ContradictionManager: Cooldown active. {m_CooldownEndTime - Time.time:F1}s remaining.");
                return false;
            }

            m_SelectedLineTag = lineTag;
            OnSelectionChanged?.Invoke(lineTag);
            return true;
        }

        /// <summary>
        /// 2つ目のバブルタップ時に呼ばれる。矛盾判定を実行
        /// </summary>
        public bool SelectSecond(string lineTag)
        {
            if (string.IsNullOrEmpty(m_SelectedLineTag) || string.IsNullOrEmpty(lineTag))
            {
                ClearSelection();
                return false;
            }

            if (lineTag == m_SelectedLineTag)
            {
                // 同じバブルを選択した場合はキャンセル
                ClearSelection();
                return false;
            }

            if (m_Database == null)
            {
                Debug.LogWarning("ContradictionManager: Database not assigned.");
                ClearSelection();
                return false;
            }

            ContradictionPair match = m_Database.FindMatch(m_SelectedLineTag, lineTag);

            if (match != null && !m_DiscoveredIDs.Contains(match.ContradictionID))
            {
                // 矛盾発見 — イベント発火後、選択状態のみリセット
                // OnSelectionCleared は発火しない（アニメーション中に HideConnectionLine されるのを防ぐ）
                // ContradictionFeedbackController のアニメーション完了時にクリーンアップする
                m_DiscoveredIDs.Add(match.ContradictionID);
                m_HalluciCoin += match.RewardCoin;
                OnContradictionDiscovered?.Invoke(match);
                Debug.Log($"ContradictionManager: Contradiction '{match.ContradictionID}' discovered! HalluciCoin: {m_HalluciCoin}");
                m_SelectedLineTag = null;
                m_CooldownEndTime = 0f;
                return true;
            }
            else if (match != null && m_DiscoveredIDs.Contains(match.ContradictionID))
            {
                // 既に発見済み — 同上
                OnPointingFailed?.Invoke("already_discovered");
                m_SelectedLineTag = null;
                m_CooldownEndTime = 0f;
                return false;
            }
            else
            {
                // 不一致 → クールダウン発動 — 同上
                m_CooldownEndTime = Time.time + m_CooldownSeconds;
                OnPointingFailed?.Invoke("no_match");
                Debug.Log($"ContradictionManager: No match found. Cooldown {m_CooldownSeconds}s.");
                m_SelectedLineTag = null;
                return false;
            }
        }

        /// <summary>
        /// 指摘モードをキャンセル
        /// </summary>
        public void ClearSelection()
        {
            m_SelectedLineTag = null;
            OnSelectionCleared?.Invoke();
        }

        /// <summary>
        /// 指定 LineTag がヒント対象かどうか（段階的開示ロジック）。
        /// ヒント表示ポリシーは ChannelData の EnableHints / MaxHintDifficulty で制御。
        /// </summary>
        public bool ShouldShowHint(string lineTag, int difficulty)
        {
            if (m_Database == null) return false;

            // ChannelData ベースのポリシー判定
            if (!m_CurrentChannelEnableHints) return false;
            if (difficulty > m_CurrentChannelMaxHintDifficulty) return false;

            return m_Database.IsContradictionTag(lineTag, m_CurrentChapter);
        }

        /// <summary>
        /// セーブ用: 発見済み矛盾IDリストを取得
        /// </summary>
        public List<string> GetDiscoveredList()
        {
            return new List<string>(m_DiscoveredIDs);
        }

        /// <summary>
        /// ロード用: 発見済み矛盾を復元
        /// </summary>
        public void RestoreDiscovered(List<string> ids, int halluciCoin)
        {
            m_DiscoveredIDs.Clear();
            if (ids != null)
            {
                foreach (var id in ids) m_DiscoveredIDs.Add(id);
            }
            m_HalluciCoin = halluciCoin;
        }

        public void SetCurrentChapter(int chapter)
        {
            m_CurrentChapter = chapter;
        }

        /// <summary>
        /// ChannelData からチャプター番号とヒントポリシーを一括設定
        /// </summary>
        public void SetCurrentChannel(ChannelData channel)
        {
            if (channel == null) return;
            m_CurrentChapter = channel.ChapterNumber;
            m_CurrentChannelEnableHints = channel.EnableHints;
            m_CurrentChannelMaxHintDifficulty = channel.MaxHintDifficulty;
        }
    }
}
