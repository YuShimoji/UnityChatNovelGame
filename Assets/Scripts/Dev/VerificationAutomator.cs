using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Dev
{
    public class VerificationAutomator : MonoBehaviour
    {
        public string TargetScene = "DebugChatScene";

        private IEnumerator Start()
        {
            Debug.Log("VerificationAutomator: Started.");
            DontDestroyOnLoad(gameObject);

            Debug.Log($"VerificationAutomator: Loading {TargetScene}...");
            yield return SceneManager.LoadSceneAsync(TargetScene);

            Debug.Log("VerificationAutomator: Scene loaded. Waiting for logic...");
            
            // Debug file to prove we ran
            string markerPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Docs/evidence/automator_ran.txt");
            File.WriteAllText(markerPath, "Automator ran at " + System.DateTime.Now);

            yield return new WaitForSeconds(3.0f);

            var capture = FindObjectOfType<VerificationCapture>();
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
    }
}
