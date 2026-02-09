using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EditorScript
{
    [MenuItem("Tools/Edit Scene")]
    static void EditScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject dialogueViewObj = new GameObject("ChatDialogueView");
        dialogueViewObj.AddComponent<ChatDialogueView>();

        // DialogueRunnerに追加
        var runner = FindObjectOfType<DialogueRunner>();
        if (runner != null)
        {
            runner.dialoguePresenters.Add(dialogueViewObj.GetComponent<ChatDialogueView>());
        }

        // Scene保存
        EditorSceneManager.SaveScene(scene);
    }
}
