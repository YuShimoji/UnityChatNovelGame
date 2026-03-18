using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// 進捗スナップショットからプレイヤーへのヒント文を生成する純粋ロジック。
    /// 状態を持たない static class。
    /// </summary>
    public static class NudgeSystem
    {
        public static string GetNudge(ProgressSnapshot snapshot, ChannelData[] channels, SaveData saveData)
        {
            // 1. まだ何も始めていない
            if (snapshot.ChaptersCompleted == 0 && snapshot.ChaptersInProgress == 0)
                return "Start by selecting a channel above.";

            // 2. プレイ中だが矛盾を一つも見つけていない
            if (snapshot.ContradictionsFound == 0 && snapshot.ChaptersInProgress > 0)
                return "Long-press a message to begin contradiction detection.";

            // 3. 矛盾は見つけたが断片が未収集
            if (snapshot.ContradictionsFound > 0 && snapshot.FragmentsCollected == 0)
                return "Check your Inventory for collected fragments.";

            // 4. HCゲート不足で次チャプターがロックされている
            if (channels != null && saveData != null)
            {
                foreach (var ch in channels)
                {
                    if (!ProgressTracker.IsContentChannel(ch)) continue;
                    if (ch.RequiredHalluciCoin <= 0) continue;
                    if (saveData.CompletedChannelIDs != null && saveData.CompletedChannelIDs.Contains(ch.ChannelID)) continue;

                    if (snapshot.HalluciCoin < ch.RequiredHalluciCoin)
                    {
                        int needed = ch.RequiredHalluciCoin - snapshot.HalluciCoin;
                        return $"Collect {needed} more HC to unlock {ch.DisplayName}.";
                    }
                }
            }

            // 5. 次チャプターが利用可能
            if (snapshot.ChaptersCompleted > 0 && snapshot.ChaptersCompleted < snapshot.ChaptersTotal)
                return "A new channel is available. Tap to begin.";

            // 6. 未発見の矛盾がまだある
            if (snapshot.ContradictionsFound < snapshot.ContradictionsTotal && snapshot.ChaptersInProgress > 0)
                return "There are still undiscovered contradictions in this chapter.";

            // 7. ほぼ完了
            if (snapshot.OverallPercent >= 0.9f)
                return "Almost there. Review what you've found.";

            // 8. フォールバック
            return "Continue exploring the channels.";
        }
    }
}
