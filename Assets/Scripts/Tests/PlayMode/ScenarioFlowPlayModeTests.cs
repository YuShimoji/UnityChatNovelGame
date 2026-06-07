using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;
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

        private static CharacterProfile CreateRuntimeCharacterProfile(string characterID, string displayName, bool isPlayer, IconSide iconSide, Sprite iconSprite)
        {
            CharacterProfile profile = ScriptableObject.CreateInstance<CharacterProfile>();
            SetPrivateField(profile, "m_CharacterID", characterID);
            SetPrivateField(profile, "m_DisplayName", displayName);
            SetPrivateField(profile, "m_IsPlayer", isPlayer);
            SetPrivateField(profile, "m_ThemeColor", isPlayer ? new Color(0.2f, 0.6f, 1.0f) : new Color(0.3f, 0.3f, 0.35f));
            SetPrivateField(profile, "m_DisplayMode", CharacterDisplayMode.IconOnly);
            SetPrivateField(profile, "m_IconSide", iconSide);
            SetPrivateField(profile, "m_Icon", iconSprite);
            return profile;
        }

        private static Dictionary<string, CharacterProfile> ReplaceCharacterProfiles(CharacterDatabase database, params CharacterProfile[] profiles)
        {
            FieldInfo profilesField = typeof(CharacterDatabase).GetField("m_Profiles", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(profilesField, "CharacterDatabase.m_Profiles was not found.");

            var originalProfiles = profilesField.GetValue(database) as Dictionary<string, CharacterProfile>
                ?? new Dictionary<string, CharacterProfile>();
            var mergedProfiles = new Dictionary<string, CharacterProfile>(originalProfiles);

            foreach (CharacterProfile profile in profiles)
            {
                if (profile != null && !string.IsNullOrEmpty(profile.CharacterID))
                {
                    mergedProfiles[profile.CharacterID] = profile;
                }
            }

            profilesField.SetValue(database, mergedProfiles);
            return originalProfiles;
        }

        private static void RestoreCharacterProfiles(CharacterDatabase database, Dictionary<string, CharacterProfile> profiles)
        {
            if (database == null || profiles == null)
            {
                return;
            }

            FieldInfo profilesField = typeof(CharacterDatabase).GetField("m_Profiles", BindingFlags.NonPublic | BindingFlags.Instance);
            profilesField?.SetValue(database, profiles);
        }

        private static void SetPrivateField<T>(object target, string fieldName, T value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(field, $"Field '{fieldName}' was not found on {target.GetType().Name}.");
            field.SetValue(target, value);
        }

        private static Transform GetLatestMessageRow(ChatController chatController)
        {
            ScrollRect scrollRect = chatController.GetComponent<ScrollRect>();
            Assert.IsNotNull(scrollRect, "ScrollRect not found.");
            Assert.IsNotNull(scrollRect.content, "ScrollRect content not found.");
            Assert.Greater(scrollRect.content.childCount, 0, "No chat rows were created.");
            return scrollRect.content.GetChild(scrollRect.content.childCount - 1);
        }

        private static void AssertIconPlacement(Transform row, bool iconFirst)
        {
            Assert.AreEqual(2, row.childCount, $"Expected icon + bubble only, but row '{row.name}' had {row.childCount} children.");
            const string expectedName = "CharacterIconContainer";
            int iconIndex = iconFirst ? 0 : row.childCount - 1;
            Assert.AreEqual(expectedName, row.GetChild(iconIndex).name, $"Icon was not placed on the expected side for row '{row.name}'.");
        }

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
        public IEnumerator SP023_NarrationMargin_Start_EmitsExpectedBubbles()
        {
            LogAssert.ignoreFailingMessages = true;
            try
            {
                yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

                ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
                ChatController chatController = Object.FindFirstObjectByType<ChatController>();
                Assert.IsNotNull(scenarioManager, "ScenarioManager not found.");
                Assert.IsNotNull(chatController, "ChatController not found.");

                yield return PlayModeTestHelpers.WaitForBubbleCount(chatController, 8, 10f);

                ScrollRect scrollRect = chatController.GetComponent<ScrollRect>();
                Assert.IsNotNull(scrollRect, "ScrollRect not found.");
                Assert.GreaterOrEqual(scrollRect.content.childCount, 8, "SP023_NarrationMargin_Start should emit at least 8 bubbles including system messages.");
            }
            finally
            {
                LogAssert.ignoreFailingMessages = false;
            }
        }

        [UnityTest]
        public IEnumerator DebugChatScene_IconSide_ReordersCharacterIcons()
        {
            LogAssert.ignoreFailingMessages = true;
            CharacterDatabase characterDatabase = null;
            Dictionary<string, CharacterProfile> originalProfiles = null;
            Texture2D iconTexture = null;
            Sprite iconSprite = null;
            CharacterProfile autoProfile = null;
            CharacterProfile leftProfile = null;
            CharacterProfile rightProfile = null;

            try
            {
                yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene", PlayModeTestHelpers.SceneLoadTimeoutSeconds);

                ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
                ChatController chatController = Object.FindFirstObjectByType<ChatController>();
                characterDatabase = CharacterDatabase.Instance;
                Assert.IsNotNull(scenarioManager, "ScenarioManager not found.");
                Assert.IsNotNull(chatController, "ChatController not found.");
                Assert.IsNotNull(characterDatabase, "CharacterDatabase not found.");

                yield return PlayModeTestHelpers.WaitForBubbleCount(chatController, 8, 10f);
                scenarioManager.StopScenario();
                chatController.ClearMessages();
                yield return null;
                yield return null;

                iconTexture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
                Color[] pixels = new Color[16];
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = Color.white;
                }
                iconTexture.SetPixels(pixels);
                iconTexture.Apply();
                iconSprite = Sprite.Create(iconTexture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f));

                autoProfile = CreateRuntimeCharacterProfile("icon_auto", "Icon Auto", false, IconSide.Auto, iconSprite);
                leftProfile = CreateRuntimeCharacterProfile("icon_left", "Icon Left", false, IconSide.Left, iconSprite);
                rightProfile = CreateRuntimeCharacterProfile("icon_right", "Icon Right", false, IconSide.Right, iconSprite);
                originalProfiles = ReplaceCharacterProfiles(characterDatabase, autoProfile, leftProfile, rightProfile);

                chatController.AddMessage("icon_auto", "auto");
                yield return null;
                AssertIconPlacement(GetLatestMessageRow(chatController), iconFirst: true);

                chatController.AddMessage("icon_left", "left");
                yield return null;
                AssertIconPlacement(GetLatestMessageRow(chatController), iconFirst: true);

                chatController.AddMessage("icon_right", "right");
                yield return null;
                AssertIconPlacement(GetLatestMessageRow(chatController), iconFirst: false);
            }
            finally
            {
                RestoreCharacterProfiles(characterDatabase, originalProfiles);
                if (iconSprite != null) Object.Destroy(iconSprite);
                if (iconTexture != null) Object.Destroy(iconTexture);
                if (autoProfile != null) Object.Destroy(autoProfile);
                if (leftProfile != null) Object.Destroy(leftProfile);
                if (rightProfile != null) Object.Destroy(rightProfile);
                LogAssert.ignoreFailingMessages = false;
            }
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
