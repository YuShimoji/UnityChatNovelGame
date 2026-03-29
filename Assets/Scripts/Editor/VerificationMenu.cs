using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Assets.Scripts.Dev;

namespace ProjectFoundPhone.EditorTools
{
    public static class VerificationMenu
    {
        [MenuItem("Tools/Verification/Run Chat UI Verification")]
        public static void RunChatUIVerification()
        {
            SetupAndRun("DebugChatScene", "single_capture", true);
        }

        [MenuItem("Tools/Verification/Run Vertical Slice Full Playthrough")]
        public static void RunVerticalSliceFullPlaythrough()
        {
            SetupAndRun("DebugChatScene", "vertical_slice_full", true);
        }

        [MenuItem("Tools/Verification/Run Performance Measurement")]
        public static void RunPerformanceMeasurement()
        {
            SetupAndRun("DebugChatScene", "performance_measurement", true);
        }

        [MenuItem("Tools/Verification/Utilities/Scan DebugChatScene Missing Scripts")]
        public static void RunMissingScriptScan()
        {
            MissingScriptScanner.ScanDebugChatSceneMissingScripts();
        }

        public static void RunVerticalSliceFullPlaythroughBatch()
        {
            SetupAndRun("DebugChatScene", "vertical_slice_full", false);
        }

        public static void RunPerformanceMeasurementBatch()
        {
            SetupAndRun("DebugChatScene", "performance_measurement", false);
        }

        private static void SetupAndRun(string targetSceneName, string verificationFlow, bool promptToSave)
        {
            if (promptToSave)
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    return;
                }
            }
            else
            {
                AssetDatabase.SaveAssets();
            }

            // Create a temporary scene for the runner
            var runnerScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Create Automator
            GameObject go = new GameObject("VerificationRunner");
            var automator = go.AddComponent<VerificationAutomator>();
            automator.TargetScene = targetSceneName;
            automator.VerificationFlow = verificationFlow;

            // Start Play Mode
            Debug.Log($"Starting Verification for {targetSceneName} ({verificationFlow})...");
            EditorApplication.isPlaying = true;
        }
    }
}
