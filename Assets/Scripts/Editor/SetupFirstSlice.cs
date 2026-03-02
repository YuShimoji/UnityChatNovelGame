using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ProjectFoundPhone.Core;

public static class SetupFirstSlice
{
    public static void Run()
    {
        string scenePath = "Assets/Scenes/ContentAuthoring.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        
        var scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
        if (scenarioManager != null)
        {
            var so = new SerializedObject(scenarioManager);
            so.Update();
            so.FindProperty("m_StartNode").stringValue = "FirstSlice_Start";
            so.ApplyModifiedProperties();
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("Successfully set m_StartNode to FirstSlice_Start.");
        }
        else
        {
            Debug.LogError("ScenarioManager not found in scene.");
        }
        
        EditorApplication.Exit(0);
    }
}
