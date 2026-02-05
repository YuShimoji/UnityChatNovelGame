using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Utils;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Data; // For TopicData
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Dev
{
    public class VerificationAutomator : MonoBehaviour
    {
        public string TargetScene = "DebugChatScene";

        private void Awake()
        {
            // Parse command line arguments
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-targetScene" && i + 1 < args.Length)
                {
                    TargetScene = args[i + 1];
                    Debug.Log($"VerificationAutomator: TargetScene overridden to '{TargetScene}' via CLI.");
                }
            }
        }

        private IEnumerator Start()
        {
            Debug.Log("VerificationAutomator: Started.");
            DontDestroyOnLoad(gameObject);

            Debug.Log($"VerificationAutomator: Loading {TargetScene}...");
            yield return SceneManager.LoadSceneAsync(TargetScene);

            Debug.Log("VerificationAutomator: Scene loaded. Setup logic...");
            
            // Special setup for Synthesis Verification
            if (TargetScene == "VerificationScene" || TargetScene.Contains("Synthesis"))
            {
                yield return StartCoroutine(SetupSynthesisBoard());
            }

            // Debug file to prove we ran
            string markerPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "docs/evidence/automator_ran.txt");
            if (!Directory.Exists(Path.GetDirectoryName(markerPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(markerPath));
            File.WriteAllText(markerPath, "Automator ran at " + System.DateTime.Now);

            yield return new WaitForSeconds(3.0f); // Wait for animations/setup

            var capture = FindFirstObjectByType<VerificationCapture>();
            if (capture == null)
            {
                Debug.LogWarning("VerificationCapture not found! Creating fallback...");
                var go = new GameObject("RuntimeVerificationCapture");
                capture = go.AddComponent<VerificationCapture>();
                capture.CaptureOnStart = false;
                capture.CaptureLogs = true;
            }

            Debug.Log("VerificationAutomator: Triggering Capture...");
            capture.TriggerCapture();

            yield return new WaitForSeconds(1.0f);

            Debug.Log("VerificationAutomator: Done. Exiting.");

#if UNITY_EDITOR
            if (Application.isBatchMode)
            {
                Debug.Log("VerificationAutomator: Batchmode detected. Exiting Editor.");
                EditorApplication.Exit(0);
            }
            else
            {
                EditorApplication.isPlaying = false;
            }
#else
            Application.Quit();
#endif
        }

        private IEnumerator SetupSynthesisBoard()
        {
            Debug.Log("VerificationAutomator: Setting up Synthesis Board...");
            yield return new WaitForEndOfFrame(); // Wait for Awake/Start of Board

            var board = FindFirstObjectByType<DeductionBoard>();
            if (board == null)
            {
                Debug.LogError("VerificationAutomator: DeductionBoard not found in VerificationScene!");
                yield break;
            }

            // Load some topics to verify UI
            var topic1 = Resources.Load<TopicData>("Topics/T_FoundPhone");
            var topic2 = Resources.Load<TopicData>("Topics/T_StrangeSignal");

            if (topic1 != null) board.AddTopic(topic1);
            else Debug.LogError("Failed to load topic: T_FoundPhone");

            yield return new WaitForSeconds(0.5f);

            if (topic2 != null) board.AddTopic(topic2);
            else Debug.LogError("Failed to load topic: T_StrangeSignal");
            
            Debug.Log("VerificationAutomator: Topics added.");
        }
    }
}
