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

        public T GetVariable<T>(string variableName)
        {
            if (m_DialogueRunner == null || m_DialogueRunner.VariableStorage == null)
            {
                return default;
            }

            string normalizedName = NormalizeVariableName(variableName);

            if (typeof(T) == typeof(bool))
            {
                if (m_DialogueRunner.VariableStorage.TryGetValue(normalizedName, out bool boolValue))
                {
                    return (T)(object)boolValue;
                }
                return default;
            }

            if (typeof(T) == typeof(string))
            {
                if (m_DialogueRunner.VariableStorage.TryGetValue(normalizedName, out string stringValue))
                {
                    return (T)(object)stringValue;
                }
                return default;
            }

            if (typeof(T) == typeof(float))
            {
                if (m_DialogueRunner.VariableStorage.TryGetValue(normalizedName, out float floatValue))
                {
                    return (T)(object)floatValue;
                }
                return default;
            }

            if (typeof(T) == typeof(double))
            {
                if (m_DialogueRunner.VariableStorage.TryGetValue(normalizedName, out float floatValue))
                {
                    return (T)(object)(double)floatValue;
                }
                return default;
            }

            if (typeof(T) == typeof(int))
            {
                if (m_DialogueRunner.VariableStorage.TryGetValue(normalizedName, out float floatValue))
                {
                    return (T)(object)Mathf.RoundToInt(floatValue);
                }
                return default;
            }

            throw new System.NotSupportedException($"ScenarioManager.GetVariable does not support type {typeof(T).FullName}");
        }

        public void SetVariable<T>(string variableName, T value)
        {
            if (m_DialogueRunner == null || m_DialogueRunner.VariableStorage == null)
            {
                Debug.LogWarning("ScenarioManager: VariableStorage is not available.");
                return;
            }

            string normalizedName = NormalizeVariableName(variableName);

            if (value is bool boolValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(normalizedName, boolValue);
                return;
            }

            if (value is string stringValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(normalizedName, stringValue);
                return;
            }

            if (value is float floatValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(normalizedName, floatValue);
                return;
            }

            if (value is double doubleValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(normalizedName, (float)doubleValue);
                return;
            }

            if (value is int intValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(normalizedName, (float)intValue);
                return;
            }

            throw new System.NotSupportedException($"ScenarioManager.SetVariable does not support type {typeof(T).FullName}");
        }
        #endregion

        #region Private Methods
        private static string NormalizeVariableName(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                return variableName;
            }

            if (variableName[0] == '$')
            {
                return variableName;
            }

            return "$" + variableName;
        }

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
