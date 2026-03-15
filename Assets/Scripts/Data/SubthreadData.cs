using System;
using System.Collections.Generic;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// スレッドの種別。DeclareThread の type 引数で指定する。
    /// </summary>
    public enum ThreadType
    {
        /// <summary>A: 注釈 — 調査メモ・覚書</summary>
        Annotation,
        /// <summary>B: 追跡 — 調査ログ・時系列記録</summary>
        Tracking,
        /// <summary>C: 偵察 — 探索結果・成果物</summary>
        Scout,
        /// <summary>分岐 — ストーリー分岐スレッド</summary>
        Branch
    }

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

        /// <summary>スレッド種別</summary>
        public ThreadType Type;

        /// <summary>未読メッセージ数</summary>
        public int UnreadCount;

        /// <summary>スレッド内の会話履歴</summary>
        public List<SavedChatMessage> ChatHistory = new List<SavedChatMessage>();
    }
}
