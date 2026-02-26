using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using Assets.Scripts.Utils;
using UnityEngine.SceneManagement;

namespace ProjectFoundPhone.Tests
{
    public class PerformanceBaselineVerification : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoInitialize()
        {
             if (SceneManager.GetActiveScene().name != "DebugChatScene") return;
             
             GameObject go = new GameObject("PerformanceBaselineVerification");
             go.AddComponent<PerformanceBaselineVerification>();
             DontDestroyOnLoad(go);
        }

        private IEnumerator Start()
        {
            DateTime verificationStart = DateTime.UtcNow;
            yield return new WaitForSeconds(1.0f); // Wait for Monitor to spawn
            var monitor = FindFirstObjectByType<PerformanceMonitor>();
            if (monitor == null)
            {
                 Debug.LogError("[Verification] PerformanceMonitor NOT found.");
                 yield break;
            }

            // Calculate wait time
            float waitTime = monitor.InitialDelay + monitor.MeasurementDuration + 2.0f;
            Debug.Log($"[Verification] Waiting {waitTime}s for PerformanceMonitor...");
            yield return new WaitForSeconds(waitTime);

            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string reportPath = Path.Combine(projectRoot, monitor.ReportPath);
            string reportDirectory = Path.GetDirectoryName(reportPath);
            string reportFileNameNoExt = Path.GetFileNameWithoutExtension(reportPath);
            string reportExt = Path.GetExtension(reportPath);
            string reportPattern = $"{reportFileNameNoExt}*{reportExt}";

            string latestReport = null;
            if (Directory.Exists(reportDirectory))
            {
                latestReport = Directory.GetFiles(reportDirectory, reportPattern)
                    .Select(path => new FileInfo(path))
                    .Where(info => info.LastWriteTimeUtc >= verificationStart.AddSeconds(-2))
                    .OrderByDescending(info => info.LastWriteTimeUtc)
                    .Select(info => info.FullName)
                    .FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(latestReport) && File.Exists(latestReport))
            {
                Debug.Log($"[Verification] SUCCESS: Report found at {latestReport}");
                // Read content to ensure it's not empty
                string content = File.ReadAllText(latestReport);
                if (content.Length > 0 && content.Contains("FPS"))
                {
                     Debug.Log("[Verification] Content validated (Contains FPS).");
                }
                else
                {
                     Debug.LogError("[Verification] FAILURE: Report is empty or missing data.");
                }
            }
            else
            {
                Debug.LogError($"[Verification] FAILURE: New report NOT found (pattern: {reportPattern}) in {reportDirectory}");
            }
        }
    }
}
