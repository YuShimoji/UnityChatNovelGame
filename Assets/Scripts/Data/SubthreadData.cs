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

        /// <summary>潜在状態 (UIに表示しない)</summary>
        public bool IsLatent;

        /// <summary>顕在化条件 (Yarn変数式。例: "$has_topic_x"。null=手動ManifestThread)</summary>
        public string ManifestCondition;

        /// <summary>顕在化時に自動でBeginBranchを発火するか (Branch型スレッド向け)</summary>
        public bool AutoBeginBranch;

        /// <summary>完了状態 (サイドバーでグレーアウト表示)</summary>
        public bool IsCompleted;

        /// <summary>分岐内で取得したトピック数 (EndBranch時に記録)</summary>
        public int AcquiredTopicCount;

        /// <summary>未読メッセージ数</summary>
        public int UnreadCount;

        /// <summary>スレッド内の会話履歴</summary>
        public List<SavedChatMessage> ChatHistory = new List<SavedChatMessage>();
    }
}
