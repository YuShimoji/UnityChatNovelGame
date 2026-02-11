using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.UI;


namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// 繧ｲ繝ｼ繝縺ｮ繧ｻ繝ｼ繝悶・繝ｭ繝ｼ繝画ｩ溯・繧堤ｮ｡逅・☆繧九・繝阪・繧ｸ繝｣繝ｼ繧ｯ繝ｩ繧ｹ
    /// 繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ繝代ち繝ｼ繝ｳ縺ｧ螳溯｣・
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        #region Singleton
        private static SaveManager s_Instance;

        /// <summary>
        /// SaveManager縺ｮ繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ
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
        /// 迴ｾ蝨ｨ繝ｭ繝ｼ繝峨＆繧後※縺・ｋ繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ
        /// </summary>
        private SaveData m_CurrentSaveData;
        #endregion

        #region Public Properties
        /// <summary>
        /// 迴ｾ蝨ｨ縺ｮ繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ・郁ｪｭ縺ｿ蜿悶ｊ蟆ら畑・・
        /// </summary>
        public SaveData CurrentSaveData => m_CurrentSaveData;

        /// <summary>
        /// 繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ縺悟ｭ伜惠縺吶ｋ縺九←縺・°
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
        /// 迴ｾ蝨ｨ縺ｮ繧ｲ繝ｼ繝迥ｶ諷九ｒ繧ｻ繝ｼ繝悶☆繧・
        /// </summary>
        /// <param name="slotNumber">繧ｻ繝ｼ繝悶せ繝ｭ繝・ヨ逡ｪ蜿ｷ・・縺九ｉ髢句ｧ具ｼ・/param>
        /// <returns>繧ｻ繝ｼ繝匁・蜉滓凾true</returns>
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
                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
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
        /// 迴ｾ蝨ｨ縺ｮ繧ｲ繝ｼ繝迥ｶ諷九°繧唄aveData繧剃ｽ懈・
        /// </summary>
        /// <param name="slotNumber">繧ｻ繝ｼ繝悶せ繝ｭ繝・ヨ逡ｪ蜿ｷ</param>
        /// <returns>菴懈・縺輔ｌ縺欖aveData</returns>
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
        /// ScenarioManager縺九ｉ迴ｾ蝨ｨ縺ｮ繝弱・繝牙錐繧貞叙蠕・
        /// </summary>
        private string GetCurrentNodeName(ScenarioManager scenarioManager)
        {
            return scenarioManager.GetVariable<string>("$current_node") ?? "Start";
        }

        /// <summary>
        /// ScenarioManager縺九ｉYarn螟画焚繧貞叙蠕・
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
                        string varName = $"$has_topic_{topic.TopicID}";
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
        /// 謖・ｮ壹せ繝ｭ繝・ヨ縺九ｉ繧ｲ繝ｼ繝繧偵Ο繝ｼ繝峨☆繧・
        /// </summary>
        /// <param name="slotNumber">繝ｭ繝ｼ繝峨☆繧九そ繝ｼ繝悶せ繝ｭ繝・ヨ逡ｪ蜿ｷ</param>
        /// <returns>繝ｭ繝ｼ繝画・蜉滓凾true</returns>
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
                SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);

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
        /// SaveData繧偵ご繝ｼ繝迥ｶ諷九↓驕ｩ逕ｨ
        /// </summary>
        /// <param name="saveData">驕ｩ逕ｨ縺吶ｋ繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ</param>
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
                    if (kvp.Value is JToken jToken)
                    {
                        switch (jToken.Type)
                        {
                            case JTokenType.Boolean:
                                scenarioManager.SetVariable(kvp.Key, jToken.Value<bool>());
                                break;
                            case JTokenType.String:
                                scenarioManager.SetVariable(kvp.Key, jToken.Value<string>());
                                break;
                            case JTokenType.Float:
                            case JTokenType.Integer:
                                scenarioManager.SetVariable(kvp.Key, jToken.Value<float>());
                                break;
                            default:
                                Debug.LogWarning($"SaveManager: Unsupported Yarn variable type for '{kvp.Key}': {jToken.Type}");
                                break;
                        }
                    }
                    else if (kvp.Value is bool boolValue)
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
        /// 謖・ｮ壹せ繝ｭ繝・ヨ縺ｮ繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ繧貞炎髯､
        /// </summary>
        /// <param name="slotNumber">蜑企勁縺吶ｋ繧ｻ繝ｼ繝悶せ繝ｭ繝・ヨ逡ｪ蜿ｷ</param>
        /// <returns>蜑企勁謌仙粥譎Ｕrue</returns>
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
        /// 謖・ｮ壹せ繝ｭ繝・ヨ縺ｫ繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ縺悟ｭ伜惠縺吶ｋ縺狗｢ｺ隱・
        /// </summary>
        /// <param name="slotNumber">遒ｺ隱阪☆繧九そ繝ｼ繝悶せ繝ｭ繝・ヨ逡ｪ蜿ｷ</param>
        /// <returns>繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ縺悟ｭ伜惠縺吶ｋ蝣ｴ蜷・rue</returns>
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
        /// 謖・ｮ壹せ繝ｭ繝・ヨ縺ｮ繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ諠・ｱ繧貞叙蠕暦ｼ医Ο繝ｼ繝峨○縺壹↓・・
        /// </summary>
        /// <param name="slotNumber">諠・ｱ繧貞叙蠕励☆繧九そ繝ｼ繝悶せ繝ｭ繝・ヨ逡ｪ蜿ｷ</param>
        /// <returns>繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ縲∝ｭ伜惠縺励↑縺・ｴ蜷・ull</returns>
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
                return JsonConvert.DeserializeObject<SaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveManager: Failed to read save info - {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 蜈ｨ縺ｦ縺ｮ繧ｻ繝ｼ繝悶せ繝ｭ繝・ヨ諠・ｱ繧貞叙蠕・
        /// </summary>
        /// <returns>繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ縺ｮ驟榊・・亥ｭ伜惠縺励↑縺・せ繝ｭ繝・ヨ縺ｯnull・・/returns>
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
        /// 繧ｻ繝ｼ繝悶ヵ繧｡繧､繝ｫ縺ｮ繝代せ繧貞叙蠕・
        /// </summary>
        /// <param name="slotNumber">繧ｻ繝ｼ繝悶せ繝ｭ繝・ヨ逡ｪ蜿ｷ</param>
        /// <returns>繧ｻ繝ｼ繝悶ヵ繧｡繧､繝ｫ縺ｮ邨ｶ蟇ｾ繝代せ</returns>
        private string GetSaveFilePath(int slotNumber)
        {
            string fileName = $"{m_SaveFilePrefix}_{slotNumber}{m_SaveFileExtension}";
            return Path.Combine(Application.persistentDataPath, fileName);
        }
        #endregion
    }
}


