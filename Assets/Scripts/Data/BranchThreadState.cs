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

        public BranchThreadState Clone()
        {
            return new BranchThreadState
            {
                ActiveBranchId = ActiveBranchId,
                IsActive = IsActive,
                WasCompleted = WasCompleted,
                TransferFlags = new List<string>(TransferFlags ?? new List<string>())
            };
        }

        public void Clear()
        {
            ActiveBranchId = null;
            IsActive = false;
            WasCompleted = false;
            TransferFlags.Clear();
        }
    }
}
