using UnityEngine;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Core;

namespace ProjectFoundPhone.Dev
{
    /// <summary>
    /// Verification script for TASK_013.
    /// Checks if "debug_topic_01" is unlocked and logs a success message.
    /// Attach this to an object in DebugChatScene (e.g., ScenarioManager) for verification.
    /// </summary>
    public class TopicUnlockVerifier : MonoBehaviour
    {
        private bool m_Verified = false;
        private string m_TargetTopicID = "debug_topic_01";

        private void Update()
        {
            if (m_Verified) return;

            // Check if DeductionBoard has the topic
            if (DeductionBoard.Instance != null && DeductionBoard.Instance.HasTopic(m_TargetTopicID))
            {
                Debug.Log($"<color=green>VERIFICATION PASSED: Topic '{m_TargetTopicID}' is unlocked in DeductionBoard.</color>");
                m_Verified = true;
            }
            
            // Optional: Check Yarn variable if DeductionBoard is not enough or as a secondary check
            // (Assuming ScenarioManager is accessible via singleton or FindObject, here we just check DeductionBoard as it's the end result)
        }
    }
}
