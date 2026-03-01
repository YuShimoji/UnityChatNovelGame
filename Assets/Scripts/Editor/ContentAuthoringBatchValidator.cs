using ProjectFoundPhone.Core;
using ProjectFoundPhone.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace ProjectFoundPhone.Editor
{
    [InitializeOnLoad]
    public static class ContentAuthoringBatchValidator
    {
        private const string ScenePath = "Assets/Scenes/ContentAuthoring.unity";
        private const double ValidationDelaySeconds = 4.0d;
        private const string ActiveKey = "ContentAuthoringBatchValidator.Active";
        private const string ExitWhenFinishedKey = "ContentAuthoringBatchValidator.ExitWhenFinished";
        private const string StageKey = "ContentAuthoringBatchValidator.Stage";
        private const string DeadlineKey = "ContentAuthoringBatchValidator.Deadline";
        private const string PassedKey = "ContentAuthoringBatchValidator.Passed";

        private const string StageIdle = "idle";
        private const string StageStarting = "starting";
        private const string StageWaiting = "waiting";
        private const string StageStopping = "stopping";

        static ContentAuthoringBatchValidator()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.update -= TickValidation;
            EditorApplication.update += TickValidation;
        }

        [MenuItem("Tools/Validate Content Authoring Scene")]
        public static void ValidateFromMenu()
        {
            StartValidation(false);
        }

        public static void ValidateBatch()
        {
            StartValidation(true);
        }

        private static void StartValidation(bool exitWhenFinished)
        {
            if (IsActive())
            {
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
            {
                Debug.LogError($"ContentAuthoringValidator: missing scene at {ScenePath}");
                if (exitWhenFinished)
                {
                    EditorApplication.Exit(1);
                }
                return;
            }

            SessionState.SetBool(ActiveKey, true);
            SessionState.SetBool(ExitWhenFinishedKey, exitWhenFinished);
            SessionState.SetBool(PassedKey, false);
            SetStage(StageStarting);

            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            EditorApplication.delayCall += EnterPlayMode;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!IsActive())
            {
                return;
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                SessionState.SetFloat(DeadlineKey, (float)(EditorApplication.timeSinceStartup + ValidationDelaySeconds));
                SetStage(StageWaiting);
                Debug.Log("ContentAuthoringValidator: entered play mode, waiting for runtime state.");
                return;
            }

            if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (GetStage() == StageStopping)
                {
                    bool passed = SessionState.GetBool(PassedKey, false);
                    bool exitWhenFinished = SessionState.GetBool(ExitWhenFinishedKey, false);
                    ClearState();

                    if (exitWhenFinished)
                    {
                        EditorApplication.Exit(passed ? 0 : 1);
                    }
                }
            }
        }

        private static void TickValidation()
        {
            if (!IsActive())
            {
                return;
            }

            string stage = GetStage();
            if (stage == StageStarting)
            {
                if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    EnterPlayMode();
                }
                return;
            }

            if (stage != StageWaiting || !EditorApplication.isPlaying)
            {
                return;
            }

            float deadline = SessionState.GetFloat(DeadlineKey, 0f);
            if (EditorApplication.timeSinceStartup < deadline)
            {
                return;
            }

            bool passed = ValidateRuntimeState();
            SessionState.SetBool(PassedKey, passed);
            SetStage(StageStopping);
            EditorApplication.isPlaying = false;
        }

        private static void EnterPlayMode()
        {
            if (!IsActive())
            {
                return;
            }

            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.Log("ContentAuthoringValidator: entering play mode.");
                EditorApplication.isPlaying = true;
            }
        }

        private static bool ValidateRuntimeState()
        {
            ScenarioManager scenarioManager = Object.FindFirstObjectByType<ScenarioManager>();
            DialogueRunner dialogueRunner = Object.FindFirstObjectByType<DialogueRunner>();
            ChatController chatController = Object.FindFirstObjectByType<ChatController>();
            GameObject overlayObject = GameObject.Find("ContentAuthoringDebugOverlay");
            TextMeshProUGUI overlayText = overlayObject != null ? overlayObject.GetComponentInChildren<TextMeshProUGUI>() : null;

            if (scenarioManager == null)
            {
                Debug.LogError("ContentAuthoringValidator: ScenarioManager not found.");
                return false;
            }

            if (dialogueRunner == null || !dialogueRunner.IsDialogueRunning)
            {
                Debug.LogError("ContentAuthoringValidator: dialogue did not start.");
                return false;
            }

            if (chatController == null)
            {
                Debug.LogError("ContentAuthoringValidator: ChatController not found.");
                return false;
            }

            ScrollRect scrollRect = chatController.GetComponent<ScrollRect>();
            if (scrollRect == null || scrollRect.content == null || scrollRect.content.childCount == 0)
            {
                Debug.LogError("ContentAuthoringValidator: chat content did not populate.");
                return false;
            }

            if (overlayText == null)
            {
                Debug.LogError("ContentAuthoringValidator: debug overlay not found.");
                return false;
            }

            string overlayValue = overlayText.text ?? string.Empty;
            if (!overlayValue.Contains("node:") || !overlayValue.Contains("line:") || !overlayValue.Contains("tag:"))
            {
                Debug.LogError($"ContentAuthoringValidator: debug overlay text is incomplete: {overlayValue}");
                return false;
            }

            Debug.Log($"ContentAuthoringValidator: PASS chatChildren={scrollRect.content.childCount} overlay=\"{overlayValue}\"");
            return true;
        }

        private static bool IsActive()
        {
            return SessionState.GetBool(ActiveKey, false);
        }

        private static string GetStage()
        {
            return SessionState.GetString(StageKey, StageIdle);
        }

        private static void SetStage(string stage)
        {
            SessionState.SetString(StageKey, stage);
        }

        private static void ClearState()
        {
            SessionState.EraseBool(ActiveKey);
            SessionState.EraseFloat(DeadlineKey);
            SessionState.EraseBool(PassedKey);
            SessionState.EraseString(StageKey);
            SessionState.EraseBool(ExitWhenFinishedKey);
        }
    }
}
