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

            // TitleScreenManager は ContentAuthoring が利用可能ならそちらに遷移する
            string expectedScene = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(0) != null
                && Application.CanStreamedLevelBeLoaded("ContentAuthoring")
                ? "ContentAuthoring" : "DebugChatScene";
            yield return PlayModeTestHelpers.WaitForScene(expectedScene, PlayModeTestHelpers.SceneLoadTimeoutSeconds);

            ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
            ChatController chatController = Object.FindFirstObjectByType<ChatController>();
            Assert.IsNotNull(scenarioManager, $"{expectedScene}: ScenarioManager not found.");
            Assert.IsNotNull(chatController, $"{expectedScene}: ChatController not found.");

            scenarioManager.StartScenario("DQT_Start");
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

            ScrollRect scrollRect = chatController.GetComponent<ScrollRect>();
            Assert.IsNotNull(scrollRect, "ScrollRect was not found on ChatController.");

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
            int beforeCount = scrollRect.content.childCount;
            chatController.AddImageMessage("player", testSprite);
            yield return null;

            // 画像メッセージによりバブルが1つ増えたことを確認
            Assert.Greater(scrollRect.content.childCount, beforeCount,
                "AddImageMessage did not create a new bubble.");

            chatController.HideChoices();
            Object.Destroy(testSprite);
            Object.Destroy(tempTexture);
        }

        [UnityTearDown]
        public IEnumerator CaptureEvidenceOnFailure()
        {
            yield return PlayModeTestHelpers.SafeTeardown("VerticalSliceSmokeGate", SaveSlot);
        }

    }
}
