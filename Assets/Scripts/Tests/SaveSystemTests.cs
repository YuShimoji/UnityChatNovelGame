using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Tests
{
    /// <summary>
    /// SaveSystemの単体テスト
    /// </summary>
    public class SaveSystemTests
    {
        private GameObject m_SaveManagerObject;
        private SaveManager m_SaveManager;

        [SetUp]
        public void Setup()
        {
            m_SaveManagerObject = new GameObject("SaveManager");
            m_SaveManager = m_SaveManagerObject.AddComponent<SaveManager>();
        }

        [TearDown]
        public void Teardown()
        {
            if (m_SaveManagerObject != null)
            {
                Object.DestroyImmediate(m_SaveManagerObject);
            }

            for (int i = 0; i < 3; i++)
            {
                string filePath = Path.Combine(Application.persistentDataPath, $"SaveData_{i}.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void SaveData_Constructor_SetsDefaultValues()
        {
            SaveData saveData = new SaveData(0);

            Assert.AreEqual(1, saveData.Version);
            Assert.AreEqual(0, saveData.SlotNumber);
            Assert.IsNotNull(saveData.SaveDateTime);
            Assert.IsNotNull(saveData.UnlockedTopicIDs);
            Assert.IsNotNull(saveData.YarnVariables);
        }

        [Test]
        public void SaveData_IsValid_ReturnsTrueForValidData()
        {
            SaveData saveData = new SaveData(0);

            Assert.IsTrue(saveData.IsValid());
        }

        [Test]
        public void SaveData_GetSummary_ReturnsFormattedString()
        {
            SaveData saveData = new SaveData(0);
            saveData.UnlockedTopicIDs.Add("topic1");
            saveData.UnlockedTopicIDs.Add("topic2");

            string summary = saveData.GetSummary();

            Assert.IsTrue(summary.Contains("Slot 0"));
            Assert.IsTrue(summary.Contains("Topics: 2"));
        }

        [UnityTest]
        public IEnumerator SaveManager_SaveGame_CreatesFile()
        {
            yield return null;

            bool success = m_SaveManager.SaveGame(0);

            Assert.IsTrue(success);
            Assert.IsTrue(m_SaveManager.HasSaveInSlot(0));
        }

        [UnityTest]
        public IEnumerator SaveManager_LoadGame_RestoresData()
        {
            yield return null;

            m_SaveManager.SaveGame(0);
            bool success = m_SaveManager.LoadGame(0);

            Assert.IsTrue(success);
            Assert.IsNotNull(m_SaveManager.CurrentSaveData);
        }

        [UnityTest]
        public IEnumerator SaveManager_DeleteSave_RemovesFile()
        {
            yield return null;

            m_SaveManager.SaveGame(0);
            Assert.IsTrue(m_SaveManager.HasSaveInSlot(0));

            bool success = m_SaveManager.DeleteSave(0);

            Assert.IsTrue(success);
            Assert.IsFalse(m_SaveManager.HasSaveInSlot(0));
        }

        [Test]
        public void SaveManager_GetSaveInfo_ReturnsNullForEmptySlot()
        {
            SaveData saveInfo = m_SaveManager.GetSaveInfo(0);

            Assert.IsNull(saveInfo);
        }

        [UnityTest]
        public IEnumerator SaveManager_GetAllSaveInfo_ReturnsAllSlots()
        {
            yield return null;

            m_SaveManager.SaveGame(0);
            m_SaveManager.SaveGame(2);

            SaveData[] allSaves = m_SaveManager.GetAllSaveInfo();

            Assert.AreEqual(3, allSaves.Length);
            Assert.IsNotNull(allSaves[0]);
            Assert.IsNull(allSaves[1]);
            Assert.IsNotNull(allSaves[2]);
        }
    }
}
