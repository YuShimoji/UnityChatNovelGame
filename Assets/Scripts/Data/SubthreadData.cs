using System;
using System.Collections.Generic;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// サブスレッド1本分のデータ。
    /// メインスレッドとは別の会話履歴を持つ。
    /// </summary>
    [Serializable]
    public class SubthreadData
    {
        /// <summary>スレッド識別子 (例: "ch1_annotation_pyramid")</summary>
        public string ThreadId;

        /// <summary>UI表示名 (例: "Pyramidの覚書")</summary>
        public string DisplayName;

        /// <summary>未読メッセージ数</summary>
        public int UnreadCount;

        /// <summary>スレッド内の会話履歴</summary>
        public List<SavedChatMessage> ChatHistory = new List<SavedChatMessage>();
    }
}
