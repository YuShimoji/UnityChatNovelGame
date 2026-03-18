using System;
using System.Collections.Generic;
using UnityEngine;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// ゲーム全体の進捗データを集約し、ProgressSnapshot を返す。
    /// 自身はデータを保持せず、既存マネージャーから毎回導出する。
    /// </summary>
    public class ProgressTracker : MonoBehaviour
    {
        private static ProgressTracker s_Instance;
        public static ProgressTracker Instance => s_Instance;

        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            s_Instance = this;
        }

        /// <summary>現時点の進捗スナップショットを計算して返す</summary>
        public ProgressSnapshot GetSnapshot()
        {
            var snap = new ProgressSnapshot();

            // --- チャプター ---
            ChannelData[] allChannels = Resources.LoadAll<ChannelData>("Channels");
            var saveManager = SaveManager.Instance;
            SaveData saveData = saveManager?.CurrentSaveData;
            List<string> completedIds = saveData?.CompletedChannelIDs ?? new List<string>();
            Dictionary<string, int> dayProgress = saveData?.ChannelDayProgress ?? new Dictionary<string, int>();

            foreach (var ch in allChannels)
            {
                if (!IsContentChannel(ch)) continue;
                snap.ChaptersTotal++;

                if (completedIds.Contains(ch.ChannelID))
                    snap.ChaptersCompleted++;
                else if (dayProgress.ContainsKey(ch.ChannelID))
                    snap.ChaptersInProgress++;
            }

            // --- 矛盾 ---
            ContradictionManager cm = FindFirstObjectByType<ContradictionManager>();
            if (cm != null)
            {
                snap.ContradictionsTotal = cm.TotalPairCount;
                snap.ContradictionsFound = cm.DiscoveredCount;
                snap.HalluciCoin = cm.HalluciCoin;
            }

            // --- トピック・断片 ---
            TopicData[] allTopics = Resources.LoadAll<TopicData>("Topics");
            DeductionBoard board = DeductionBoard.Instance;
            var unlockedTopics = board?.UnlockedTopics;

            foreach (var topic in allTopics)
            {
                if (IsExcludedTopic(topic.TopicID)) continue;

                if (topic.TopicID.StartsWith("fragment_"))
                {
                    snap.FragmentsTotal++;
                    if (unlockedTopics != null && HasTopic(unlockedTopics, topic.TopicID))
                        snap.FragmentsCollected++;
                }
                else
                {
                    snap.TopicsTotal++;
                    if (unlockedTopics != null && HasTopic(unlockedTopics, topic.TopicID))
                        snap.TopicsUnlocked++;
                }
            }

            // --- 加重平均 ---
            snap.OverallPercent = CalculateOverall(snap);

            return snap;
        }

        private static float CalculateOverall(ProgressSnapshot s)
        {
            float sum = 0f;
            if (s.ChaptersTotal > 0)
                sum += 0.35f * s.ChaptersCompleted / s.ChaptersTotal;
            if (s.ContradictionsTotal > 0)
                sum += 0.30f * s.ContradictionsFound / s.ContradictionsTotal;
            if (s.FragmentsTotal > 0)
                sum += 0.20f * s.FragmentsCollected / s.FragmentsTotal;
            if (s.TopicsTotal > 0)
                sum += 0.15f * s.TopicsUnlocked / s.TopicsTotal;
            return Mathf.Clamp01(sum);
        }

        /// <summary>テスト/デバッグ用チャンネルを除外する</summary>
        public static bool IsContentChannel(ChannelData ch)
        {
            if (ch == null) return false;
            string id = ch.ChannelID;
            return !id.StartsWith("ch_etk") && !id.StartsWith("ch_test");
        }

        private static bool IsExcludedTopic(string topicId)
        {
            return topicId.StartsWith("debug_") || topicId.StartsWith("etk_");
        }

        private static bool HasTopic(IReadOnlyList<TopicData> list, string topicId)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].TopicID == topicId) return true;
            }
            return false;
        }
    }

    [Serializable]
    public struct ProgressSnapshot
    {
        public int ChaptersTotal;
        public int ChaptersCompleted;
        public int ChaptersInProgress;

        public int ContradictionsTotal;
        public int ContradictionsFound;

        public int FragmentsTotal;
        public int FragmentsCollected;

        public int TopicsTotal;
        public int TopicsUnlocked;

        public int HalluciCoin;

        public float OverallPercent;
    }
}
