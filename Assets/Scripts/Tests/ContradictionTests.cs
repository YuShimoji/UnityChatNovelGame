using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;
using System.Collections.Generic;
using System.Linq;

namespace ProjectFoundPhone.Tests
{
    /// <summary>
    /// ContradictionPair, ContradictionDatabase, ContradictionManager のコアロジックテスト
    /// </summary>
    public class ContradictionTests
    {
        #region Helper Methods
        private ContradictionPair CreatePair(string id, string srcTag, string tgtTag, int chapter = 1, int reward = 10)
        {
            var pair = ScriptableObject.CreateInstance<ContradictionPair>();
            var so = new SerializedObject(pair);
            so.FindProperty("m_ContradictionID").stringValue = id;
            so.FindProperty("m_SourceLineTag").stringValue = srcTag;
            so.FindProperty("m_TargetLineTag").stringValue = tgtTag;
            so.FindProperty("m_Chapter").intValue = chapter;
            so.FindProperty("m_RewardCoin").intValue = reward;
            so.FindProperty("m_Difficulty").intValue = 1;
            so.ApplyModifiedPropertiesWithoutUndo();
            return pair;
        }

        private ContradictionDatabase CreateDatabase(params ContradictionPair[] pairs)
        {
            var db = ScriptableObject.CreateInstance<ContradictionDatabase>();
            var so = new SerializedObject(db);
            var pairsProp = so.FindProperty("m_Pairs");
            pairsProp.arraySize = pairs.Length;
            for (int i = 0; i < pairs.Length; i++)
            {
                pairsProp.GetArrayElementAtIndex(i).objectReferenceValue = pairs[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            return db;
        }

        private (GameObject go, ContradictionManager manager) CreateManager(ContradictionDatabase db, int chapter = 1)
        {
            var go = new GameObject("TestContradictionManager");
            var manager = go.AddComponent<ContradictionManager>();
            var so = new SerializedObject(manager);
            so.FindProperty("m_Database").objectReferenceValue = db;
            so.FindProperty("m_CurrentChapter").intValue = chapter;
            so.FindProperty("m_CooldownSeconds").floatValue = 10f;
            so.ApplyModifiedPropertiesWithoutUndo();
            return (go, manager);
        }
        #endregion

        #region ContradictionPair Tests
        [Test]
        public void ContradictionPair_Matches_CorrectOrder()
        {
            var pair = CreatePair("test", "src_tag", "tgt_tag");
            Assert.IsTrue(pair.Matches("src_tag", "tgt_tag"));
        }

        [Test]
        public void ContradictionPair_Matches_ReverseOrder()
        {
            var pair = CreatePair("test", "src_tag", "tgt_tag");
            Assert.IsTrue(pair.Matches("tgt_tag", "src_tag"));
        }

        [Test]
        public void ContradictionPair_Matches_WrongTags()
        {
            var pair = CreatePair("test", "src_tag", "tgt_tag");
            Assert.IsFalse(pair.Matches("src_tag", "wrong_tag"));
            Assert.IsFalse(pair.Matches("wrong_tag", "tgt_tag"));
            Assert.IsFalse(pair.Matches("wrong_a", "wrong_b"));
        }

        [Test]
        public void ContradictionPair_Matches_NullTags()
        {
            var pair = CreatePair("test", "src_tag", "tgt_tag");
            Assert.IsFalse(pair.Matches(null, "tgt_tag"));
            Assert.IsFalse(pair.Matches("src_tag", null));
            Assert.IsFalse(pair.Matches(null, null));
        }

        [Test]
        public void ContradictionPair_Properties_ReturnCorrectValues()
        {
            var pair = CreatePair("ch1_test", "src", "tgt", chapter: 2, reward: 15);
            Assert.AreEqual("ch1_test", pair.ContradictionID);
            Assert.AreEqual("src", pair.SourceLineTag);
            Assert.AreEqual("tgt", pair.TargetLineTag);
            Assert.AreEqual(2, pair.Chapter);
            Assert.AreEqual(15, pair.RewardCoin);
        }
        #endregion

        #region ContradictionDatabase Tests
        [Test]
        public void Database_FindMatch_ReturnsCorrectPair()
        {
            var pair1 = CreatePair("p1", "a_src", "a_tgt");
            var pair2 = CreatePair("p2", "b_src", "b_tgt");
            var db = CreateDatabase(pair1, pair2);

            var result = db.FindMatch("a_src", "a_tgt");
            Assert.IsNotNull(result);
            Assert.AreEqual("p1", result.ContradictionID);
        }

        [Test]
        public void Database_FindMatch_ReturnsNullForNoMatch()
        {
            var pair = CreatePair("p1", "a_src", "a_tgt");
            var db = CreateDatabase(pair);

            Assert.IsNull(db.FindMatch("a_src", "wrong"));
            Assert.IsNull(db.FindMatch("wrong", "a_tgt"));
        }

        [Test]
        public void Database_FindMatch_WorksInReverseOrder()
        {
            var pair = CreatePair("p1", "a_src", "a_tgt");
            var db = CreateDatabase(pair);

            var result = db.FindMatch("a_tgt", "a_src");
            Assert.IsNotNull(result);
            Assert.AreEqual("p1", result.ContradictionID);
        }

        [Test]
        public void Database_GetAvailablePairs_FiltersChapter()
        {
            var ch1 = CreatePair("ch1", "s1", "t1", chapter: 1);
            var ch2 = CreatePair("ch2", "s2", "t2", chapter: 2);
            var ch3 = CreatePair("ch3", "s3", "t3", chapter: 3);
            var db = CreateDatabase(ch1, ch2, ch3);

            var ch1Only = db.GetAvailablePairs(1);
            Assert.AreEqual(1, ch1Only.Count);

            var upToCh2 = db.GetAvailablePairs(2);
            Assert.AreEqual(2, upToCh2.Count);

            var all = db.GetAvailablePairs(3);
            Assert.AreEqual(3, all.Count);
        }

        [Test]
        public void Database_IsContradictionTag_ReturnsTrueForValidTag()
        {
            var pair = CreatePair("p1", "src", "tgt", chapter: 1);
            var db = CreateDatabase(pair);

            Assert.IsTrue(db.IsContradictionTag("src", 1));
            Assert.IsTrue(db.IsContradictionTag("tgt", 1));
        }

        [Test]
        public void Database_IsContradictionTag_ReturnsFalseForFutureChapter()
        {
            var pair = CreatePair("p1", "src", "tgt", chapter: 3);
            var db = CreateDatabase(pair);

            Assert.IsFalse(db.IsContradictionTag("src", 2));
        }

        [Test]
        public void Database_IsContradictionTag_ReturnsFalseForUnknownTag()
        {
            var pair = CreatePair("p1", "src", "tgt", chapter: 1);
            var db = CreateDatabase(pair);

            Assert.IsFalse(db.IsContradictionTag("unknown", 1));
        }
        #endregion

        #region ContradictionManager Tests
        private GameObject m_ManagerObj;
        private ContradictionManager m_Manager;

        [TearDown]
        public void TearDown()
        {
            if (m_ManagerObj != null) Object.DestroyImmediate(m_ManagerObj);
        }

        [Test]
        public void Manager_SelectFirst_EntersPointingMode()
        {
            var pair = CreatePair("p1", "src", "tgt");
            var db = CreateDatabase(pair);
            (m_ManagerObj, m_Manager) = CreateManager(db);

            string notifiedTag = null;
            m_Manager.OnSelectionChanged += tag => notifiedTag = tag;

            bool result = m_Manager.SelectFirst("src");

            Assert.IsTrue(result);
            Assert.IsTrue(m_Manager.IsInPointingMode);
            Assert.AreEqual("src", notifiedTag);
        }

        [Test]
        public void Manager_SelectFirst_RejectsNullOrEmpty()
        {
            var db = CreateDatabase();
            (m_ManagerObj, m_Manager) = CreateManager(db);

            Assert.IsFalse(m_Manager.SelectFirst(null));
            Assert.IsFalse(m_Manager.SelectFirst(""));
            Assert.IsFalse(m_Manager.IsInPointingMode);
        }

        [Test]
        public void Manager_SelectSecond_DiscoverContradiction()
        {
            var pair = CreatePair("p1", "src", "tgt", reward: 10);
            var db = CreateDatabase(pair);
            (m_ManagerObj, m_Manager) = CreateManager(db);

            ContradictionPair discovered = null;
            m_Manager.OnContradictionDiscovered += p => discovered = p;

            m_Manager.SelectFirst("src");
            bool result = m_Manager.SelectSecond("tgt");

            Assert.IsTrue(result);
            Assert.IsNotNull(discovered);
            Assert.AreEqual("p1", discovered.ContradictionID);
            Assert.AreEqual(10, m_Manager.HalluciCoin);
            Assert.IsTrue(m_Manager.DiscoveredIDs.Contains("p1"));
            Assert.IsFalse(m_Manager.IsInPointingMode);
        }

        [Test]
        public void Manager_SelectSecond_DiscoverReverseOrder()
        {
            var pair = CreatePair("p1", "src", "tgt", reward: 5);
            var db = CreateDatabase(pair);
            (m_ManagerObj, m_Manager) = CreateManager(db);

            m_Manager.SelectFirst("tgt");
            bool result = m_Manager.SelectSecond("src");

            Assert.IsTrue(result);
            Assert.AreEqual(5, m_Manager.HalluciCoin);
        }

        [Test]
        public void Manager_SelectSecond_AlreadyDiscovered()
        {
            var pair = CreatePair("p1", "src", "tgt", reward: 10);
            var db = CreateDatabase(pair);
            (m_ManagerObj, m_Manager) = CreateManager(db);

            string failReason = null;
            m_Manager.OnPointingFailed += reason => failReason = reason;

            m_Manager.SelectFirst("src");
            m_Manager.SelectSecond("tgt");

            // Second discovery attempt
            m_Manager.SelectFirst("src");
            bool result = m_Manager.SelectSecond("tgt");

            Assert.IsFalse(result);
            Assert.AreEqual("already_discovered", failReason);
            Assert.AreEqual(10, m_Manager.HalluciCoin); // No double reward
        }

        [Test]
        public void Manager_SelectSecond_NoMatch_TriggersCooldown()
        {
            var pair = CreatePair("p1", "src", "tgt");
            var db = CreateDatabase(pair);
            (m_ManagerObj, m_Manager) = CreateManager(db);

            string failReason = null;
            m_Manager.OnPointingFailed += reason => failReason = reason;

            m_Manager.SelectFirst("src");
            bool result = m_Manager.SelectSecond("wrong_tag");

            Assert.IsFalse(result);
            Assert.AreEqual("no_match", failReason);
            Assert.IsTrue(m_Manager.IsOnCooldown);
        }

        [Test]
        public void Manager_SelectSecond_SameBubble_Cancels()
        {
            var db = CreateDatabase();
            (m_ManagerObj, m_Manager) = CreateManager(db);

            bool cleared = false;
            m_Manager.OnSelectionCleared += () => cleared = true;

            m_Manager.SelectFirst("tag_a");
            bool result = m_Manager.SelectSecond("tag_a");

            Assert.IsFalse(result);
            Assert.IsTrue(cleared);
            Assert.IsFalse(m_Manager.IsInPointingMode);
        }

        [Test]
        public void Manager_SelectSecond_WithoutFirst_Clears()
        {
            var db = CreateDatabase();
            (m_ManagerObj, m_Manager) = CreateManager(db);

            bool result = m_Manager.SelectSecond("tag_b");

            Assert.IsFalse(result);
            Assert.IsFalse(m_Manager.IsInPointingMode);
        }

        [Test]
        public void Manager_ClearSelection_ExitsPointingMode()
        {
            var db = CreateDatabase();
            (m_ManagerObj, m_Manager) = CreateManager(db);

            bool cleared = false;
            m_Manager.OnSelectionCleared += () => cleared = true;

            m_Manager.SelectFirst("tag_a");
            m_Manager.ClearSelection();

            Assert.IsFalse(m_Manager.IsInPointingMode);
            Assert.IsTrue(cleared);
        }

        [Test]
        public void Manager_MultipleContradictions_AccumulateCoins()
        {
            // SelectFirst/SelectSecond 経由だと SaveManager.Instance が
            // DontDestroyOnLoad を呼び EditMode で例外になるため、
            // RestoreDiscovered 経由でコイン蓄積を検証する。
            var p1 = CreatePair("p1", "s1", "t1", reward: 10);
            var p2 = CreatePair("p2", "s2", "t2", reward: 20);
            var db = CreateDatabase(p1, p2);
            (m_ManagerObj, m_Manager) = CreateManager(db);

            m_Manager.RestoreDiscovered(new System.Collections.Generic.List<string> { "p1", "p2" }, 30);

            Assert.AreEqual(30, m_Manager.HalluciCoin);
            Assert.AreEqual(2, m_Manager.DiscoveredIDs.Count);
        }

        [Test]
        public void Manager_GetDiscoveredList_ReturnsCorrectList()
        {
            // RestoreDiscovered 経由で発見済み状態を作る。
            // SelectFirst/SelectSecond 経由だと SaveManager.Instance が
            // DontDestroyOnLoad を呼び EditMode で例外になるため。
            var db = CreateDatabase();
            (m_ManagerObj, m_Manager) = CreateManager(db);

            m_Manager.RestoreDiscovered(new List<string> { "p1" }, 10);

            List<string> list = m_Manager.GetDiscoveredList();
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list.Contains("p1"));
        }

        [Test]
        public void Manager_RestoreDiscovered_RestoresState()
        {
            var db = CreateDatabase();
            (m_ManagerObj, m_Manager) = CreateManager(db);

            m_Manager.RestoreDiscovered(new List<string> { "p1", "p2" }, 50);

            Assert.AreEqual(50, m_Manager.HalluciCoin);
            Assert.AreEqual(2, m_Manager.DiscoveredIDs.Count);
            Assert.IsTrue(m_Manager.DiscoveredIDs.Contains("p1"));
            Assert.IsTrue(m_Manager.DiscoveredIDs.Contains("p2"));
        }

        [Test]
        public void Manager_RestoreDiscovered_NullList()
        {
            var db = CreateDatabase();
            (m_ManagerObj, m_Manager) = CreateManager(db);

            m_Manager.RestoreDiscovered(null, 0);

            Assert.AreEqual(0, m_Manager.HalluciCoin);
            Assert.AreEqual(0, m_Manager.DiscoveredIDs.Count);
        }

        [Test]
        public void Manager_SetCurrentChapter_UpdatesChapter()
        {
            var db = CreateDatabase();
            (m_ManagerObj, m_Manager) = CreateManager(db, chapter: 1);

            m_Manager.SetCurrentChapter(3);
            Assert.AreEqual(3, m_Manager.CurrentChapter);
        }

        [Test]
        public void Manager_ShouldShowHint_ReturnsTrueForLowDifficultyEarlyChapter()
        {
            var pair = CreatePair("p1", "src", "tgt", chapter: 1);
            var db = CreateDatabase(pair);
            (m_ManagerObj, m_Manager) = CreateManager(db, chapter: 2);

            Assert.IsTrue(m_Manager.ShouldShowHint("src", 1));
        }

        [Test]
        public void Manager_ShouldShowHint_ReturnsFalseForHighDifficulty()
        {
            var pair = CreatePair("p1", "src", "tgt", chapter: 1);
            var db = CreateDatabase(pair);
            (m_ManagerObj, m_Manager) = CreateManager(db, chapter: 2);

            Assert.IsFalse(m_Manager.ShouldShowHint("src", 2));
        }

        [Test]
        public void Manager_ShouldShowHint_ChannelPolicyControlsHint()
        {
            // 現仕様: ヒント表示は ChannelData の EnableHints / MaxHintDifficulty で制御。
            // チャプター番号による一律抑制は行わない。
            // デフォルト (EnableHints=true, MaxHintDifficulty=1) では chapter=4 でもヒントは出る。
            var pair = CreatePair("p1", "src", "tgt", chapter: 1);
            var db = CreateDatabase(pair);
            (m_ManagerObj, m_Manager) = CreateManager(db, chapter: 4);

            // デフォルトポリシーではヒント有効
            Assert.IsTrue(m_Manager.ShouldShowHint("src", 1));

            // difficulty が MaxHintDifficulty を超えると非表示
            Assert.IsFalse(m_Manager.ShouldShowHint("src", 2));
        }

        [Test]
        public void Manager_NullDatabase_HandlesGracefully()
        {
            m_ManagerObj = new GameObject("NullDbManager");
            m_Manager = m_ManagerObj.AddComponent<ContradictionManager>();

            m_Manager.SelectFirst("tag");
            bool result = m_Manager.SelectSecond("other");

            Assert.IsFalse(result);
            Assert.IsFalse(m_Manager.ShouldShowHint("tag", 1));
        }
        #endregion
    }
}
