using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;
using Assets.Scripts.Dev;

public class VerificationTool
{
    [MenuItem("Tools/FoundPhone/Run Verification")]
    public static void RunVerification()
    {
        Debug.Log("VerificationTool: RunVerification called.");
        SetupVerificationEnvironment();
        
        string scenePath = "Assets/Scenes/VerificationScene.unity";
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        
        EditorApplication.EnterPlaymode();
    }

    public static void SetupVerificationEnvironment()
    {
        string scenePath = "Assets/Scenes/VerificationScene.unity";
        
        // Create Scene if not exists
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var go = new GameObject("VerificationAutomator");
        go.AddComponent<VerificationAutomator>(); // Ensure this creates the script ref
        
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("VerificationTool: VerificationScene created.");

        // Add to Build Settings
        AddSceneToBuildSettings(scenePath);
        AddSceneToBuildSettings("Assets/Scenes/DebugChatScene.unity");
    }

    private static void AddSceneToBuildSettings(string path)
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        if (!scenes.Any(s => s.path == path))
        {
            scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
