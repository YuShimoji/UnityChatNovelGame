using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.Effects;
using DG.Tweening;

namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// シナリオの進行を管理するクラス
    /// DialogueRunnerと連携してメッセージ表示やトピック解放などを制御する
    /// </summary>
    public class ScenarioManager : MonoBehaviour
    {
        #region Private Fields
        [Header("References")]
        [SerializeField] private DialogueRunner m_DialogueRunner;
        [SerializeField] private ChatController m_ChatController;

        [Header("Debug")]
        [SerializeField] private string m_DebugScenarioID;
        #endregion

        #region Public Properties
        public bool InputLocked { get; set; }
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            RegisterCustomCommands();

            if (!string.IsNullOrEmpty(m_DebugScenarioID))
            {
                PlayScenario(m_DebugScenarioID);
            }
        }

        private void OnDestroy()
        {
            UnregisterCustomCommands();
        }
        #endregion

        #region Public Methods
        public void PlayScenario(string nodeName)
        {
            if (m_DialogueRunner != null)
            {
                if (m_DialogueRunner.NodeExists(nodeName))
                {
                    m_DialogueRunner.StartDialogue(nodeName);
                }
                else
                {
                    Debug.LogWarning($"ScenarioManager: Node '{nodeName}' not found.");
                }
            }
        }

        /// <summary>
        /// Yarn変数を取得する
        /// </summary>
        /// <typeparam name="T">変数の型</typeparam>
        /// <param name="variableName">変数名</param>
        /// <returns>変数の値、取得できない場合はdefault(T)</returns>
        public T GetVariable<T>(string variableName)
        {
            if (m_DialogueRunner == null || m_DialogueRunner.VariableStorage == null)
            {
                Debug.LogWarning($"ScenarioManager: Cannot get variable {variableName}. DialogueRunner or VariableStorage is not initialized.");
                return default(T);
            }

            if (m_DialogueRunner.VariableStorage.TryGetValue<T>(variableName, out var value))
            {
                return value;
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: Variable {variableName} not found in VariableStorage.");
            }

            return default(T);
        }

        /// <summary>
        /// Yarn変数を設定する
        /// </summary>
        /// <typeparam name="T">変数の型（string, float, bool）</typeparam>
        /// <param name="variableName">変数名</param>
        /// <param name="value">設定する値</param>
        public void SetVariable<T>(string variableName, T value)
        {
            if (m_DialogueRunner == null || m_DialogueRunner.VariableStorage == null)
            {
                Debug.LogWarning($"ScenarioManager: Cannot set variable {variableName}. DialogueRunner or VariableStorage is not initialized.");
                return;
            }

            // Yarn SpinnerのVariableStorageは型別のSetValueオーバーロードを持つ
            switch (value)
            {
                case string stringValue:
                    m_DialogueRunner.VariableStorage.SetValue(variableName, stringValue);
                    break;
                case float floatValue:
                    m_DialogueRunner.VariableStorage.SetValue(variableName, floatValue);
                    break;
                case bool boolValue:
                    m_DialogueRunner.VariableStorage.SetValue(variableName, boolValue);
                    break;
                case int intValue:
                    m_DialogueRunner.VariableStorage.SetValue(variableName, (float)intValue);
                    break;
                default:
                    Debug.LogWarning($"ScenarioManager: Unsupported variable type {typeof(T)} for {variableName}");
                    return;
            }
            Debug.Log($"ScenarioManager: Set variable {variableName} = {value}");
        }
        #endregion

        #region Private Methods
        private void RegisterCustomCommands()
        {
            if (m_DialogueRunner == null) return;

            // Example command registration (reconstructed from context)
            // m_DialogueRunner.AddCommandHandler("custom_command", MethodName);
        }

        private void UnregisterCustomCommands()
        {
            // Clean up commands
        }
        #endregion
    }
}
