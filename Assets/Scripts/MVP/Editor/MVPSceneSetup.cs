using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ProjectFoundPhone.MVP.Editor
{
    /// <summary>
    /// MVPシーンを自動構築するEditorツール。
    /// Tools > FoundPhone > Setup MVP Scene で実行。
    /// 空シーンにMVPGameControllerを配置して保存する。
    /// </summary>
    public class MVPSceneSetup : EditorWindow
    {
        [MenuItem("Tools/FoundPhone/Setup MVP Scene")]
        public static void SetupMVPScene()
        {
            // 新しいシーンを作成
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // MVPGameController を配置
            GameObject controllerObj = new GameObject("MVPGameController");
            controllerObj.AddComponent<MVPGameController>();

            // カメラの背景色を暗くする
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
            }

            // シーンを保存
            string scenePath = "Assets/Scenes/MVPScene.unity";
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"MVP Scene created and saved to {scenePath}");
            Debug.Log("Press Play to test the MVP flow: Title → Chat → Choice → End");
        }
    }
}
