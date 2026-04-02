using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectFoundPhone.Tests
{
    /// <summary>
    /// PlayMode テスト共通ヘルパー。
    /// シーンロード、条件待ち、エビデンスキャプチャ、クリーンアップを集約。
    /// </summary>
    public static class PlayModeTestHelpers
    {
        public const string EvidenceRelativePath = "docs/verification";
        public const float SceneLoadTimeoutSeconds = 10f;
        public const float ChatMessageTimeoutSeconds = 5f;

        public static IEnumerator LoadSceneWithTimeout(string sceneName, float timeoutSeconds)
        {
            SceneManager.LoadScene(sceneName);
            yield return null;

            float startTime = Time.realtimeSinceStartup;
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                if (Time.realtimeSinceStartup - startTime > timeoutSeconds)
                {
                    Assert.Fail($"Timeout loading scene '{sceneName}' after {timeoutSeconds} seconds.");
                }
                yield return null;
            }
        }

        public static IEnumerator WaitForScene(string sceneName, float timeoutSeconds)
        {
            float startTime = Time.realtimeSinceStartup;
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                if (Time.realtimeSinceStartup - startTime > timeoutSeconds)
                {
                    Assert.Fail($"Timeout waiting for scene '{sceneName}' after {timeoutSeconds} seconds.");
                }
                yield return null;
            }
        }

        public static IEnumerator WaitForChatMessages(ChatController chatController, float timeoutSeconds, string failureMessage)
        {
            yield return WaitForCondition(
                () =>
                {
                    ScrollRect scrollRect = chatController != null ? chatController.GetComponent<ScrollRect>() : null;
                    return scrollRect != null && scrollRect.content != null && scrollRect.content.childCount > 0;
                },
                timeoutSeconds,
                failureMessage);
        }

        public static IEnumerator WaitForCondition(Func<bool> predicate, float timeoutSeconds, string failureMessage)
        {
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < timeoutSeconds)
            {
                if (predicate())
                {
                    yield break;
                }
                yield return null;
            }
            Assert.Fail(failureMessage);
        }

        public static IEnumerator WaitForBubbleCount(ChatController chatController, int minCount, float timeoutSeconds)
        {
            yield return WaitForCondition(
                () =>
                {
                    ScrollRect scrollRect = chatController != null ? chatController.GetComponent<ScrollRect>() : null;
                    return scrollRect != null && scrollRect.content != null && scrollRect.content.childCount >= minCount;
                },
                timeoutSeconds,
                $"Expected at least {minCount} chat bubbles within {timeoutSeconds}s.");
        }

        public static void CaptureEvidence(string label)
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string evidenceDir = Path.Combine(projectRoot, EvidenceRelativePath);
            Directory.CreateDirectory(evidenceDir);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string sceneName = SceneManager.GetActiveScene().name;
            string baseName = $"{label}_{timestamp}_{sceneName}";

            string screenshotPath = Path.Combine(evidenceDir, $"{baseName}.png");
            if (!Application.isBatchMode)
            {
                ScreenCapture.CaptureScreenshot(screenshotPath);
            }
            else
            {
                Debug.LogWarning("Skipping ScreenCapture.CaptureScreenshot in batch mode to prevent hangs.");
            }

            string logPath = Path.Combine(evidenceDir, $"{baseName}.txt");
            string message = $"Test Failed: {TestContext.CurrentContext.Test.Name}\n" +
                             $"Message: {TestContext.CurrentContext.Result.Message}\n" +
                             $"Scene: {sceneName}\n" +
                             $"Time: {DateTime.Now:O}\n";
            File.WriteAllText(logPath, message);

            Debug.Log($"PlayModeTest: Evidence saved to {evidenceDir}");
        }

        public static void CleanupSaveSlot(int slotNumber)
        {
            string filePath = Path.Combine(Application.persistentDataPath, $"SaveData_{slotNumber}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// シーン破棄前にダイアログを停止する共通 teardown ヘルパー。
        /// </summary>
        public static IEnumerator SafeTeardown(string evidenceLabel, int saveSlot = 0)
        {
            ScenarioManager scenarioManager = UnityEngine.Object.FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null)
            {
                scenarioManager.StopScenario();
            }
            yield return null;

            TestStatus status = TestContext.CurrentContext.Result.Outcome.Status;
            if (status == TestStatus.Failed)
            {
                CaptureEvidence(evidenceLabel);
            }

            CleanupSaveSlot(saveSlot);
        }
    }
}
