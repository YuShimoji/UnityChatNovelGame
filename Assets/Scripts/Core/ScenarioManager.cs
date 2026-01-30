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
