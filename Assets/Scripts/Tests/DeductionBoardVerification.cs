using UnityEngine;
using System.Collections;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.UI;
using Assets.Scripts.Utils;

namespace ProjectFoundPhone.Tests
{
    /// <summary>
    /// Automated verification script for Deduction Board (Task 018).
    /// Executes a Yarn scenario that triggers UnlockTopic, then captures evidence.
    /// </summary>
    public class DeductionBoardVerification : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float m_DelayBeforeCapture = 15.0f; // Sufficient time for DebugScript to run
        [SerializeField] private string m_TargetTopicID = "debug_topic_01";
        
        [Header("References")]
        [SerializeField] private VerificationCapture m_VerificationCapture;

        private IEnumerator Start()
        {
            Debug.Log("DeductionBoardVerification: Starting automated test...");

            // 1. Ensure Dependencies
            var scenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager == null)
            {
                Debug.LogError("DeductionBoardVerification: ScenarioManager not found!");
                yield break;
            }

            if (DeductionBoard.Instance == null)
            {
                Debug.LogError("DeductionBoardVerification: DeductionBoard not found!");
                yield break;
            }

            if (m_VerificationCapture == null)
            {
                m_VerificationCapture = FindFirstObjectByType<VerificationCapture>();
                if (m_VerificationCapture == null)
                {
                    Debug.LogWarning("DeductionBoardVerification: VerificationCapture not found. Evidence might not be saved properly.");
                }
            }

            // 2. Start Scenario
            // We assume DebugScript.yarn is loaded and "Start" is the node.
            Debug.Log("DeductionBoardVerification: Starting Scenario 'Start'...");
            scenarioManager.StartScenario("Start");

            // 3. Wait for Scenario to process (unlock topic)
            Debug.Log($"DeductionBoardVerification: Waiting {m_DelayBeforeCapture} seconds for scenario execution...");
            yield return new WaitForSeconds(m_DelayBeforeCapture);

            // 4. Verify Logic
            bool hasTopic = DeductionBoard.Instance.HasTopic(m_TargetTopicID);
            if (hasTopic)
            {
                Debug.Log($"DeductionBoardVerification: SUCCESS - Topic '{m_TargetTopicID}' was found on the board.");
            }
            else
            {
                Debug.LogError($"DeductionBoardVerification: FAILURE - Topic '{m_TargetTopicID}' was NOT found on the board.");
            }

            // 5. Capture Evidence
            if (m_VerificationCapture != null)
            {
                Debug.Log("DeductionBoardVerification: Triggering Evidence Capture...");
                m_VerificationCapture.TriggerCapture();
            }
            else
            {
                Debug.LogError("DeductionBoardVerification: Cannot capture evidence (Tool missing).");
            }
        }
    }
}
