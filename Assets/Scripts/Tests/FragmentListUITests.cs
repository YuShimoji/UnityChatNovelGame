using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.Tests
{
    /// <summary>
    /// FragmentListUI のロジックテスト（EditMode）
    /// </summary>
    public class FragmentListUITests
    {
        private GameObject m_CanvasObj;
        private GameObject m_BoardObj;
        private DeductionBoard m_Board;
        private GameObject m_FragmentListObj;
        private FragmentListUI m_FragmentListUI;

        [SetUp]
        public void SetUp()
        {
            // Canvas (FragmentListUI.BuildUI が FindFirstObjectByType<Canvas>() を使うため必要)
            m_CanvasObj = new GameObject("TestCanvas", typeof(Canvas));

            // DeductionBoard (Singleton として動作する)
            m_BoardObj = new GameObject("TestDeductionBoard");
            m_Board = m_BoardObj.AddComponent<DeductionBoard>();

            // CardContainer (DeductionBoard.AddTopic が内部で使うため必要)
            GameObject container = new GameObject("CardContainer");
            container.transform.SetParent(m_BoardObj.transform);

            GameObject cardPrefabObj = new GameObject("TopicCardPrefab");
            cardPrefabObj.transform.SetParent(m_BoardObj.transform);
            TopicCard topicCardPrefab = cardPrefabObj.AddComponent<TopicCard>();

            SerializedObject boardSo = new SerializedObject(m_Board);
            boardSo.FindProperty("m_CardContainer").objectReferenceValue = container.transform;
            boardSo.FindProperty("m_TopicCardPrefab").objectReferenceValue = topicCardPrefab;
            boardSo.ApplyModifiedPropertiesWithoutUndo();

            // FragmentListUI
            m_FragmentListObj = new GameObject("TestFragmentListUI");
            m_FragmentListObj.transform.SetParent(m_CanvasObj.transform, false);
            m_FragmentListUI = m_FragmentListObj.AddComponent<FragmentListUI>();
        }

        [TearDown]
        public void TearDown()
        {
            if (m_FragmentListObj != null) Object.DestroyImmediate(m_FragmentListObj);
            if (m_BoardObj != null) Object.DestroyImmediate(m_BoardObj);
            if (m_CanvasObj != null) Object.DestroyImmediate(m_CanvasObj);
        }

        private TopicData CreateTopic(string id, string title, string description = "")
        {
            TopicData topic = ScriptableObject.CreateInstance<TopicData>();
            SerializedObject so = new SerializedObject(topic);
            so.FindProperty("m_TopicID").stringValue = id;
            so.FindProperty("m_Title").stringValue = title;
            so.FindProperty("m_Description").stringValue = description;
            so.ApplyModifiedPropertiesWithoutUndo();
            return topic;
        }

        [Test]
        public void Show_BuildsPanelOnFirstCall()
        {
            Assert.IsFalse(m_FragmentListUI.IsPanelBuilt);

            m_FragmentListUI.Show();

            Assert.IsTrue(m_FragmentListUI.IsPanelBuilt);
            Assert.IsTrue(m_FragmentListUI.IsShowing);
        }

        [Test]
        public void Hide_SetsPanelInactive()
        {
            m_FragmentListUI.Show();
            m_FragmentListUI.Hide();

            Assert.IsFalse(m_FragmentListUI.IsShowing);
        }

        [Test]
        public void Toggle_SwitchesVisibility()
        {
            m_FragmentListUI.Toggle();
            Assert.IsTrue(m_FragmentListUI.IsShowing);

            m_FragmentListUI.Toggle();
            Assert.IsFalse(m_FragmentListUI.IsShowing);
        }

        [Test]
        public void RefreshList_EmptyBoard_ShowsZeroItems()
        {
            m_FragmentListUI.Show();

            Assert.AreEqual(0, m_FragmentListUI.ItemCount);
        }

        [Test]
        public void RefreshList_WithTopics_CreatesCorrectItemCount()
        {
            m_Board.AddTopic(CreateTopic("f1", "Fragment 1", "Description 1"));
            m_Board.AddTopic(CreateTopic("f2", "Fragment 2", "Description 2"));
            m_Board.AddTopic(CreateTopic("f3", "Fragment 3", "Description 3"));

            m_FragmentListUI.Show();

            Assert.AreEqual(3, m_FragmentListUI.ItemCount);
        }

        [Test]
        public void RefreshList_AfterNewTopic_UpdatesCount()
        {
            m_Board.AddTopic(CreateTopic("f1", "Fragment 1"));
            m_FragmentListUI.Show();
            Assert.AreEqual(1, m_FragmentListUI.ItemCount);

            m_Board.AddTopic(CreateTopic("f2", "Fragment 2"));
            m_FragmentListUI.RefreshList();
            Assert.AreEqual(2, m_FragmentListUI.ItemCount);
        }

        [Test]
        public void RefreshList_AfterClear_ReturnsToZero()
        {
            m_Board.AddTopic(CreateTopic("f1", "Fragment 1"));
            m_Board.AddTopic(CreateTopic("f2", "Fragment 2"));
            m_FragmentListUI.Show();
            Assert.AreEqual(2, m_FragmentListUI.ItemCount);

            m_Board.ClearAllTopics();
            m_FragmentListUI.RefreshList();
            Assert.AreEqual(0, m_FragmentListUI.ItemCount);
        }
    }
}
