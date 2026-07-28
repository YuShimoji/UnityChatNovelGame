using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ProjectFoundPhone.Tests
{
    /// <summary>
    /// PlayMode テスト共通ヘルパー。
    /// シーンロード、条件待ち、エビデンスキャプチャ、クリーンアップを集約。
    /// </summary>
    public static class PlayModeTestHelpers
    {
        private const string TestSaveDirectoryEnvironmentVariable =
            "FOUNDPHONE_TEST_SAVE_DIRECTORY";
        private const string TestSaveRootEnvironmentVariable =
            "FOUNDPHONE_TEST_SAVE_ROOT";
        private const string TestSaveRootDirectoryName = "FoundPhoneTests";

        public const string EvidenceRelativePath = "docs/verification";
        public const float SceneLoadTimeoutSeconds = 10f;
        public const float ChatMessageTimeoutSeconds = 5f;

        public static string RequireIsolatedSaveDirectory()
        {
            string configuredDirectory =
                Environment.GetEnvironmentVariable(TestSaveDirectoryEnvironmentVariable);
            Assert.That(
                configuredDirectory,
                Is.Not.Null.And.Not.Empty,
                $"Run Unity tests through tools/run-unity.ps1 -IsolateTestSaveData. "
                + $"{TestSaveDirectoryEnvironmentVariable} is not configured.");

            string fullDirectory = Path.GetFullPath(configuredDirectory);
            string configuredRoot =
                Environment.GetEnvironmentVariable(TestSaveRootEnvironmentVariable);
            Assert.That(
                configuredRoot,
                Is.Not.Null.And.Not.Empty,
                $"{TestSaveRootEnvironmentVariable} is not configured.");

            string allowedRoot = Path.GetFullPath(configuredRoot);
            Assert.That(
                Path.GetFileName(allowedRoot),
                Is.EqualTo(TestSaveRootDirectoryName),
                $"{TestSaveRootEnvironmentVariable} must end with {TestSaveRootDirectoryName}.");
            string allowedPrefix =
                allowedRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            Assert.That(
                fullDirectory.StartsWith(allowedPrefix, StringComparison.OrdinalIgnoreCase),
                Is.True,
                $"{TestSaveDirectoryEnvironmentVariable} must be under {allowedRoot}.");
            Assert.That(
                string.Equals(
                    fullDirectory,
                    Path.GetFullPath(Application.persistentDataPath),
                    StringComparison.OrdinalIgnoreCase),
                Is.False,
                "The isolated test directory must not equal Application.persistentDataPath.");

            Directory.CreateDirectory(fullDirectory);
            return fullDirectory;
        }

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
            string filePath = Path.Combine(
                RequireIsolatedSaveDirectory(),
                $"SaveData_{slotNumber}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// シーン破棄前にダイアログを停止する共通 teardown ヘルパー。
        /// StopScenario() → CancelActiveWait → DialogueRunner.Stop の順序で、
        /// Yarn VM が停止済みの状態で <<StartWait>> の CTS キャンセル継続が
        /// Continue() を呼んで DialogueException を起こす問題を回避する。
        /// </summary>
        public static IEnumerator SafeTeardown(string evidenceLabel, int saveSlot = 0)
        {
            // Yarn VM の StopScenario → Continue() 競合で生じる既知の DialogueException を受け流す
            LogAssert.ignoreFailingMessages = true;

            // DOTween の orphaned tween を先にキル (destroyed オブジェクトへのアクセス防止)
            DG.Tweening.DOTween.KillAll();

            ScenarioManager scenarioManager = UnityEngine.Object.FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null)
            {
                scenarioManager.StopScenario();
            }

            // 非同期コマンドの継続処理が完了するまで数フレーム待機
            yield return null;
            yield return null;
            yield return null;

            LogAssert.ignoreFailingMessages = false;

            TestStatus status = TestContext.CurrentContext.Result.Outcome.Status;
            if (status == TestStatus.Failed)
            {
                CaptureEvidence(evidenceLabel);
            }

            CleanupSaveSlot(saveSlot);
        }
    }
}
