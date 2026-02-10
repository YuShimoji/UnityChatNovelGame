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
            int oldLength = runner.dialogueViews != null ? runner.dialogueViews.Length : 0;
            Array.Resize(ref runner.dialogueViews, oldLength + 1);
            runner.dialogueViews[oldLength] = view;
        }

        EditorSceneManager.SaveScene(scene);
    }
}
#endif
