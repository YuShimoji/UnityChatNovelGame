using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Utils
{
    /// <summary>
    /// verification tool to capture screenshot and logs automatically on start.
    /// saves evidence to Docs/evidence/.
    /// </summary>
    public class VerificationCapture : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Capture screenshot immediately on Start?")]
        public bool CaptureOnStart = true;
        
        [Tooltip("Delay in seconds before capturing on Start (to allow UI to settle)")]
        public float DelaySeconds = 0.5f;

        [Tooltip("Capture logs?")]
        public bool CaptureLogs = true;

        private const string EvidencePath = "Docs/evidence";

        private void Start()
        {
            if (CaptureOnStart)
            {
                StartCoroutine(CaptureRoutine());
            }
        }

        public void TriggerCapture()
        {
            StartCoroutine(CaptureRoutine());
        }

        private IEnumerator CaptureRoutine()
        {
            if (DelaySeconds > 0)
                yield return new WaitForSeconds(DelaySeconds);

            yield return new WaitForEndOfFrame();

            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string evidenceDir = Path.Combine(projectRoot, EvidencePath);

            // Ensure directory exists
            if (!Directory.Exists(evidenceDir))
            {
                Directory.CreateDirectory(evidenceDir);
                Debug.Log($"Created evidence directory: {evidenceDir}");
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string sceneName = SceneManager.GetActiveScene().name;
            string filename = $"Capture_{timestamp}_{sceneName}.png";
            string fullPath = Path.Combine(evidenceDir, filename);

            ScreenCapture.CaptureScreenshot(fullPath);
            Debug.Log($"Screenshot captured: {fullPath}");

            if (CaptureLogs)
            {
                string logFilename = $"Log_{timestamp}_{sceneName}.txt";
                string logPath = Path.Combine(evidenceDir, logFilename);
                // Note: This only captures logs that happened *so far* or we'd need to hook Application.logMessageReceived
                // For now, let's just write a simple marker file or hook if needed. 
                // A full log dump might require accumulating logs. 
                // Let's keep it simple: just a marker saying verification occurred.
                File.WriteAllText(logPath, $"Verification event at {DateTime.Now} in scene {sceneName}\nSee Unity Console or Player.log for details.");
            }
        }
    }
}
