using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Tests;
using Assets.Scripts.Utils;

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
            
            // Link References (using SerializedObject to avoid dirtying scene weirdly, though direct assign in editor is fine)
            // We can't access private fields easily without reflection or making them public. 
            // Since we made them [SerializeField], they are accessible to SerializedObject.
            // But for simplicity in this script, let's just assume the user might need to assign if we can't.
            // Actually, we can just use SerializedObject.
            
            /*
            SerializedObject so = new SerializedObject(verifier);
            so.FindProperty("m_VerificationCapture").objectReferenceValue = capture;
            so.FindProperty("m_DelayBeforeCapture").floatValue = 15.0f;
            so.FindProperty("m_TargetTopicID").stringValue = "debug_topic_01";
            so.ApplyModifiedProperties();
            */
            
            // However, since we defined the fields in the file we just wrote, we know they are private [SerializeField].
            // To be safe and quick, let's just warn the user to check settings if we can't set them easily, 
            // OR use reflection.
            
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
