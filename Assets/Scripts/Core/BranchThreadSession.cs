using System.Collections.Generic;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Core
{
    internal sealed class BranchThreadSession
    {
        private BranchThreadState m_State = new BranchThreadState();

        public string ActiveBranchId => m_State.ActiveBranchId;
        public bool IsActive => m_State.IsActive;
        public IReadOnlyList<string> TransferFlags => m_State.TransferFlags;
        public IReadOnlyList<string> ReportedFlags => m_State.SelectionApplied
            ? m_State.TransferredFlags
            : m_State.TransferFlags;

        public string ReflectionMessage
        {
            get => m_State.ReflectionMessage;
            set => m_State.ReflectionMessage = value;
        }

        public BranchThreadState Snapshot() => m_State.Clone();

        public void Restore(BranchThreadState state)
        {
            m_State = state?.Clone() ?? new BranchThreadState();
        }

        public void Begin(string branchId)
        {
            // The restored Yarn node can re-enter the same active branch.
            if (!m_State.IsActive || m_State.ActiveBranchId != branchId)
            {
                m_State.TransferFlags.Clear();
            }

            m_State.ActiveBranchId = branchId;
            m_State.IsActive = true;
            m_State.WasCompleted = false;
        }

        public void TrackFlag(string flagId)
        {
            if (!string.IsNullOrWhiteSpace(flagId) && !m_State.TransferFlags.Contains(flagId))
            {
                m_State.TransferFlags.Add(flagId);
            }
        }

        public void ApplySelection(IReadOnlyList<string> selectedIds)
        {
            m_State.SelectionApplied = true;
            m_State.TransferredFlags = selectedIds == null
                ? new List<string>()
                : new List<string>(selectedIds);
            m_State.HiddenFlags.Clear();
            foreach (string flagId in m_State.TransferFlags)
            {
                if (!m_State.TransferredFlags.Contains(flagId))
                {
                    m_State.HiddenFlags.Add(flagId);
                }
            }
        }

        public void End(bool completed)
        {
            m_State.IsActive = false;
            m_State.WasCompleted = completed;
            m_State.TransferFlags.Clear();
            m_State.TransferredFlags.Clear();
            m_State.HiddenFlags.Clear();
            m_State.SelectionApplied = false;
            m_State.ReflectionMessage = null;
        }
    }
}
