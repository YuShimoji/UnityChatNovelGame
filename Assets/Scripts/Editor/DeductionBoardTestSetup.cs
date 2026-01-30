using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Tests;
using Assets.Scripts.Utils;

// Avoid namespace conflict with ProjectFoundPhone.Debug
using Debug = UnityEngine.Debug;

namespace ProjectFoundPhone.Editor
{
    public class DeductionBoardTestSetup
    {
        [MenuItem("Project FoundPhone/Tests/Setup Deduction Board Verification")]
        public static void SetupVerificationScene()
        {
            Debug.Log("Setting up Deduction Board Verification Scene...");

            // 1. Ensure DeductionBoard Exists
            var deductionBoard = Object.FindFirstObjectByType<DeductionBoard>();
            if (deductionBoard == null)
            {
                Debug.Log("DeductionBoard not found. Instantiating from Prefab...");
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/DeductionBoard.prefab");
                if (prefab != null)
                {
                    var instance = Object.Instantiate(prefab);
                    instance.name = "DeductionBoard";
                    deductionBoard = instance.GetComponent<DeductionBoard>();
                }
                else
                {
                    Debug.LogError("Could not load DeductionBoard prefab at Assets/Prefabs/UI/DeductionBoard.prefab");
                }
            }
            else
            {
                Debug.Log("DeductionBoard found in scene.");
            }

            // 2. Ensure Verification Tools Exist
            var testRunner = GameObject.Find("VerificationRunner");
            if (testRunner == null)
            {
                testRunner = new GameObject("VerificationRunner");
                Debug.Log("Created VerificationRunner object.");
            }

            // Add VerificationCapture
            var capture = testRunner.GetComponent<VerificationCapture>();
            if (capture == null)
            {
                capture = testRunner.AddComponent<VerificationCapture>();
                capture.CaptureOnStart = false; // We want manual trigger from the test script
                capture.CaptureLogs = true;
                Debug.Log("Added VerificationCapture component.");
            }

            // Add DeductionBoardVerification
            var verifier = testRunner.GetComponent<DeductionBoardVerification>();
            if (verifier == null)
            {
                verifier = testRunner.AddComponent<DeductionBoardVerification>();
                Debug.Log("Added DeductionBoardVerification component.");
            }
            
            // Let's use Reflection for robust setup.
            var type = typeof(DeductionBoardVerification);
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            
            type.GetField("m_VerificationCapture", flags)?.SetValue(verifier, capture);
            type.GetField("m_DelayBeforeCapture", flags)?.SetValue(verifier, 15.0f);
            type.GetField("m_TargetTopicID", flags)?.SetValue(verifier, "debug_topic_01");

            Debug.Log("Linked components and configured settings.");

            Debug.Log("Setup Complete! Please ensure you are in 'DebugChatScene' or 'VerificationScene'.");
        }
    }
}
