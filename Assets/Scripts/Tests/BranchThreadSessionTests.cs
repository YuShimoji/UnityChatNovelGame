using NUnit.Framework;
using ProjectFoundPhone.Core;

namespace ProjectFoundPhone.Tests
{
    public class BranchThreadSessionTests
    {
        [Test]
        public void ReenteringRestoredBranchKeepsTransferFlagsUntilBranchEnds()
        {
            var session = new BranchThreadSession();
            session.Begin("branch-a");
            session.TrackFlag("kept");
            session.TrackFlag("hidden");
            var snapshot = session.Snapshot();

            var resumed = new BranchThreadSession();
            resumed.Restore(snapshot);
            resumed.Begin("branch-a");
            resumed.TrackFlag("kept");
            CollectionAssert.AreEqual(new[] { "kept", "hidden" }, resumed.TransferFlags);

            resumed.ApplySelection(new[] { "kept" });
            CollectionAssert.AreEqual(new[] { "kept" }, resumed.ReportedFlags);
            CollectionAssert.AreEqual(new[] { "hidden" }, resumed.Snapshot().HiddenFlags);

            resumed.End(completed: true);
            Assert.IsFalse(resumed.IsActive);
            Assert.IsTrue(resumed.Snapshot().WasCompleted);
            Assert.IsEmpty(resumed.TransferFlags);
            CollectionAssert.AreEqual(new[] { "kept", "hidden" }, snapshot.TransferFlags);
        }
    }
}
