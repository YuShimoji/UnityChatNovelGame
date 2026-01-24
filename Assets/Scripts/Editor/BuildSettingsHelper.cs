using UnityEditor;
using System.Linq;

[InitializeOnLoad]
public static class BuildSettingsHelper
{
    static BuildSettingsHelper()
    {
        EnsureDebugChatSceneInBuildSettings();
    }

    [MenuItem("Tools/FoundPhone/Fix Build Settings")]
    public static void EnsureDebugChatSceneInBuildSettings()
    {
        string scenePath = "Assets/Scenes/DebugChatScene.unity";
        var scenes = EditorBuildSettings.scenes.ToList();

        if (!scenes.Any(s => s.path == scenePath))
        {
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            UnityEngine.Debug.Log($"Added {scenePath} to Build Settings.");
        }
    }
}
