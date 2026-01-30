using System.IO;
using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Debug.Editor
{
    /// <summary>
    /// TopicData繧｢繧ｻ繝・ヨ縺ｮInspector陦ｨ遉ｺ繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧貞叙蠕励☆繧九お繝・ぅ繧ｿ繝・・繝ｫ
    /// TASK_013縺ｮEvidence蜿門ｾ礼畑
    /// </summary>
    public static class TopicAssetScreenshotTool
    {
        private const string c_TopicAssetPath = "Assets/Resources/Topics/debug_topic_01.asset";
        private const string c_EvidencePath = "docs/evidence/task011_topic_assets.png";

        /// <summary>
        /// TopicData繧｢繧ｻ繝・ヨ繧帝∈謚槭＠縲！nspector陦ｨ遉ｺ縺ｮ繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧貞叙蠕励☆繧・        /// </summary>
        [MenuItem("Tools/FoundPhone/Capture Topic Asset Screenshot")]
        public static void CaptureTopicAssetScreenshot()
        {
            // 繧｢繧ｻ繝・ヨ繧定ｪｭ縺ｿ霎ｼ縺ｿ
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

            // 繧｢繧ｻ繝・ヨ繧帝∈謚槭＠縺ｦInspector縺ｫ陦ｨ遉ｺ
            Selection.activeObject = topicAsset;
            EditorUtility.FocusProjectWindow();
            
            // 繝励Ο繧ｸ繧ｧ繧ｯ繝医え繧｣繝ｳ繝峨え縺ｧ繧｢繧ｻ繝・ヨ繧帝∈謚樒憾諷九↓縺吶ｋ
            EditorGUIUtility.PingObject(topicAsset);

            // Evidence繝・ぅ繝ｬ繧ｯ繝医Μ縺悟ｭ伜惠縺励↑縺・ｴ蜷医・菴懈・
            string evidenceDir = Path.GetDirectoryName(c_EvidencePath);
            if (!string.IsNullOrEmpty(evidenceDir) && !Directory.Exists(evidenceDir))
            {
                Directory.CreateDirectory(evidenceDir);
                UnityEngine.Debug.Log($"Created evidence directory: {evidenceDir}");
            }

            // 繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧貞叙蠕・            // 豕ｨ諢・ Unity Editor API縺ｧ縺ｯInspector繧ｦ繧｣繝ｳ繝峨え縺ｮ蜀・ｮｹ繧堤峩謗･蜿門ｾ励〒縺阪↑縺・◆繧√・            // 繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ蜈ｨ菴薙・繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧貞叙蠕励＠縺ｾ縺・            // 繝ｦ繝ｼ繧ｶ繝ｼ縺ｯ謇句虚縺ｧInspector繧ｦ繧｣繝ｳ繝峨え繧定｡ｨ遉ｺ縺励※縺九ｉ螳溯｡後＠縺ｦ縺上□縺輔＞
            
            string fullPath = Path.Combine(Application.dataPath, "..", c_EvidencePath);
            fullPath = Path.GetFullPath(fullPath);
            
            // 譌｢蟄倥・繝輔ぃ繧､繝ｫ縺後≠繧後・蜑企勁
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // 繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧貞叙蠕暦ｼ・nity 2022.1莉･髯搾ｼ・#if UNITY_2022_1_OR_NEWER
            // 逶ｸ蟇ｾ繝代せ縺ｧ菫晏ｭ假ｼ医・繝ｭ繧ｸ繧ｧ繧ｯ繝医Ν繝ｼ繝医°繧峨・逶ｸ蟇ｾ繝代せ・・            string relativePath = c_EvidencePath.Replace('\\', '/');
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

            // 繧｢繧ｻ繝・ヨ繧貞・繧､繝ｳ繝昴・繝茨ｼ亥ｿ・ｦ√↓蠢懊§縺ｦ・・            AssetDatabase.Refresh();
        }

        /// <summary>
        /// TopicData繧｢繧ｻ繝・ヨ繧帝∈謚槭☆繧具ｼ医せ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ蜿門ｾ怜燕縺ｮ貅門ｙ・・        /// </summary>
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

            // 繧｢繧ｻ繝・ヨ繧帝∈謚・            Selection.activeObject = topicAsset;
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(topicAsset);
            
            UnityEngine.Debug.Log($"Selected TopicData asset: {topicAsset.name} (ID: {topicAsset.TopicID})");
            UnityEngine.Debug.Log("Please ensure the Inspector window is visible and showing the TopicData asset, then use 'Tools/FoundPhone/Capture Topic Asset Screenshot' to capture.");
        }
    }
}
