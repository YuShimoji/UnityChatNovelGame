using UnityEditor;
using System.Linq;

[InitializeOnLoad]
public static class BuildSettingsHelper
{
    static BuildSettingsHelper()
    {
        EnsureCoreScenesInBuildSettings();
    }

    [MenuItem("Tools/FoundPhone/Fix Build Settings")]
    public static void EnsureCoreScenesInBuildSettings()
    {
        var requiredScenes = new System.Collections.Generic.List<string>
        {
            "Assets/Scenes/TitleScene.unity",
            "Assets/Scenes/DebugChatScene.unity",
            "Assets/Scenes/MVPScene.unity"
        };

        const string contentAuthoringScenePath = "Assets/Scenes/ContentAuthoring.unity";
        string contentAuthoringSceneFullPath = System.IO.Path.Combine(
            UnityEngine.Application.dataPath,
            "Scenes",
            "ContentAuthoring.unity");
        if (System.IO.File.Exists(contentAuthoringSceneFullPath))
        {
            requiredScenes.Insert(1, contentAuthoringScenePath);
        }
        
        var scenes = EditorBuildSettings.scenes.ToList();
        var reorderedScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

        foreach (string scenePath in requiredScenes)
        {
            EditorBuildSettingsScene existingScene = scenes.FirstOrDefault(s => s.path == scenePath);
            reorderedScenes.Add(existingScene ?? new EditorBuildSettingsScene(scenePath, true));
        }

        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (!requiredScenes.Contains(scene.path))
            {
                reorderedScenes.Add(scene);
            }
        }

        bool changed =
            scenes.Count != reorderedScenes.Count ||
            scenes.Where((scene, index) => scene.path != reorderedScenes[index].path || scene.enabled != reorderedScenes[index].enabled).Any();

        if (changed)
        {
            EditorBuildSettings.scenes = reorderedScenes.ToArray();
        }
    }

    public static void EnsureDebugChatSceneInBuildSettings()
    {
        EnsureCoreScenesInBuildSettings();
    }
}
