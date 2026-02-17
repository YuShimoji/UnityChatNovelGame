using ProjectFoundPhone.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    public static class TitleSceneSetupTools
    {
        private const string TitleScenePath = "Assets/Scenes/TitleScene.unity";
        private const string DefaultNewGameScene = "DebugChatScene";

        [MenuItem("Tools/FoundPhone/Re-Setup/TitleScene Manager")]
        public static void ReSetupTitleSceneManager()
        {
            SceneSetupResult result = EnsureTitleSceneManager();
            Debug.Log($"TitleScene setup completed. Created={result.Created}, MissingScriptRemoved={result.RemovedMissingScripts}");
        }

        [MenuItem("Tools/FoundPhone/Re-Setup/Vertical Slice Essentials")]
        public static void ReSetupVerticalSliceEssentials()
        {
            BuildSettingsHelper.EnsureDebugChatSceneInBuildSettings();
            SceneSetupResult result = EnsureTitleSceneManager();

            Debug.Log(
                "Vertical Slice essentials setup completed. " +
                $"TitleSceneManagerCreated={result.Created}, MissingScriptRemoved={result.RemovedMissingScripts}"
            );
        }

        [MenuItem("Tools/FoundPhone/Validate/TitleScene Manager Wiring")]
        public static void ValidateTitleSceneManagerWiring()
        {
            var scene = EditorSceneManager.OpenScene(TitleScenePath, OpenSceneMode.Single);
            TitleScreenManager manager = Object.FindFirstObjectByType<TitleScreenManager>();

            if (manager == null)
            {
                Debug.LogError("TitleScene validation failed: TitleScreenManager is missing.");
                return;
            }

            Debug.Log($"TitleScene validation passed: {scene.name} contains TitleScreenManager.");
        }

        private static SceneSetupResult EnsureTitleSceneManager()
        {
            var scene = EditorSceneManager.OpenScene(TitleScenePath, OpenSceneMode.Single);
            GameObject managerObject = GameObject.Find("TitleScreenManager");
            bool created = false;

            if (managerObject == null)
            {
                managerObject = new GameObject("TitleScreenManager");
                created = true;
            }

            int removedMissingScripts = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(managerObject);
            TitleScreenManager manager = managerObject.GetComponent<TitleScreenManager>();
            if (manager == null)
            {
                manager = managerObject.AddComponent<TitleScreenManager>();
            }

            SerializedObject serializedManager = new SerializedObject(manager);
            SerializedProperty newGameSceneProperty = serializedManager.FindProperty("m_NewGameSceneName");
            if (newGameSceneProperty != null && string.IsNullOrWhiteSpace(newGameSceneProperty.stringValue))
            {
                newGameSceneProperty.stringValue = DefaultNewGameScene;
                serializedManager.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            return new SceneSetupResult(created, removedMissingScripts);
        }

        private readonly struct SceneSetupResult
        {
            public SceneSetupResult(bool created, int removedMissingScripts)
            {
                Created = created;
                RemovedMissingScripts = removedMissingScripts;
            }

            public bool Created { get; }
            public int RemovedMissingScripts { get; }
        }
    }
}
