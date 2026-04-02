using System.Collections;
using NUnit.Framework;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ProjectFoundPhone.Tests
{
    /// <summary>
    /// シナリオフロー PlayMode テスト。
    /// ETK ノードを使った Yarn コマンド網羅 + Ch2 起動 + Save/Load 拡張。
    /// </summary>
    public class ScenarioFlowPlayModeTests
    {
        private const int SaveSlot = 0;

        [UnityTest]
        public IEnumerator ETK_Commands_EmitsMessagesWithoutException()
        {
            yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

            ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
            ChatController chatController = Object.FindFirstObjectByType<ChatController>();
            Assert.IsNotNull(scenarioManager, "ScenarioManager not found.");
            Assert.IsNotNull(chatController, "ChatController not found.");

            scenarioManager.StartScenario("ETK_Commands");
            yield return PlayModeTestHelpers.WaitForChatMessages(chatController, PlayModeTestHelpers.ChatMessageTimeoutSeconds,
                "ETK_Commands did not emit any chat messages.");

            ScrollRect scrollRect = chatController.GetComponent<ScrollRect>();
            Assert.IsNotNull(scrollRect, "ScrollRect not found.");
            Assert.Greater(scrollRect.content.childCount, 0, "ETK_Commands produced no chat bubbles.");
        }

        [UnityTest]
        public IEnumerator ETK_RichText_EmitsMessagesWithoutException()
        {
            yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

            ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
            ChatController chatController = Object.FindFirstObjectByType<ChatController>();
            Assert.IsNotNull(scenarioManager, "ScenarioManager not found.");
            Assert.IsNotNull(chatController, "ChatController not found.");

            scenarioManager.StartScenario("ETK_RichText");
            yield return PlayModeTestHelpers.WaitForChatMessages(chatController, PlayModeTestHelpers.ChatMessageTimeoutSeconds,
                "ETK_RichText did not emit any chat messages.");

            ScrollRect scrollRect = chatController.GetComponent<ScrollRect>();
            Assert.Greater(scrollRect.content.childCount, 2, "ETK_RichText should produce multiple bubbles.");
        }

        [UnityTest]
        public IEnumerator Ch2_LocationConfusion_StartsWithoutException()
        {
            yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

            ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
            ChatController chatController = Object.FindFirstObjectByType<ChatController>();
            Assert.IsNotNull(scenarioManager, "ScenarioManager not found.");
            Assert.IsNotNull(chatController, "ChatController not found.");

            scenarioManager.StartScenario("Ch2_Opening");
            yield return PlayModeTestHelpers.WaitForChatMessages(chatController, PlayModeTestHelpers.ChatMessageTimeoutSeconds,
                "Ch2 did not emit any chat messages.");

            ScrollRect scrollRect = chatController.GetComponent<ScrollRect>();
            Assert.Greater(scrollRect.content.childCount, 0, "Ch2 produced no chat bubbles.");
        }

        [UnityTest]
        public IEnumerator SaveLoad_MultipleCycles_DoesNotCorruptState()
        {
            yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

            ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
            ChatController chatController = Object.FindFirstObjectByType<ChatController>();
            SaveManager saveManager = SaveManager.Instance;

            Assert.IsNotNull(scenarioManager, "ScenarioManager not found.");
            Assert.IsNotNull(chatController, "ChatController not found.");
            Assert.IsNotNull(saveManager, "SaveManager.Instance returned null.");

            scenarioManager.StartScenario("DQT_Start");
            yield return PlayModeTestHelpers.WaitForChatMessages(chatController, PlayModeTestHelpers.ChatMessageTimeoutSeconds,
                "DQT_Start did not emit messages.");

            // Save/Load を3回繰り返し、状態破損がないことを確認
            for (int i = 0; i < 3; i++)
            {
                bool saved = saveManager.SaveGame(SaveSlot);
                Assert.IsTrue(saved, $"SaveGame failed on cycle {i + 1}.");

                bool loaded = saveManager.LoadGame(SaveSlot);
                Assert.IsTrue(loaded, $"LoadGame failed on cycle {i + 1}.");
            }

            // Load 後も ScenarioManager が有効であることを確認
            Assert.IsNotNull(Object.FindFirstObjectByType<ScenarioManager>(),
                "ScenarioManager was destroyed after multiple Save/Load cycles.");
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return PlayModeTestHelpers.SafeTeardown("ScenarioFlow", SaveSlot);
        }
    }
}
