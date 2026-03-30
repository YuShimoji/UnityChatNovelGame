using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Yarn.Unity;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.UI;


namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// ゲームのセーブ/ロード機能を管理するマネージャークラス
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

        #region Constants
        /// <summary>
        /// オートセーブ専用スロット番号（通常スロットとは別管理）
        /// </summary>
        public const int AutoSaveSlot = 99;

        /// <summary>
        /// オートセーブのクールダウン秒数
        /// </summary>
        private const float AutoSaveCooldownSeconds = 30f;

        /// <summary>
        /// オートセーブインジケーターの表示秒数
        /// </summary>
        private const float AutoSaveIndicatorDuration = 1f;
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

        /// <summary>
        /// 最後にオートセーブが実行された時刻
        /// </summary>
        private float m_LastAutoSaveTime = -AutoSaveCooldownSeconds;

        /// <summary>
        /// オートセーブインジケーターUI（Canvas直下に動的生成）
        /// </summary>
        private GameObject m_AutoSaveIndicator;

        /// <summary>
        /// DeductionBoard不在警告を既に出力したか
        /// </summary>
        private bool m_DeductionBoardWarningShown;
        private CanvasGroup m_AutoSaveCanvasGroup;
        private Coroutine m_AutoSaveIndicatorCoroutine;
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
            if (!IsValidSlot(slotNumber))
            {
                Debug.LogError($"SaveManager: Invalid slot number {slotNumber}. Must be between 0 and {m_MaxSaveSlots - 1}, or {AutoSaveSlot} (auto-save).");
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
                Debug.LogError($"SaveManager: Failed to save game - {e.Message}\n{e.StackTrace}");
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
                saveData.BranchThread = scenarioManager.GetBranchThreadStateSnapshot();
                saveData.CurrentChannelID = scenarioManager.CurrentChannelID;
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
                if (!m_DeductionBoardWarningShown)
                {
                    Debug.LogWarning("SaveManager: DeductionBoard not found. Topic progress will not be saved.");
                    m_DeductionBoardWarningShown = true;
                }
            }

            // チャット履歴を保存
            // サブスレッド表示中でもメインスレッドの履歴を確実に保存する
            ChatController chatController = FindFirstObjectByType<ChatController>();
            if (chatController != null)
            {
                saveData.ActiveThreadId = chatController.ActiveThreadId;
                var allHistories = chatController.GetAllThreadHistories();
                if (allHistories.TryGetValue("__main__", out var mainHistory))
                {
                    saveData.ChatHistory = mainHistory;
                }
                else
                {
                    saveData.ChatHistory = chatController.GetChatHistory();
                }
            }

            // サブスレッドを保存
            if (scenarioManager != null)
            {
                var threads = scenarioManager.GetAllDeclaredThreads();
                saveData.Subthreads = new List<SubthreadData>(threads.Values);
            }

            // 矛盾発見データを保存
            ContradictionManager contradictionManager = FindFirstObjectByType<ContradictionManager>();
            if (contradictionManager != null)
            {
                saveData.DiscoveredContradictionIDs = contradictionManager.GetDiscoveredList();
                saveData.HalluciCoin = contradictionManager.HalluciCoin;
            }

            // ダッシュボード進捗を保存（CompletedChannelIDs, ChannelDayProgress）
            // これらは EndDayCommand から CurrentSaveData に直接書き込まれるため、引き継ぎが必要
            if (m_CurrentSaveData != null)
            {
                saveData.CompletedChannelIDs = new List<string>(m_CurrentSaveData.CompletedChannelIDs);
                saveData.ChannelDayProgress = new Dictionary<string, int>(m_CurrentSaveData.ChannelDayProgress);
                saveData.UsedRecipeIDs = new List<string>(m_CurrentSaveData.UsedRecipeIDs);
            }

            return saveData;
        }

        /// <summary>
        /// ScenarioManagerから現在のノード名を取得
        /// </summary>
        private string GetCurrentNodeName(ScenarioManager scenarioManager)
        {
            string currentNodeName = scenarioManager.GetVariable<string>("$current_node");
            if (!string.IsNullOrWhiteSpace(currentNodeName))
            {
                return currentNodeName;
            }

            string currentChannelId = scenarioManager.CurrentChannelID;
            if (!string.IsNullOrWhiteSpace(currentChannelId))
            {
                ChannelData channelData = Resources.Load<ChannelData>($"Channels/{currentChannelId}");
                if (channelData != null && !string.IsNullOrWhiteSpace(channelData.StartNodeName))
                {
                    return channelData.StartNodeName;
                }
            }

            return scenarioManager.DefaultStartNode;
        }

        /// <summary>
        /// VariableStorageから全Yarn変数を取得。
        /// DeductionBoard依存を排除し、動的生成された$has_topic_X等も確実に保存する。
        /// </summary>
        private Dictionary<string, object> GetYarnVariables(ScenarioManager scenarioManager)
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();

            // VariableStorage から全変数を直接取得
            var dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            if (dialogueRunner != null && dialogueRunner.VariableStorage != null)
            {
                var (floats, strings, bools) = dialogueRunner.VariableStorage.GetAllVariables();
                foreach (var kvp in bools)
                {
                    variables[kvp.Key] = kvp.Value;
                }
                foreach (var kvp in floats)
                {
                    variables[kvp.Key] = kvp.Value;
                }
                foreach (var kvp in strings)
                {
                    // $speaker や $current_node は別途保存されるためスキップ
                    if (kvp.Key == "$speaker" || kvp.Key == "$current_node") continue;
                    variables[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                Debug.LogWarning("SaveManager: DialogueRunner or VariableStorage not found. Yarn variables will not be saved.");
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
            if (!IsValidSlot(slotNumber))
            {
                Debug.LogError($"SaveManager: Invalid slot number {slotNumber}. Must be between 0 and {m_MaxSaveSlots - 1}, or {AutoSaveSlot} (auto-save).");
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
                Debug.LogError($"SaveManager: Failed to load game - {e.Message}\n{e.StackTrace}");
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
                if (!m_DeductionBoardWarningShown)
                {
                    Debug.LogWarning("SaveManager: DeductionBoard not found. Topics will not be restored.");
                    m_DeductionBoardWarningShown = true;
                }
            }

            // チャット履歴を復元（シナリオ再開前に実行）
            ChatController chatController = FindFirstObjectByType<ChatController>();
            if (chatController != null && saveData.ChatHistory != null && saveData.ChatHistory.Count > 0)
            {
                chatController.RestoreChatHistory(saveData.ChatHistory);
            }

            // サブスレッドを復元
            ScenarioManager scenarioManagerForThreads = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManagerForThreads != null && saveData.Subthreads != null)
            {
                // スレッドUIをリセット（古いエントリの重複防止）
                var threadSwitcher = FindFirstObjectByType<ThreadSwitcherController>();
                threadSwitcher?.Reset();

                scenarioManagerForThreads.ClearDeclaredThreads();
                foreach (var thread in saveData.Subthreads)
                {
                    scenarioManagerForThreads.RegisterDeclaredThread(thread);
                }
            }

            // ActiveThreadIdを復元
            // SetActiveThreadIdを先に呼ぶとSwitchToThread内のm_ThreadHistories保存キーがずれるため、
            // SwitchToThreadに直接任せる（内部でメイン履歴を__main__に正しく保存してから切替）
            if (chatController != null && !string.IsNullOrEmpty(saveData.ActiveThreadId))
            {
                var thread = scenarioManagerForThreads?.GetDeclaredThread(saveData.ActiveThreadId);
                if (thread != null)
                {
                    chatController.SetActiveThreadType(thread.Type);
                    chatController.SwitchToThread(saveData.ActiveThreadId, thread.ChatHistory);
                }
            }

            // 矛盾発見データを復元
            ContradictionManager contradictionManager = FindFirstObjectByType<ContradictionManager>();
            if (contradictionManager != null)
            {
                contradictionManager.RestoreDiscovered(
                    saveData.DiscoveredContradictionIDs ?? new List<string>(),
                    saveData.HalluciCoin);

                // ヒントポリシーを復元 (CurrentChannelのEnableHints/MaxHintDifficulty)
                if (!string.IsNullOrEmpty(saveData.CurrentChannelID))
                {
                    ChannelData channelForHints = Resources.Load<ChannelData>($"Channels/{saveData.CurrentChannelID}");
                    if (channelForHints != null)
                    {
                        contradictionManager.SetCurrentChannel(channelForHints);
                    }
                }
            }

            ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();

            // $halluci_coin を Yarn 変数に同期 (ロード直後のHCゲート評価に必要)
            if (scenarioManager != null)
            {
                scenarioManager.SetVariable<float>("$halluci_coin", (float)saveData.HalluciCoin);
            }
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

                scenarioManager.ApplyBranchThreadState(saveData.BranchThread);

                // m_CurrentChannel を復元 (EndDay が正しいチャンネルに進捗を記録するために必要)
                if (!string.IsNullOrEmpty(saveData.CurrentChannelID))
                {
                    ChannelData channelData = Resources.Load<ChannelData>($"Channels/{saveData.CurrentChannelID}");
                    if (channelData != null)
                    {
                        scenarioManager.SetCurrentChannel(channelData);
                    }
                    else
                    {
                        Debug.LogWarning($"SaveManager: ChannelData '{saveData.CurrentChannelID}' not found in Resources.");
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
            if (!IsValidSlot(slotNumber))
            {
                Debug.LogError($"SaveManager: Invalid slot number {slotNumber}. Must be between 0 and {m_MaxSaveSlots - 1}, or {AutoSaveSlot} (auto-save).");
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
                Debug.LogError($"SaveManager: Failed to delete save - {e.Message}\n{e.StackTrace}");
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
            if (!IsValidSlot(slotNumber))
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
            if (!IsValidSlot(slotNumber))
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
                Debug.LogError($"SaveManager: Failed to read save info - {e.Message}\n{e.StackTrace}");
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

        #region Public Methods - AutoSave
        /// <summary>
        /// オートセーブを実行する。
        /// 専用スロット (slot=99) を使用し、クールダウン内の連続呼び出しを無視する。
        /// </summary>
        /// <param name="forceSave">trueの場合、クールダウンを無視して即座に保存する（EndDay等の重要イベント用）</param>
        /// <returns>セーブが実行された場合true、クールダウン中でスキップされた場合false</returns>
        public bool AutoSave(bool forceSave = false)
        {
            float now = Time.unscaledTime;
            if (!forceSave && now - m_LastAutoSaveTime < AutoSaveCooldownSeconds)
            {
                Debug.Log($"SaveManager: AutoSave skipped (cooldown). Next available in {AutoSaveCooldownSeconds - (now - m_LastAutoSaveTime):F1}s");
                return false;
            }

            bool success = SaveGame(AutoSaveSlot);
            if (success)
            {
                m_LastAutoSaveTime = now;
                ShowAutoSaveIndicator();
                Debug.Log("SaveManager: AutoSave completed.");
            }
            return success;
        }

        /// <summary>
        /// オートセーブスロットにデータが存在するか
        /// </summary>
        public bool HasAutoSave()
        {
            return HasSaveInSlot(AutoSaveSlot);
        }

        /// <summary>
        /// オートセーブスロットのデータ情報を取得
        /// </summary>
        public SaveData GetAutoSaveInfo()
        {
            return GetSaveInfo(AutoSaveSlot);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// スロット番号が有効かチェックする（通常スロット + オートセーブスロット）
        /// </summary>
        private bool IsValidSlot(int slotNumber)
        {
            if (slotNumber == AutoSaveSlot) return true;
            return slotNumber >= 0 && slotNumber < m_MaxSaveSlots;
        }

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

        #region AutoSave Indicator
        /// <summary>
        /// オートセーブインジケーターを表示する（Canvas直下に軽量表示）
        /// </summary>
        private void ShowAutoSaveIndicator()
        {
            EnsureAutoSaveIndicator();
            if (m_AutoSaveIndicator == null) return;

            if (m_AutoSaveIndicatorCoroutine != null)
            {
                StopCoroutine(m_AutoSaveIndicatorCoroutine);
            }
            m_AutoSaveIndicatorCoroutine = StartCoroutine(AutoSaveIndicatorRoutine());
        }

        /// <summary>
        /// インジケーターをフェードイン→表示→フェードアウトするコルーチン
        /// </summary>
        private IEnumerator AutoSaveIndicatorRoutine()
        {
            if (m_AutoSaveCanvasGroup == null) yield break;

            m_AutoSaveIndicator.SetActive(true);
            m_AutoSaveIndicator.transform.SetAsLastSibling();

            // フェードイン (0.15s)
            float elapsed = 0f;
            while (elapsed < 0.15f)
            {
                elapsed += Time.unscaledDeltaTime;
                m_AutoSaveCanvasGroup.alpha = Mathf.Clamp01(elapsed / 0.15f);
                yield return null;
            }
            m_AutoSaveCanvasGroup.alpha = 1f;

            // 表示維持
            yield return new WaitForSecondsRealtime(AutoSaveIndicatorDuration);

            // フェードアウト (0.3s)
            elapsed = 0f;
            while (elapsed < 0.3f)
            {
                elapsed += Time.unscaledDeltaTime;
                m_AutoSaveCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / 0.3f);
                yield return null;
            }
            m_AutoSaveCanvasGroup.alpha = 0f;
            m_AutoSaveIndicator.SetActive(false);
            m_AutoSaveIndicatorCoroutine = null;
        }

        /// <summary>
        /// オートセーブインジケーターUIを動的生成する（まだ存在しない場合のみ）
        /// </summary>
        private void EnsureAutoSaveIndicator()
        {
            if (m_AutoSaveIndicator != null) return;

            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("SaveManager: AutoSave indicator skipped — no Canvas found in scene.");
                return;
            }

            // ルートオブジェクト
            GameObject indicator = new GameObject("AutoSaveIndicator", typeof(RectTransform), typeof(CanvasGroup));
            indicator.transform.SetParent(canvas.transform, false);

            RectTransform rt = indicator.GetComponent<RectTransform>();
            // 右下に配置
            rt.anchorMin = new Vector2(1f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-16f, 16f);
            rt.sizeDelta = new Vector2(160f, 32f);

            m_AutoSaveCanvasGroup = indicator.GetComponent<CanvasGroup>();
            m_AutoSaveCanvasGroup.alpha = 0f;
            m_AutoSaveCanvasGroup.blocksRaycasts = false;
            m_AutoSaveCanvasGroup.interactable = false;

            // 背景
            GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(indicator.transform, false);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            Image bgImage = bg.GetComponent<Image>();
            bgImage.color = new Color(0f, 0f, 0f, 0.5f);
            bgImage.raycastTarget = false;

            // テキスト
            GameObject textObj = new GameObject("Label", typeof(RectTransform));
            textObj.transform.SetParent(indicator.transform, false);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 2f);
            textRect.offsetMax = new Vector2(-8f, -2f);

            TextMeshProUGUI label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = "Auto Saving...";
            label.fontSize = 14f;
            label.color = new Color(1f, 1f, 1f, 0.9f);
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;

            // 最前面に配置（他のUI要素の下に隠れないようにする）
            indicator.transform.SetAsLastSibling();
            indicator.SetActive(false);
            m_AutoSaveIndicator = indicator;
        }
        #endregion
    }
}


