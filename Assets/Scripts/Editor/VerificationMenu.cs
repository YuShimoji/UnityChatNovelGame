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
            SetupAndRun("DebugChatScene");
        }

        [MenuItem("Tools/Verification/Run Synthesis Verification")]
        public static void RunSynthesisVerification()
        {
            SetupAndRun("VerificationScene");
        }

        private static void SetupAndRun(string targetSceneName)
        {
            // Ask to save changes if needed
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            // Create a temporary scene for the runner
            var runnerScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // Create Automator
            GameObject go = new GameObject("VerificationRunner");
            var automator = go.AddComponent<VerificationAutomator>();
            automator.TargetScene = targetSceneName;
            
            // Ensure it survives load
            // Note: DontDestroyOnLoad only works at runtime. 
            // VerificationAutomator.Start() handles DontDestroyOnLoad when playing starts.
            
            // Start Play Mode
            Debug.Log($"Starting Verification for {targetSceneName}...");
            EditorApplication.isPlaying = true;
        }
    }
}
