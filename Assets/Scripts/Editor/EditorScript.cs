#if YARN_SPINNER
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using ProjectFoundPhone.UI;

public class EditorScript
{
    [MenuItem("Tools/Edit Scene")]
    private static void EditScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject dialogueViewObj = new GameObject("ChatDialogueView");
        ChatDialogueView view = dialogueViewObj.AddComponent<ChatDialogueView>();

        var runner = UnityEngine.Object.FindFirstObjectByType<DialogueRunner>();
        if (runner != null)
        {
            SerializedObject so = new SerializedObject(runner);
            so.Update();
            SerializedProperty presenters = so.FindProperty("dialoguePresenters");
            if (presenters != null)
            {
                int index = presenters.arraySize;
                presenters.arraySize = index + 1;
                presenters.GetArrayElementAtIndex(index).objectReferenceValue = view;
            }
            so.ApplyModifiedProperties();
        }

        EditorSceneManager.SaveScene(scene);
    }
}
#endif
