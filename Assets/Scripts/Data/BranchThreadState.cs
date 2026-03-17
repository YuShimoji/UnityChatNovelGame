using System;
using System.Collections.Generic;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// C分岐スパイク向けの最小ブリッジ状態。
    /// 分岐スレッドのアクティブ状態と、メインスレッドへ戻す転送フラグを保持する。
    /// </summary>
    [Serializable]
    public class BranchThreadState
    {
        public string ActiveBranchId;
        public bool IsActive;
        public bool WasCompleted;
        public List<string> TransferFlags = new List<string>();

        /// <summary>プレイヤーが「持ち帰る」と選択したトピックID (selectモード時)</summary>
        public List<string> TransferredFlags = new List<string>();

        /// <summary>プレイヤーが「持ち帰らない」と選択したトピックID (selectモード時)</summary>
        public List<string> HiddenFlags = new List<string>();

        /// <summary>知識転送選択UIを通過したかどうか</summary>
        public bool SelectionApplied;

        /// <summary>EndBranch 時にメインスレッドへ投入する反映メッセージ (Yarn指定)</summary>
        public string ReflectionMessage;

        public BranchThreadState Clone()
        {
            return new BranchThreadState
            {
                ActiveBranchId = ActiveBranchId,
                IsActive = IsActive,
                WasCompleted = WasCompleted,
                TransferFlags = new List<string>(TransferFlags ?? new List<string>()),
                TransferredFlags = new List<string>(TransferredFlags ?? new List<string>()),
                HiddenFlags = new List<string>(HiddenFlags ?? new List<string>()),
                SelectionApplied = SelectionApplied,
                ReflectionMessage = ReflectionMessage
            };
        }

        public void Clear()
        {
            ActiveBranchId = null;
            IsActive = false;
            WasCompleted = false;
            TransferFlags.Clear();
            TransferredFlags.Clear();
            HiddenFlags.Clear();
            SelectionApplied = false;
            ReflectionMessage = null;
        }
    }
}
