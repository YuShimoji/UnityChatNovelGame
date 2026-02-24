using System;
using System.Collections;
using System.Collections.Generic;
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

            scenarioManager.StartScenario("VerticalSlice_Start");

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

        [UnityTest]
        public IEnumerator DebugChatScene_ChoiceAndImageFallback_AreUsable()
        {
            yield return LoadSceneWithTimeout("DebugChatScene", SceneLoadTimeoutSeconds);

            ChatController chatController = UnityEngine.Object.FindFirstObjectByType<ChatController>();
            Assert.IsNotNull(chatController, "DebugChatScene: ChatController not found.");

            chatController.ShowChoices(new List<string> { "A", "B" }, _ => { });
            yield return null;

            GameObject choiceA = GameObject.Find("ChoiceA");
            GameObject choiceB = GameObject.Find("ChoiceB");
            Assert.IsNotNull(choiceA, "ChoiceA was not created.");
            Assert.IsNotNull(choiceB, "ChoiceB was not created.");
            Assert.IsTrue(choiceA.activeInHierarchy, "ChoiceA is inactive.");
            Assert.IsTrue(choiceB.activeInHierarchy, "ChoiceB is inactive.");

            Texture2D tempTexture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            Color[] colors = new Color[16];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.white;
            }
            tempTexture.SetPixels(colors);
            tempTexture.Apply();

            Sprite testSprite = Sprite.Create(tempTexture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f));
            chatController.AddImageMessage("player", testSprite);
            yield return null;

            ScrollRect scrollRect = chatController.GetComponent<ScrollRect>();
            Assert.IsNotNull(scrollRect, "ScrollRect was not found on ChatController.");
            Assert.IsNotNull(scrollRect.content, "ScrollRect content is null.");
            Assert.Greater(scrollRect.content.childCount, 0, "No chat bubbles were created.");

            GameObject lastBubble = scrollRect.content.GetChild(scrollRect.content.childCount - 1).gameObject;
            Assert.IsTrue(lastBubble.activeInHierarchy, "Image bubble is inactive.");

            Transform imageContentTransform = lastBubble.transform.Find("ImageContent");
            Image imageContent = imageContentTransform != null ? imageContentTransform.GetComponent<Image>() : null;
            bool hasImage = imageContent != null && imageContent.sprite != null;

            bool hasFallbackText = TryFindFallbackImageText(lastBubble, out string bubbleText)
                && bubbleText.Contains("[Image:");

            Assert.IsTrue(hasImage || hasFallbackText, "Image message did not render as image or fallback text.");

            chatController.HideChoices();
            UnityEngine.Object.Destroy(testSprite);
            UnityEngine.Object.Destroy(tempTexture);
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
            SceneManager.LoadScene(sceneName);
            yield return null; // Wait for the scene to properly activate
            
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

        private static bool TryFindFallbackImageText(GameObject bubble, out string textValue)
        {
            textValue = string.Empty;
            if (bubble == null)
            {
                return false;
            }

            Component[] components = bubble.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null)
                {
                    continue;
                }

                var textProperty = component.GetType().GetProperty("text");
                if (textProperty == null || textProperty.PropertyType != typeof(string))
                {
                    continue;
                }

                string value = textProperty.GetValue(component) as string;
                if (!string.IsNullOrEmpty(value))
                {
                    textValue = value;
                    return true;
                }
            }

            return false;
        }
    }
}
