using System.Collections.Generic;
using NUnit.Framework;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.UI;
using UnityEditor;
using UnityEngine;

namespace ProjectFoundPhone.Tests
{
    public class InventoryTabControllerTests
    {
        private GameObject m_CanvasObj;
        private GameObject m_BoardObj;
        private DeductionBoard m_Board;
        private GameObject m_InventoryObj;
        private InventoryTabController m_InventoryTab;
        private readonly List<TopicData> m_CreatedTopics = new List<TopicData>();

        [SetUp]
        public void SetUp()
        {
            m_CanvasObj = new GameObject("TestCanvas", typeof(Canvas));

            m_BoardObj = new GameObject("TestDeductionBoard");
            m_Board = m_BoardObj.AddComponent<DeductionBoard>();

            GameObject container = new GameObject("CardContainer");
            container.transform.SetParent(m_BoardObj.transform);

            GameObject cardPrefabObj = new GameObject("TopicCardPrefab");
            cardPrefabObj.transform.SetParent(m_BoardObj.transform);
            TopicCard topicCardPrefab = cardPrefabObj.AddComponent<TopicCard>();

            SerializedObject boardSo = new SerializedObject(m_Board);
            boardSo.FindProperty("m_CardContainer").objectReferenceValue = container.transform;
            boardSo.FindProperty("m_TopicCardPrefab").objectReferenceValue = topicCardPrefab;
            boardSo.ApplyModifiedPropertiesWithoutUndo();

            m_InventoryObj = new GameObject("TestInventoryTab");
            m_InventoryObj.transform.SetParent(m_CanvasObj.transform, false);
            m_InventoryTab = m_InventoryObj.AddComponent<InventoryTabController>();
            m_InventoryTab.BuildUI(m_CanvasObj.transform);
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < m_CreatedTopics.Count; i++)
            {
                if (m_CreatedTopics[i] != null)
                {
                    Object.DestroyImmediate(m_CreatedTopics[i]);
                }
            }
            m_CreatedTopics.Clear();

            if (m_InventoryObj != null) Object.DestroyImmediate(m_InventoryObj);
            if (m_BoardObj != null) Object.DestroyImmediate(m_BoardObj);
            if (m_CanvasObj != null) Object.DestroyImmediate(m_CanvasObj);
        }

        [Test]
        public void BuildUI_DefaultsToFragmentsTab()
        {
            Assert.IsTrue(m_InventoryTab.IsBuilt);
            Assert.AreEqual(InventoryTabController.InventorySubTab.Fragments, m_InventoryTab.CurrentSubTab);
            Assert.AreEqual(0, m_InventoryTab.FragmentItemCount);
            Assert.AreEqual(0, m_InventoryTab.TopicItemCount);
        }

        [Test]
        public void RefreshFragments_OnlyShowsFragmentIds()
        {
            AddTopics(
                CreateTopic("fragment_ch1_01", "Fragment"),
                CreateTopic("topic_found_phone", "Topic"),
                CreateTopic("T_FoundPhone", "Legacy Topic"),
                CreateTopic("debug_topic_01", "Debug Topic"));

            m_InventoryTab.Show();

            Assert.AreEqual(1, m_InventoryTab.FragmentItemCount);
        }

        [Test]
        public void TopicsTab_IncludesLegacyAndDebugTopicIds()
        {
            AddTopics(
                CreateTopic("fragment_ch1_01", "Fragment"),
                CreateTopic("topic_found_phone", "Topic"),
                CreateTopic("T_FoundPhone", "Legacy Topic"),
                CreateTopic("debug_topic_01", "Debug Topic"));

            m_InventoryTab.SelectSubTab(InventoryTabController.InventorySubTab.Topics);

            Assert.AreEqual(InventoryTabController.InventorySubTab.Topics, m_InventoryTab.CurrentSubTab);
            Assert.AreEqual(3, m_InventoryTab.TopicItemCount);
        }

        [Test]
        public void Refresh_DoesNotDuplicateEntriesAcrossCalls()
        {
            AddTopics(CreateTopic("fragment_ch1_01", "Fragment 1"));

            m_InventoryTab.Show();
            Assert.AreEqual(1, m_InventoryTab.FragmentItemCount);

            m_InventoryTab.Refresh();
            Assert.AreEqual(1, m_InventoryTab.FragmentItemCount);

            AddTopics(CreateTopic("fragment_ch1_02", "Fragment 2"));
            m_InventoryTab.Refresh();

            Assert.AreEqual(2, m_InventoryTab.FragmentItemCount);
        }

        private void AddTopics(params TopicData[] topics)
        {
            for (int i = 0; i < topics.Length; i++)
            {
                m_Board.AddTopic(topics[i]);
            }
        }

        private TopicData CreateTopic(string id, string title, string description = "")
        {
            TopicData topic = ScriptableObject.CreateInstance<TopicData>();
            m_CreatedTopics.Add(topic);

            SerializedObject so = new SerializedObject(topic);
            so.FindProperty("m_TopicID").stringValue = id;
            so.FindProperty("m_Title").stringValue = title;
            so.FindProperty("m_Description").stringValue = description;
            so.ApplyModifiedPropertiesWithoutUndo();

            return topic;
        }
    }
}
