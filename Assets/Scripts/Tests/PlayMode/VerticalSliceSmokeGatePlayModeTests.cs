using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ProjectFoundPhone.Tests
{
    public class VerticalSliceSmokeGatePlayModeTests
    {
        private const int SaveSlot = 0;

        [UnityTest]
        public IEnumerator VerticalSlice_SmokeFlow_TitleToChat_SaveLoad()
        {
            yield return PlayModeTestHelpers.LoadSceneWithTimeout("TitleScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

            TitleScreenManager titleManager = Object.FindFirstObjectByType<TitleScreenManager>();
            Assert.IsNotNull(titleManager, "TitleScene: TitleScreenManager not found.");

            titleManager.StartNewGame();

            yield return PlayModeTestHelpers.WaitForScene("DebugChatScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

            ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
            ChatController chatController = Object.FindFirstObjectByType<ChatController>();
            Assert.IsNotNull(scenarioManager, "DebugChatScene: ScenarioManager not found.");
            Assert.IsNotNull(chatController, "DebugChatScene: ChatController not found.");

            scenarioManager.StartScenario("VerticalSlice_Start");
            yield return PlayModeTestHelpers.WaitForChatMessages(chatController, PlayModeTestHelpers.ChatMessageTimeoutSeconds,
                "Scenario did not emit any chat messages within the expected time.");

            SaveManager saveManager = SaveManager.Instance;
            Assert.IsNotNull(saveManager, "SaveManager.Instance returned null.");

            bool saved = saveManager.SaveGame(SaveSlot);
            Assert.IsTrue(saved, "SaveGame failed.");

            bool loaded = saveManager.LoadGame(SaveSlot);
            Assert.IsTrue(loaded, "LoadGame failed.");
        }

        [UnityTest]
        public IEnumerator DebugChatScene_Ch1Start_PreservesCurrentChannelAcrossSaveLoad()
        {
            yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

            ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
            ChatController chatController = Object.FindFirstObjectByType<ChatController>();
            SaveManager saveManager = SaveManager.Instance;

            Assert.IsNotNull(scenarioManager, "DebugChatScene: ScenarioManager not found.");
            Assert.IsNotNull(chatController, "DebugChatScene: ChatController not found.");
            Assert.IsNotNull(saveManager, "SaveManager.Instance returned null.");

            scenarioManager.StartScenario("Ch1_Day1_Opening");
            yield return PlayModeTestHelpers.WaitForCondition(
                () => scenarioManager.CurrentChannelID == "ch1",
                PlayModeTestHelpers.ChatMessageTimeoutSeconds,
                "ScenarioManager did not auto-assign channel 'ch1' from Ch1_Day1_Opening.");
            yield return PlayModeTestHelpers.WaitForChatMessages(chatController, PlayModeTestHelpers.ChatMessageTimeoutSeconds,
                "Ch1_Day1_Opening did not emit any chat messages within the expected time.");

            bool saved = saveManager.SaveGame(SaveSlot);
            Assert.IsTrue(saved, "SaveGame failed for Ch1 flow.");
            Assert.IsNotNull(saveManager.CurrentSaveData, "CurrentSaveData was null after saving Ch1 flow.");
            Assert.AreEqual("ch1", saveManager.CurrentSaveData.CurrentChannelID,
                "Save data did not preserve the current channel for Ch1.");

            bool loaded = saveManager.LoadGame(SaveSlot);
            Assert.IsTrue(loaded, "LoadGame failed for Ch1 flow.");
            yield return PlayModeTestHelpers.WaitForCondition(
                () => scenarioManager.CurrentChannelID == "ch1",
                PlayModeTestHelpers.ChatMessageTimeoutSeconds,
                "ScenarioManager did not restore channel 'ch1' after LoadGame.");

            Assert.AreEqual("ch1", scenarioManager.CurrentChannelID,
                "ScenarioManager lost CurrentChannel after Save/Load.");
        }

        [UnityTest]
        public IEnumerator DebugChatScene_DQTStart_EmitsMessages()
        {
            yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

            ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
            ChatController chatController = Object.FindFirstObjectByType<ChatController>();

            Assert.IsNotNull(scenarioManager, "DebugChatScene: ScenarioManager not found.");
            Assert.IsNotNull(chatController, "DebugChatScene: ChatController not found.");

            scenarioManager.StartScenario("DQT_Start");
            yield return PlayModeTestHelpers.WaitForChatMessages(chatController, PlayModeTestHelpers.ChatMessageTimeoutSeconds,
                "DQT_Start did not emit any chat messages within the expected time.");

            ScrollRect scrollRect = chatController.GetComponent<ScrollRect>();
            Assert.IsNotNull(scrollRect, "ScrollRect was not found on ChatController.");
            Assert.Greater(scrollRect.content.childCount, 0, "DQT_Start produced no chat bubbles.");
        }

        [UnityTest]
        public IEnumerator DebugChatScene_ChoiceAndImageFallback_AreUsable()
        {
            yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

            ChatController chatController = Object.FindFirstObjectByType<ChatController>();
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
            Object.Destroy(testSprite);
            Object.Destroy(tempTexture);
        }

        [UnityTearDown]
        public IEnumerator CaptureEvidenceOnFailure()
        {
            yield return PlayModeTestHelpers.SafeTeardown("VerticalSliceSmokeGate", SaveSlot);
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
