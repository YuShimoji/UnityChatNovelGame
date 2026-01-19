using UnityEngine;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Dev
{
    public class ChatScenarioTester : MonoBehaviour
    {
        public ScenarioManager scenarioManager;
        public ChatScenarioData scenarioData;

        public void PlayScenario()
        {
            if (scenarioManager != null && scenarioData != null)
            {
                scenarioManager.PlayScenario(scenarioData);
            }
            else
            {
                Debug.LogError("Assign ScenarioManager and ScenarioData!");
            }
        }
        
        // ContextMenu to run from Editor
        [ContextMenu("Play Scenario")]
        public void PlayScenarioContextMenu()
        {
            PlayScenario();
        }
    }
}
