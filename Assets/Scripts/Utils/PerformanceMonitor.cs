using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Utils
{
    /// <summary>
    /// Monitors performance metrics (FPS, Memory, GC) and logs them to a report.
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Duration of the measurement in seconds.")]
        public float MeasurementDuration = 10f;
        
        [Tooltip("Interval between samples in seconds.")]
        public float SampleInterval = 1f;

        [Tooltip("Wait for this many seconds before starting (to skip loading spikes).")]
        public float InitialDelay = 2f;

        [Header("Output")]
        public string ReportPath = "Docs/reports/REPORT_TASK_022_PerformanceBaseline.md";

        private float _startTime;
        private bool _isMeasuring = false;
        private bool _hasFinished = false;
        private StringBuilder _logBuffer = new StringBuilder();

        private int _frameCount = 0;
        private float _timeAccumulator = 0;

        private static bool _autoInitialized = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnRuntimeMethodLoad()
        {
            if (_autoInitialized) return;
            
            string currentScene = SceneManager.GetActiveScene().name;
            Debug.Log($"[PerformanceMonitor] Auto-init check. Current Scene: {currentScene}");

            if (currentScene != "DebugChatScene") 
            {
                Debug.Log("[PerformanceMonitor] Skipping auto-init: Scene is not DebugChatScene.");
                return;
            }

            GameObject go = new GameObject("PerformanceMonitor (Auto)");
            go.AddComponent<PerformanceMonitor>();
            DontDestroyOnLoad(go);
            _autoInitialized = true;
            Debug.Log("[PerformanceMonitor] Auto-initialized in DebugChatScene.");
        }

        private void Start()
        {
            StartCoroutine(MeasurementRoutine());
        }

        private void OnApplicationQuit()
        {
            if (_isMeasuring && !_hasFinished)
            {
                Debug.Log("[PerformanceMonitor] App quitting during measurement. Saving partial report...");
                _logBuffer.AppendLine("\n**Note: App quit before full duration.**");
                SaveReport();
            }
        }

        private IEnumerator MeasurementRoutine()
        {
            Debug.Log("[PerformanceMonitor] Waiting for initial delay (2s)...");
            yield return new WaitForSeconds(InitialDelay);

            Debug.Log("[PerformanceMonitor] Starting measurement (10s)...");
            _startTime = Time.time;
            _isMeasuring = true;
            _logBuffer.AppendLine("# Performance Baseline Report");
            _logBuffer.AppendLine($"- **Date**: {DateTime.Now}");
            _logBuffer.AppendLine($"- **Scene**: {SceneManager.GetActiveScene().name}");
            _logBuffer.AppendLine($"- **Duration**: {MeasurementDuration}s (Requested)");
            _logBuffer.AppendLine($"- **Platform**: {Application.platform}");
            _logBuffer.AppendLine();
            _logBuffer.AppendLine("| Time (s) | FPS | Reserved (MB) | GC Alloc (KB) |");
            _logBuffer.AppendLine("|----------|-----|---------------|---------------|");

            float nextSampleTime = Time.time + SampleInterval;
            while (Time.time < _startTime + MeasurementDuration)
            {
                _frameCount++;
                _timeAccumulator += Time.unscaledDeltaTime;

                if (Time.time >= nextSampleTime)
                {
                    float fps = _frameCount / _timeAccumulator;
                    long reservedMemory = Profiler.GetTotalReservedMemoryLong() / (1024 * 1024);
                    
                    _logBuffer.AppendLine($"| {Time.time - _startTime:F1} | {fps:F1} | {reservedMemory} | - |");
                    
                    _frameCount = 0;
                    _timeAccumulator = 0;
                    nextSampleTime = Time.time + SampleInterval;
                }
                yield return null;
            }

            _isMeasuring = false;
            _hasFinished = true;
            Debug.Log("[PerformanceMonitor] Measurement finished.");
            SaveReport();
        }

        private void SaveReport()
        {
            try 
            {
                string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                string fullPath = Path.Combine(projectRoot, ReportPath);
                string dir = Path.GetDirectoryName(fullPath);

                Debug.Log($"[PerformanceMonitor] Attempting to save report to: {fullPath}");

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    Debug.Log($"[PerformanceMonitor] Created directory: {dir}");
                }

                File.WriteAllText(fullPath, _logBuffer.ToString());
                Debug.Log($"[PerformanceMonitor] Report successfully saved.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[PerformanceMonitor] Failed to save report: {e.Message}");
            }
        }
    }
}
