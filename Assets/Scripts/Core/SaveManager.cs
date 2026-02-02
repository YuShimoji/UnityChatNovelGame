using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.UI;
using Yarn.Unity;

namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// ゲームのセーブ・ロード機能を管理するマネージャークラス
    /// シングルトンパターンで実装
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        #region Singleton
        private static SaveManager s_Instance;

        /// <summary>
        /// SaveManagerのシングルトンインスタンス
        /// </summary>
        public static SaveManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<SaveManager>();
                    if (s_Instance == null)
                    {
                        GameObject go = new GameObject("SaveManager");
                        s_Instance = go.AddComponent<SaveManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return s_Instance;
            }
        }
        #endregion

        #region Private Fields
        [Header("Settings")]
        [SerializeField] private string m_SaveFilePrefix = "SaveData";
        [SerializeField] private string m_SaveFileExtension = ".json";
        [SerializeField] private int m_MaxSaveSlots = 3;

        /// <summary>
        /// 現在ロードされているセーブデータ
        /// </summary>
        private SaveData m_CurrentSaveData;
        #endregion

        #region Public Properties
        /// <summary>
        /// 現在のセーブデータ（読み取り専用）
        /// </summary>
        public SaveData CurrentSaveData => m_CurrentSaveData;

        /// <summary>
        /// セーブデータが存在するかどうか
        /// </summary>
        public bool HasSaveData => m_CurrentSaveData != null;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }
        #endregion

        #region Public Methods - Save
        /// <summary>
        /// 現在のゲーム状態をセーブする
        /// </summary>
        /// <param name="slotNumber">セーブスロット番号（0から開始）</param>
        /// <returns>セーブ成功時true</returns>
        public bool SaveGame(int slotNumber = 0)
        {
            if (slotNumber < 0 || slotNumber >= m_MaxSaveSlots)
            {
                Debug.LogError($"SaveManager: Invalid slot number {slotNumber}. Must be between 0 and {m_MaxSaveSlots - 1}.");
                return false;
            }

            try
            {
                SaveData saveData = CreateSaveData(slotNumber);
                string json = JsonUtility.ToJson(saveData, true);
                string filePath = GetSaveFilePath(slotNumber);

                File.WriteAllText(filePath, json);
                m_CurrentSaveData = saveData;

                Debug.Log($"SaveManager: Game saved to slot {slotNumber} at {filePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveManager: Failed to save game - {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 現在のゲーム状態からSaveDataを作成
        /// </summary>
        /// <param name="slotNumber">セーブスロット番号</param>
        /// <returns>作成されたSaveData</returns>
        private SaveData CreateSaveData(int slotNumber)
        {
            SaveData saveData = new SaveData(slotNumber);

            ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null)
            {
                saveData.CurrentNodeName = GetCurrentNodeName(scenarioManager);
                saveData.YarnVariables = GetYarnVariables(scenarioManager);
            }
            else
            {
                Debug.LogWarning("SaveManager: ScenarioManager not found. Scenario progress will not be saved.");
            }

            DeductionBoard deductionBoard = DeductionBoard.Instance;
            if (deductionBoard != null)
            {
                foreach (var topic in deductionBoard.UnlockedTopics)
                {
                    if (topic != null)
                    {
                        saveData.UnlockedTopicIDs.Add(topic.TopicID);
                    }
                }
            }
            else
            {
                Debug.LogWarning("SaveManager: DeductionBoard not found. Topic progress will not be saved.");
            }

            return saveData;
        }

        /// <summary>
        /// ScenarioManagerから現在のノード名を取得
        /// </summary>
        private string GetCurrentNodeName(ScenarioManager scenarioManager)
        {
            return scenarioManager.GetVariable<string>("current_node") ?? "Start";
        }

        /// <summary>
        /// ScenarioManagerからYarn変数を取得
        /// </summary>
        private Dictionary<string, object> GetYarnVariables(ScenarioManager scenarioManager)
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            
            DeductionBoard deductionBoard = DeductionBoard.Instance;
            if (deductionBoard != null)
            {
                foreach (var topic in deductionBoard.UnlockedTopics)
                {
                    if (topic != null)
                    {
                        string varName = $"has_topic_{topic.TopicID}";
                        bool hasTopicVar = scenarioManager.GetVariable<bool>(varName);
                        if (hasTopicVar)
                        {
                            variables[varName] = hasTopicVar;
                        }
                    }
                }
            }
            
            return variables;
        }
        #endregion

        #region Public Methods - Load
        /// <summary>
        /// 指定スロットからゲームをロードする
        /// </summary>
        /// <param name="slotNumber">ロードするセーブスロット番号</param>
        /// <returns>ロード成功時true</returns>
        public bool LoadGame(int slotNumber = 0)
        {
            if (slotNumber < 0 || slotNumber >= m_MaxSaveSlots)
            {
                Debug.LogError($"SaveManager: Invalid slot number {slotNumber}. Must be between 0 and {m_MaxSaveSlots - 1}.");
                return false;
            }

            string filePath = GetSaveFilePath(slotNumber);
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"SaveManager: Save file not found at {filePath}");
                return false;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData == null || !saveData.IsValid())
                {
                    Debug.LogError("SaveManager: Loaded save data is invalid.");
                    return false;
                }

                ApplySaveData(saveData);
                m_CurrentSaveData = saveData;

                Debug.Log($"SaveManager: Game loaded from slot {slotNumber}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveManager: Failed to load game - {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// SaveDataをゲーム状態に適用
        /// </summary>
        /// <param name="saveData">適用するセーブデータ</param>
        private void ApplySaveData(SaveData saveData)
        {
            DeductionBoard deductionBoard = DeductionBoard.Instance;
            if (deductionBoard != null)
            {
                deductionBoard.ClearAllTopics();

                foreach (string topicID in saveData.UnlockedTopicIDs)
                {
                    TopicData topicData = Resources.Load<TopicData>($"Topics/{topicID}");
                    if (topicData != null)
                    {
                        deductionBoard.AddTopic(topicData);
                    }
                    else
                    {
                        Debug.LogWarning($"SaveManager: Failed to load TopicData for ID: {topicID}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("SaveManager: DeductionBoard not found. Topics will not be restored.");
            }

            ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null)
            {
                foreach (var kvp in saveData.YarnVariables)
                {
                    if (kvp.Value is bool boolValue)
                    {
                        scenarioManager.SetVariable(kvp.Key, boolValue);
                    }
                    else if (kvp.Value is string stringValue)
                    {
                        scenarioManager.SetVariable(kvp.Key, stringValue);
                    }
                    else if (kvp.Value is double || kvp.Value is float)
                    {
                        scenarioManager.SetVariable(kvp.Key, Convert.ToSingle(kvp.Value));
                    }
                }

                if (!string.IsNullOrEmpty(saveData.CurrentNodeName))
                {
                    scenarioManager.StartScenario(saveData.CurrentNodeName);
                }
            }
            else
            {
                Debug.LogWarning("SaveManager: ScenarioManager not found. Scenario state will not be restored.");
            }
        }
        #endregion

        #region Public Methods - Delete
        /// <summary>
        /// 指定スロットのセーブデータを削除
        /// </summary>
        /// <param name="slotNumber">削除するセーブスロット番号</param>
        /// <returns>削除成功時true</returns>
        public bool DeleteSave(int slotNumber)
        {
            if (slotNumber < 0 || slotNumber >= m_MaxSaveSlots)
            {
                Debug.LogError($"SaveManager: Invalid slot number {slotNumber}. Must be between 0 and {m_MaxSaveSlots - 1}.");
                return false;
            }

            string filePath = GetSaveFilePath(slotNumber);
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"SaveManager: Save file not found at {filePath}");
                return false;
            }

            try
            {
                File.Delete(filePath);
                if (m_CurrentSaveData != null && m_CurrentSaveData.SlotNumber == slotNumber)
                {
                    m_CurrentSaveData = null;
                }
                Debug.Log($"SaveManager: Save deleted from slot {slotNumber}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveManager: Failed to delete save - {e.Message}");
                return false;
            }
        }
        #endregion

        #region Public Methods - Utility
        /// <summary>
        /// 指定スロットにセーブデータが存在するか確認
        /// </summary>
        /// <param name="slotNumber">確認するセーブスロット番号</param>
        /// <returns>セーブデータが存在する場合true</returns>
        public bool HasSaveInSlot(int slotNumber)
        {
            if (slotNumber < 0 || slotNumber >= m_MaxSaveSlots)
            {
                return false;
            }

            string filePath = GetSaveFilePath(slotNumber);
            return File.Exists(filePath);
        }

        /// <summary>
        /// 指定スロットのセーブデータ情報を取得（ロードせずに）
        /// </summary>
        /// <param name="slotNumber">情報を取得するセーブスロット番号</param>
        /// <returns>セーブデータ、存在しない場合null</returns>
        public SaveData GetSaveInfo(int slotNumber)
        {
            if (slotNumber < 0 || slotNumber >= m_MaxSaveSlots)
            {
                return null;
            }

            string filePath = GetSaveFilePath(slotNumber);
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveManager: Failed to read save info - {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 全てのセーブスロット情報を取得
        /// </summary>
        /// <returns>セーブデータの配列（存在しないスロットはnull）</returns>
        public SaveData[] GetAllSaveInfo()
        {
            SaveData[] saves = new SaveData[m_MaxSaveSlots];
            for (int i = 0; i < m_MaxSaveSlots; i++)
            {
                saves[i] = GetSaveInfo(i);
            }
            return saves;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// セーブファイルのパスを取得
        /// </summary>
        /// <param name="slotNumber">セーブスロット番号</param>
        /// <returns>セーブファイルの絶対パス</returns>
        private string GetSaveFilePath(int slotNumber)
        {
            string fileName = $"{m_SaveFilePrefix}_{slotNumber}{m_SaveFileExtension}";
            return Path.Combine(Application.persistentDataPath, fileName);
        }
        #endregion
    }
}
