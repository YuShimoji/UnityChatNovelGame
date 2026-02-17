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
        string[] requiredScenes = new[]
        {
            "Assets/Scenes/TitleScene.unity",
            "Assets/Scenes/DebugChatScene.unity"
        };
        
        var scenes = EditorBuildSettings.scenes.ToList();
        bool changed = false;

        foreach (string scenePath in requiredScenes)
        {
            if (!scenes.Any(s => s.path == scenePath))
            {
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                UnityEngine.Debug.Log($"Added {scenePath} to Build Settings.");
                changed = true;
            }
        }

        if (changed)
        {
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
