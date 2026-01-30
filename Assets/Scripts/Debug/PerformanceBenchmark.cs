using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine.Profiling;

namespace ProjectFoundPhone.DebugUtils
{
    /// <summary>
    /// Simple utility to measure performance metrics and generate a baseline report.
    /// This script automatically runs and captures data for a specified duration.
    /// </summary>
    public class PerformanceBenchmark : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float duration = 15f;
        
        private float startTime;
        private int frameCount = 0;
        private float totalFps = 0;
        private float minFps = float.MaxValue;
        private float maxFps = 0;
        
        private static Stopwatch bootTimer;
        private static long bootTimeMs = -1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            bootTimer = Stopwatch.StartNew();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            if (FindObjectOfType<PerformanceBenchmark>() != null) return;
            
            GameObject go = new GameObject("PerformanceBenchmark");
            go.AddComponent<PerformanceBenchmark>();
            DontDestroyOnLoad(go);
            
            UnityEngine.Debug.Log("[Perf] PerformanceBenchmark auto-spawned.");
        }

        void Start()
        {
            if (bootTimeMs < 0 && bootTimer != null)
            {
                bootTimer.Stop();
                bootTimeMs = bootTimer.ElapsedMilliseconds;
            }
            
            startTime = Time.time;
            UnityEngine.Debug.Log($"[Perf] Performance Baseline Measurement Started. Duration: {duration}s");
            UnityEngine.Debug.Log($"[Perf] Initial Boot Time: {bootTimeMs} ms");
        }

        void Update()
        {
            float fps = 1.0f / Time.unscaledDeltaTime;
            totalFps += fps;
            if (fps < minFps) minFps = fps;
            if (fps > maxFps) maxFps = fps;
            frameCount++;

            // End measurement after duration
            if (Time.time - startTime >= duration)
            {
                GenerateReport();
                enabled = false;
                UnityEngine.Debug.Log("[Perf] Measurement complete. Report generated.");
            }
        }

        private void GenerateReport()
        {
            float avgFps = totalFps / frameCount;
            string reportDir = Path.Combine(Application.dataPath, "../docs/reports");
            if (!Directory.Exists(reportDir)) Directory.CreateDirectory(reportDir);
            
            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmm");
            string filename = $"PERFORMANCE_BASELINE_{timestamp}.md";
            string reportPath = Path.Combine(reportDir, filename);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# Performance Baseline Report");
            sb.AppendLine($"**Date**: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"**Scene**: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            sb.AppendLine($"**Platform**: {Application.platform}");
            sb.AppendLine("");
            sb.AppendLine("## Core Metrics");
            sb.AppendLine("| Metric | Value |");
            sb.AppendLine("| --- | --- |");
            sb.AppendLine($"| Boot Time (BeforeSceneLoad ~ Start) | {bootTimeMs} ms |");
            sb.AppendLine($"| Average FPS | {avgFps:F2} |");
            sb.AppendLine($"| Min FPS | {minFps:F2} |");
            sb.AppendLine($"| Max FPS | {maxFps:F2} |");
            sb.AppendLine($"| Sample Duration | {duration} s |");
            sb.AppendLine("");
            sb.AppendLine("## Memory Metrics");
            sb.AppendLine("| Metric | Value |");
            sb.AppendLine("| --- | --- |");
            sb.AppendLine($"| Total Allocated Memory | {Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024)} MB |");
            sb.AppendLine($"| Total Reserved Memory | {Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)} MB |");
            sb.AppendLine($"| GC Used Memory | {System.GC.GetTotalMemory(false) / (1024 * 1024)} MB |");

            File.WriteAllText(reportPath, sb.ToString());
            
            // Generate a constant name file for current baseline summary
            string latestPath = Path.Combine(reportDir, "PERFORMANCE_BASELINE_LATEST.md");
            File.WriteAllText(latestPath, sb.ToString());
            
            UnityEngine.Debug.Log($"[Perf] Report generated at: {reportPath}");
        }
    }
}
