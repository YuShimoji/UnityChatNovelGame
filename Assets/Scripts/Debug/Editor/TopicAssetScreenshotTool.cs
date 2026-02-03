using System.IO;
using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Logging.Editor
{
    /// <summary>
    /// TopicDataアセットのInspector表示スクリーンショットを取得するエディタツール
    /// TASK_013のEvidence取得用
    /// </summary>
    public static class TopicAssetScreenshotTool
    {
        private const string c_TopicAssetPath = "Assets/Resources/Topics/debug_topic_01.asset";
        private const string c_EvidencePath = "docs/evidence/task011_topic_assets.png";

        /// <summary>
        /// TopicDataアセットを選択し、Inspector表示のスクリーンショットを取得する
        /// </summary>
        [MenuItem("Tools/FoundPhone/Capture Topic Asset Screenshot")]
        public static void CaptureTopicAssetScreenshot()
        {
            // アセットを読み込み
            TopicData topicAsset = AssetDatabase.LoadAssetAtPath<TopicData>(c_TopicAssetPath);
            
            if (topicAsset == null)
            {
                UnityEngine.Debug.LogError($"TopicAssetScreenshotTool: Failed to load asset at {c_TopicAssetPath}");
                EditorUtility.DisplayDialog(
                    "Error",
                    $"Failed to load TopicData asset at:\n{c_TopicAssetPath}\n\nPlease ensure the asset exists.",
                    "OK"
                );
                return;
            }

            // アセットを選択してInspectorに表示
            Selection.activeObject = topicAsset;
            EditorUtility.FocusProjectWindow();
            
            // プロジェクトウィンドウでアセットを選択状態にする
            EditorGUIUtility.PingObject(topicAsset);

            // Evidenceディレクトリが存在しない場合は作成
            string evidenceDir = Path.GetDirectoryName(c_EvidencePath);
            if (!string.IsNullOrEmpty(evidenceDir) && !Directory.Exists(evidenceDir))
            {
                Directory.CreateDirectory(evidenceDir);
                UnityEngine.Debug.Log($"Created evidence directory: {evidenceDir}");
            }

            // スクリーンショットを取得
            // 注意: Unity Editor APIではInspectorウィンドウの内容を直接取得できないため、
            // スクリーン全体のスクリーンショットを取得します
            // ユーザーは手動でInspectorウィンドウを表示してから実行してください
            
            string fullPath = Path.Combine(Application.dataPath, "..", c_EvidencePath);
            fullPath = Path.GetFullPath(fullPath);
            
            // 既存のファイルがあれば削除
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // スクリーンショットを取得（Unity 2022.1以降）
#if UNITY_2022_1_OR_NEWER
            // 相対パスで保存（プロジェクトルートからの相対パス）
            string relativePath = c_EvidencePath.Replace('\\', '/');
            ScreenCapture.CaptureScreenshot(relativePath, 1);
            
            UnityEngine.Debug.Log($"Screenshot captured to: {fullPath}");
            UnityEngine.Debug.Log($"Please ensure the Inspector window is visible and showing the TopicData asset before taking the screenshot.");
            
            EditorUtility.DisplayDialog(
                "Screenshot Captured",
                $"Screenshot saved to:\n{fullPath}\n\nNote: This captures the entire screen. Please ensure the Inspector window is visible and showing the TopicData asset.",
                "OK"
            );
#else
            UnityEngine.Debug.LogWarning("ScreenCapture.CaptureScreenshot requires Unity 2022.1 or newer. Please take a manual screenshot.");
            EditorUtility.DisplayDialog(
                "Manual Screenshot Required",
                $"Unity version does not support automatic screenshot capture.\n\nPlease manually take a screenshot:\n1. Ensure Inspector window shows the TopicData asset\n2. Use Win+Shift+S (Windows) to capture\n3. Save to: {fullPath}",
                "OK"
            );
#endif

            // アセットを再インポート（必要に応じて）
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// TopicDataアセットを選択する（スクリーンショット取得前の準備）
        /// </summary>
        [MenuItem("Tools/FoundPhone/Select Topic Asset for Screenshot")]
        public static void SelectTopicAsset()
        {
            TopicData topicAsset = AssetDatabase.LoadAssetAtPath<TopicData>(c_TopicAssetPath);
            
            if (topicAsset == null)
            {
                UnityEngine.Debug.LogError($"TopicAssetScreenshotTool: Failed to load asset at {c_TopicAssetPath}");
                EditorUtility.DisplayDialog(
                    "Error",
                    $"Failed to load TopicData asset at:\n{c_TopicAssetPath}",
                    "OK"
                );
                return;
            }

            // アセットを選択
            Selection.activeObject = topicAsset;
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(topicAsset);
            
            UnityEngine.Debug.Log($"Selected TopicData asset: {topicAsset.name} (ID: {topicAsset.TopicID})");
            UnityEngine.Debug.Log("Please ensure the Inspector window is visible and showing the TopicData asset, then use 'Tools/FoundPhone/Capture Topic Asset Screenshot' to capture.");
        }
    }
}
