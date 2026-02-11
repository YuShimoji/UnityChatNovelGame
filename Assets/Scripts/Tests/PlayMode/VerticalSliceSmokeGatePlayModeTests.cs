using System;
using System.Collections;
using System.IO;
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
    public class VerticalSliceSmokeGatePlayModeTests
    {
        private const string EvidenceRelativePath = "docs/evidence/TASK_047";
        private const int SaveSlot = 0;
        private const float SceneLoadTimeoutSeconds = 10f;
        private const float ChatMessageTimeoutSeconds = 5f;

        [UnityTest]
        public IEnumerator VerticalSlice_SmokeFlow_TitleToChat_SaveLoad()
        {
            yield return LoadSceneWithTimeout("TitleScene", SceneLoadTimeoutSeconds);

            TitleScreenManager titleManager = UnityEngine.Object.FindFirstObjectByType<TitleScreenManager>();
            Assert.IsNotNull(titleManager, "TitleScene: TitleScreenManager not found.");

            titleManager.StartNewGame();

            yield return WaitForScene("DebugChatScene", SceneLoadTimeoutSeconds);

            ScenarioManager scenarioManager = UnityEngine.Object.FindFirstObjectByType<ScenarioManager>();
            ChatController chatController = UnityEngine.Object.FindFirstObjectByType<ChatController>();
            Assert.IsNotNull(scenarioManager, "DebugChatScene: ScenarioManager not found.");
            Assert.IsNotNull(chatController, "DebugChatScene: ChatController not found.");

            scenarioManager.StartScenario("Start");

            bool messagesAppeared = false;
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < ChatMessageTimeoutSeconds)
            {
                ScrollRect scrollRect = chatController.GetComponent<ScrollRect>();
                if (scrollRect != null && scrollRect.content != null && scrollRect.content.childCount > 0)
                {
                    messagesAppeared = true;
                    break;
                }
                yield return null;
            }

            Assert.IsTrue(messagesAppeared, "Scenario did not emit any chat messages within the expected time.");

            SaveManager saveManager = SaveManager.Instance;
            Assert.IsNotNull(saveManager, "SaveManager.Instance returned null.");

            bool saved = saveManager.SaveGame(SaveSlot);
            Assert.IsTrue(saved, "SaveGame failed.");

            bool loaded = saveManager.LoadGame(SaveSlot);
            Assert.IsTrue(loaded, "LoadGame failed.");

            CleanupSaveSlot(SaveSlot);
        }

        [TearDown]
        public void CaptureEvidenceOnFailure()
        {
            TestStatus status = TestContext.CurrentContext.Result.Outcome.Status;
            if (status == TestStatus.Failed)
            {
                CaptureEvidence("VerticalSliceSmokeGate");
            }

            CleanupSaveSlot(SaveSlot);
        }

        private static IEnumerator LoadSceneWithTimeout(string sceneName, float timeoutSeconds)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            Assert.IsNotNull(operation, $"LoadSceneAsync returned null for {sceneName}.");

            float startTime = Time.realtimeSinceStartup;
            while (!operation.isDone)
            {
                if (Time.realtimeSinceStartup - startTime > timeoutSeconds)
                {
                    Assert.Fail($"Timeout loading scene '{sceneName}' after {timeoutSeconds} seconds.");
                }
                yield return null;
            }
        }

        private static IEnumerator WaitForScene(string sceneName, float timeoutSeconds)
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

        private static void CaptureEvidence(string label)
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string evidenceDir = Path.Combine(projectRoot, EvidenceRelativePath);
            Directory.CreateDirectory(evidenceDir);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string sceneName = SceneManager.GetActiveScene().name;
            string baseName = $"{label}_{timestamp}_{sceneName}";

            string screenshotPath = Path.Combine(evidenceDir, $"{baseName}.png");
            ScreenCapture.CaptureScreenshot(screenshotPath);

            string logPath = Path.Combine(evidenceDir, $"{baseName}.txt");
            string message = $"Test Failed: {TestContext.CurrentContext.Test.Name}\n" +
                             $"Message: {TestContext.CurrentContext.Result.Message}\n" +
                             $"Scene: {sceneName}\n" +
                             $"Time: {DateTime.Now:O}\n";
            File.WriteAllText(logPath, message);

            Debug.Log($"VerticalSliceSmokeGate: Evidence saved to {evidenceDir}");
        }

        private static void CleanupSaveSlot(int slotNumber)
        {
            string filePath = Path.Combine(Application.persistentDataPath, $"SaveData_{slotNumber}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
