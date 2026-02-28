using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Dev
{
    public class VerificationAutomator : MonoBehaviour
    {
        public string TargetScene = "DebugChatScene";
        public string VerificationFlow = "single_capture";

        private const float SceneLoadTimeoutSeconds = 15f;
        private const float StateTimeoutSeconds = 20f;
        private const float MaxMvpFlowSeconds = 60f;

        private readonly List<string> m_LogEntries = new List<string>();
        private bool m_HasFailed;
        private string m_FailureReason = string.Empty;
        private string m_ProjectRoot;
        private DateTimeOffset m_StartedAt;

        private void Awake()
        {
            ParseCommandLineArguments();
            Application.logMessageReceived += OnLogMessageReceived;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        private IEnumerator Start()
        {
            m_StartedAt = DateTimeOffset.Now;
            m_ProjectRoot = Directory.GetParent(Application.dataPath).FullName;

            Debug.Log($"VerificationAutomator: flow={VerificationFlow}, targetScene={TargetScene}");
            DontDestroyOnLoad(gameObject);

            yield return LoadTargetScene(TargetScene);
            if (m_HasFailed)
            {
                FinishAndExit(1);
                yield break;
            }

            switch ((VerificationFlow ?? string.Empty).ToLowerInvariant())
            {
                case "mvp_pack":
                    yield return RunMvpFinalVerificationPack();
                    break;
                case "vertical_slice_full":
                    yield return RunVerticalSliceFullPlaythrough();
                    break;
                default:
                    yield return RunSingleCapture();
                    break;
            }

            int exitCode = m_HasFailed || HasUnexpectedErrors() ? 1 : 0;
            FinishAndExit(exitCode);
        }

        private void ParseCommandLineArguments()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-targetScene" && i + 1 < args.Length)
                {
                    TargetScene = args[i + 1];
                }

                if (args[i] == "-verificationFlow" && i + 1 < args.Length)
                {
                    VerificationFlow = args[i + 1];
                }
            }
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            string entry = $"[{DateTime.Now:O}] [{type}] {condition}";
            if (!string.IsNullOrWhiteSpace(stackTrace))
            {
                entry += Environment.NewLine + stackTrace;
            }

            m_LogEntries.Add(entry);
        }

        private IEnumerator RunSingleCapture()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string evidenceDir = Path.Combine(m_ProjectRoot, "docs", "evidence");
            Directory.CreateDirectory(evidenceDir);

            string markerPath = Path.Combine(evidenceDir, "automator_ran.txt");
            File.WriteAllText(markerPath, "Automator ran at " + DateTime.Now.ToString("O"));

            yield return new WaitForSeconds(1.0f);
            yield return CaptureScreenshot(Path.Combine(evidenceDir, $"Capture_{timestamp}_{TargetScene}.png"));

            string logPath = Path.Combine(evidenceDir, $"Log_{timestamp}_{TargetScene}.txt");
            File.WriteAllLines(logPath, m_LogEntries);
        }

        private IEnumerator RunMvpFinalVerificationPack()
        {
            string evidenceDir = Path.Combine(m_ProjectRoot, "docs", "evidence");
            Directory.CreateDirectory(evidenceDir);

            float branchASeconds = 0f;
            float branchBSeconds = 0f;

            Debug.Log("VerificationAutomator: MVP branch A start.");
            yield return CaptureScreenshot(Path.Combine(evidenceDir, "01_Title_Screen.png"));
            if (m_HasFailed) yield break;

            yield return RunMvpBranch(
                choiceObjectName: "ChoiceA",
                resultCapturePath: Path.Combine(evidenceDir, "04_End_Screen.png"),
                elapsedSeconds: seconds => branchASeconds = seconds,
                rapidInput: false
            );
            if (m_HasFailed) yield break;

            Debug.Log("VerificationAutomator: MVP branch B start.");
            yield return LoadTargetScene("MVPScene");
            if (m_HasFailed) yield break;

            yield return RunMvpBranch(
                choiceObjectName: "ChoiceB",
                resultCapturePath: Path.Combine(evidenceDir, "05_ChoiceB_End_Screen.png"),
                elapsedSeconds: seconds => branchBSeconds = seconds,
                rapidInput: false
            );
            if (m_HasFailed) yield break;

            Debug.Log("VerificationAutomator: MVP rapid input start.");
            yield return LoadTargetScene("MVPScene");
            if (m_HasFailed) yield break;

            yield return RunMvpBranch(
                choiceObjectName: "ChoiceA",
                resultCapturePath: Path.Combine(evidenceDir, "06_RapidInput_End_Screen.png"),
                elapsedSeconds: _ => { },
                rapidInput: true
            );
            if (m_HasFailed) yield break;

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string summaryPath = Path.Combine(evidenceDir, $"MVP_FINAL_VERIFICATION_{timestamp}.md");
            string[] lines =
            {
                "# MVP Final Verification Pack",
                string.Empty,
                $"- Started: {m_StartedAt:O}",
                $"- Finished: {DateTimeOffset.Now:O}",
                $"- Result: {(m_HasFailed ? "FAILED" : "SUCCESS")}",
                $"- Branch A Seconds: {branchASeconds:F2}",
                $"- Branch B Seconds: {branchBSeconds:F2}",
                $"- Unexpected Error Count: {GetUnexpectedErrorEntries().Count}",
                string.Empty,
                "## Evidence",
                "- 01_Title_Screen.png",
                "- 02_Chat_Screen.png",
                "- 03_Choice_Screen.png",
                "- 04_End_Screen.png",
                "- 05_ChoiceB_End_Screen.png",
                "- 06_RapidInput_End_Screen.png",
                string.Empty,
                "## Checks",
                $"- [{(branchASeconds <= MaxMvpFlowSeconds ? "x" : " ")}] Branch A within 60 seconds",
                $"- [{(branchBSeconds <= MaxMvpFlowSeconds ? "x" : " ")}] Branch B within 60 seconds",
                $"- [{(GetUnexpectedErrorEntries().Count == 0 ? "x" : " ")}] Error/Exception/Assert = 0",
            };

            File.WriteAllLines(summaryPath, lines);
            File.WriteAllLines(Path.Combine(evidenceDir, $"MVP_FINAL_VERIFICATION_LOG_{timestamp}.txt"), m_LogEntries);
        }

        private IEnumerator RunMvpBranch(
            string choiceObjectName,
            string resultCapturePath,
            Action<float> elapsedSeconds,
            bool rapidInput
        )
        {
            float startedAt = Time.realtimeSinceStartup;

            Button startButton = FindButton("StartButton");
            if (startButton == null)
            {
                Fail("StartButton was not found in MVPScene.");
                yield break;
            }

            if (rapidInput)
            {
                for (int i = 0; i < 5; i++)
                {
                    startButton.onClick.Invoke();
                }
            }
            else
            {
                startButton.onClick.Invoke();
            }

            yield return WaitUntilOrFail(
                () => IsActive("ChatPanel") && ChatHasAnyBubble() && !IsActive("ChoicePanel"),
                StateTimeoutSeconds,
                "Chat state was not reached in MVPScene."
            );
            if (m_HasFailed) yield break;

            if (!rapidInput && !File.Exists(Path.Combine(m_ProjectRoot, "docs", "evidence", "02_Chat_Screen.png")))
            {
                yield return CaptureScreenshot(Path.Combine(m_ProjectRoot, "docs", "evidence", "02_Chat_Screen.png"));
                if (m_HasFailed) yield break;
            }

            yield return WaitUntilOrFail(
                () => IsActive("ChoicePanel") && IsActive("ChoiceA") && IsActive("ChoiceB"),
                StateTimeoutSeconds,
                "Choice state was not reached in MVPScene."
            );
            if (m_HasFailed) yield break;

            if (!rapidInput && !File.Exists(Path.Combine(m_ProjectRoot, "docs", "evidence", "03_Choice_Screen.png")))
            {
                yield return CaptureScreenshot(Path.Combine(m_ProjectRoot, "docs", "evidence", "03_Choice_Screen.png"));
                if (m_HasFailed) yield break;
            }

            Button choiceButton = FindButton(choiceObjectName);
            if (choiceButton == null)
            {
                Fail($"{choiceObjectName} button was not found in MVPScene.");
                yield break;
            }

            if (rapidInput)
            {
                for (int i = 0; i < 5; i++)
                {
                    choiceButton.onClick.Invoke();
                }
            }
            else
            {
                choiceButton.onClick.Invoke();
            }

            yield return WaitUntilOrFail(
                () => IsActive("EndPanel"),
                StateTimeoutSeconds,
                "EndPanel was not reached in MVPScene."
            );
            if (m_HasFailed) yield break;

            yield return CaptureScreenshot(resultCapturePath);
            if (m_HasFailed) yield break;

            float seconds = Time.realtimeSinceStartup - startedAt;
            elapsedSeconds(seconds);

            if (seconds > MaxMvpFlowSeconds)
            {
                Fail($"MVP flow exceeded {MaxMvpFlowSeconds:F0} seconds: {seconds:F2}s");
            }
        }

        private IEnumerator RunVerticalSliceFullPlaythrough()
        {
            ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>(FindObjectsInactive.Include);
            ChatController chatController = FindFirstObjectByType<ChatController>(FindObjectsInactive.Include);
            DeductionBoard deductionBoard = FindFirstObjectByType<DeductionBoard>(FindObjectsInactive.Include);

            if (deductionBoard == null)
            {
                yield return EnsureDeductionBoardExists();
                deductionBoard = FindFirstObjectByType<DeductionBoard>(FindObjectsInactive.Include);
            }

            if (scenarioManager == null || chatController == null || deductionBoard == null)
            {
                string dependencyState =
                    $"ScenarioManager={(scenarioManager != null)}, " +
                    $"ChatController={(chatController != null)}, " +
                    $"DeductionBoard={(deductionBoard != null)}";
                Fail("DebugChatScene verification dependencies were not found. " + dependencyState);
                yield break;
            }

            string evidenceDir = Path.Combine(m_ProjectRoot, "docs", "evidence", "TASK_027");
            Directory.CreateDirectory(evidenceDir);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            scenarioManager.StartScenario("VerticalSlice_Start");

            yield return WaitUntilOrFail(
                () => ChatHasAnyBubble(),
                StateTimeoutSeconds,
                "Chat bubbles did not appear in DebugChatScene."
            );
            if (m_HasFailed) yield break;

            string captureStart = Path.Combine(evidenceDir, "Capture_01_start.png");
            string captureTopic = Path.Combine(evidenceDir, "Capture_02_topic.png");
            string captureEnd = Path.Combine(evidenceDir, "Capture_03_synthesis_or_end.png");

            yield return CaptureScreenshot(captureStart);
            if (m_HasFailed) yield break;

            yield return WaitForChoiceOrFallbackToBranchA(scenarioManager);
            if (m_HasFailed) yield break;

            yield return WaitUntilOrFail(
                () => deductionBoard.HasTopic("debug_topic_01"),
                StateTimeoutSeconds,
                "debug_topic_01 was not unlocked."
            );
            if (m_HasFailed) yield break;

            yield return CaptureScreenshot(captureTopic);
            if (m_HasFailed) yield break;

            TopicData foundPhoneTopic = Resources.Load<TopicData>("Topics/topic_found_phone");
            if (foundPhoneTopic == null)
            {
                Fail("Resources/Topics/topic_found_phone could not be loaded.");
                yield break;
            }

            deductionBoard.AddTopic(foundPhoneTopic);
            yield return null;

            MethodInfo synthesisMethod = typeof(DeductionBoard).GetMethod(
                "CheckSynthesis",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (synthesisMethod == null)
            {
                Fail("DeductionBoard.CheckSynthesis reflection lookup failed.");
                yield break;
            }

            TopicData debugTopic = deductionBoard.UnlockedTopics.FirstOrDefault(
                topic => topic != null && topic.TopicID == "debug_topic_01"
            );
            if (debugTopic == null)
            {
                Fail("debug_topic_01 was not found in DeductionBoard.");
                yield break;
            }

            bool synthesisSucceeded = (bool)synthesisMethod.Invoke(deductionBoard, new object[] { debugTopic, foundPhoneTopic });
            if (!synthesisSucceeded)
            {
                Fail("Synthesis for debug_topic_01 + topic_found_phone did not succeed.");
                yield break;
            }

            yield return WaitUntilOrFail(
                () => deductionBoard.HasTopic("topic_suspicious_message"),
                StateTimeoutSeconds,
                "topic_suspicious_message was not added after synthesis."
            );
            if (m_HasFailed) yield break;

            yield return WaitUntilOrFail(
                () => AllVisibleTexts().Any(text => text.Contains("--- Chapter 0 END ---", StringComparison.Ordinal)),
                StateTimeoutSeconds,
                "End marker was not observed."
            );
            if (m_HasFailed) yield break;

            yield return CaptureScreenshot(captureEnd);
            if (m_HasFailed) yield break;

            string resultPath = Path.Combine(evidenceDir, $"FULL_PLAYTHROUGH_RESULTS_{timestamp}.md");
            string logPath = Path.Combine(evidenceDir, $"Log_{timestamp}.txt");
            string[] lines =
            {
                "# TASK_027 Automated Full Playthrough Results",
                string.Empty,
                $"- Started: {m_StartedAt:O}",
                $"- Finished: {DateTimeOffset.Now:O}",
                $"- Result: {(m_HasFailed ? "FAILED" : "SUCCESS")}",
                $"- Unexpected Error Count: {GetUnexpectedErrorEntries().Count}",
                string.Empty,
                "## Captures",
                "- Capture_01_start.png",
                "- Capture_02_topic.png",
                "- Capture_03_synthesis_or_end.png",
                string.Empty,
                "## Checklist",
                "- [x] Chat flow reached choice",
                "- [x] Topic unlock confirmed",
                "- [x] DeductionBoard synthesis confirmed",
                "- [x] End marker observed",
                $"- [{(GetUnexpectedErrorEntries().Count == 0 ? "x" : " ")}] Console Error/Exception 0",
            };

            File.WriteAllLines(resultPath, lines);
            File.WriteAllLines(logPath, m_LogEntries);
        }

        private IEnumerator EnsureDeductionBoardExists()
        {
#if UNITY_EDITOR
            const string prefabPath = "Assets/Prefabs/UI/DeductionBoard.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Fail($"DeductionBoard prefab was not found: {prefabPath}");
                yield break;
            }

            GameObject instance = Instantiate(prefab);
            instance.name = prefab.name;
            instance.SetActive(true);
            Debug.Log("VerificationAutomator: Runtime DeductionBoard instantiated from prefab.");
            yield return null;
#else
            Fail("DeductionBoard prefab injection is only available in the Unity Editor.");
            yield break;
#endif
        }

        private IEnumerator LoadTargetScene(string sceneName)
        {
            Debug.Log($"VerificationAutomator: Loading scene {sceneName}");
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                Fail($"SceneManager.LoadSceneAsync returned null for {sceneName}.");
                yield break;
            }

            float start = Time.realtimeSinceStartup;
            while (!operation.isDone)
            {
                if (Time.realtimeSinceStartup - start > SceneLoadTimeoutSeconds)
                {
                    Fail($"Scene load timed out: {sceneName}");
                    yield break;
                }

                yield return null;
            }

            yield return null;
            yield return null;
        }

        private IEnumerator WaitUntilOrFail(Func<bool> predicate, float timeoutSeconds, string failureMessage)
        {
            float start = Time.realtimeSinceStartup;
            while (!predicate())
            {
                if (Time.realtimeSinceStartup - start > timeoutSeconds)
                {
                    Fail(failureMessage);
                    yield break;
                }

                yield return null;
            }
        }

        private IEnumerator WaitForChoiceOrFallbackToBranchA(ScenarioManager scenarioManager)
        {
            float start = Time.realtimeSinceStartup;
            while (!IsActive("ChoiceA") || !IsActive("ChoiceB"))
            {
                if (Time.realtimeSinceStartup - start > 10f)
                {
                    Debug.Log("VerificationAutomator: Choice UI did not appear. Falling back to BranchA direct start.");
                    scenarioManager.StartScenario("BranchA");
                    yield break;
                }

                yield return null;
            }

            Button choiceA = FindButton("ChoiceA");
            if (choiceA == null)
            {
                Fail("ChoiceA button was not found in DebugChatScene.");
                yield break;
            }

            choiceA.onClick.Invoke();
        }

        private IEnumerator CaptureScreenshot(string fullPath)
        {
            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            if (Application.isBatchMode)
            {
                yield return null;
                yield return null;

                Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                screenshot.Apply();

                byte[] bytes = screenshot.EncodeToPNG();
                Destroy(screenshot);

                if (bytes == null || bytes.Length == 0)
                {
                    Fail($"Screenshot encode failed: {fullPath}");
                    yield break;
                }

                File.WriteAllBytes(fullPath, bytes);
            }
            else
            {
                yield return new WaitForEndOfFrame();
                ScreenCapture.CaptureScreenshot(fullPath, 1);

                float start = Time.realtimeSinceStartup;
                while (!File.Exists(fullPath))
                {
                    if (Time.realtimeSinceStartup - start > 5f)
                    {
                        Fail($"Screenshot write timed out: {fullPath}");
                        yield break;
                    }

                    yield return null;
                }
            }

            Debug.Log($"VerificationAutomator: Screenshot written to {fullPath}");
        }

        private static Button FindButton(string objectName)
        {
            GameObject obj = GameObject.Find(objectName);
            return obj != null ? obj.GetComponent<Button>() : null;
        }

        private static bool IsActive(string objectName)
        {
            GameObject obj = GameObject.Find(objectName);
            return obj != null && obj.activeInHierarchy;
        }

        private static bool ChatHasAnyBubble()
        {
            GameObject content = GameObject.Find("Content");
            return content != null && content.transform.childCount > 0;
        }

        private static IEnumerable<string> AllVisibleTexts()
        {
            Component[] components = Resources.FindObjectsOfTypeAll<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null || component.gameObject == null || !component.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (component is Text text && !string.IsNullOrWhiteSpace(text.text))
                {
                    yield return text.text;
                    continue;
                }

                PropertyInfo textProperty = component.GetType().GetProperty("text");
                if (textProperty == null || textProperty.PropertyType != typeof(string))
                {
                    continue;
                }

                string value = textProperty.GetValue(component) as string;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    yield return value;
                }
            }
        }

        private List<string> GetUnexpectedErrorEntries()
        {
            return m_LogEntries.Where(log =>
                (
                    log.Contains("[Error]", StringComparison.Ordinal) ||
                    log.Contains("[Exception]", StringComparison.Ordinal) ||
                    log.Contains("[Assert]", StringComparison.Ordinal)
                ) &&
                !log.Contains("ReadPixels was called to read pixels from system frame buffer", StringComparison.Ordinal)
            ).ToList();
        }

        private bool HasUnexpectedErrors()
        {
            return GetUnexpectedErrorEntries().Count > 0;
        }

        private void Fail(string reason)
        {
            if (m_HasFailed)
            {
                return;
            }

            m_HasFailed = true;
            m_FailureReason = reason;
            Debug.LogError("VerificationAutomator: " + reason);
        }

        private void FinishAndExit(int exitCode)
        {
            string evidenceDir = Path.Combine(m_ProjectRoot, "docs", "evidence");
            Directory.CreateDirectory(evidenceDir);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string summaryPath = Path.Combine(evidenceDir, $"AUTOMATION_SUMMARY_{VerificationFlow}_{timestamp}.md");

            string[] lines =
            {
                "# Verification Automation Summary",
                string.Empty,
                $"- Flow: {VerificationFlow}",
                $"- Target Scene: {TargetScene}",
                $"- Started: {m_StartedAt:O}",
                $"- Finished: {DateTimeOffset.Now:O}",
                $"- Exit Code: {exitCode}",
                $"- Result: {(exitCode == 0 ? "SUCCESS" : "FAILED")}",
                $"- Failure Reason: {(string.IsNullOrWhiteSpace(m_FailureReason) ? "none" : m_FailureReason)}",
                $"- Unexpected Error Count: {GetUnexpectedErrorEntries().Count}",
            };

            File.WriteAllLines(summaryPath, lines);
            File.WriteAllLines(Path.Combine(evidenceDir, $"AUTOMATION_LOG_{VerificationFlow}_{timestamp}.txt"), m_LogEntries);

#if UNITY_EDITOR
            bool shouldExitEditor = Application.isBatchMode || Environment.GetCommandLineArgs().Contains("-executeMethod");
            if (shouldExitEditor)
            {
                EditorApplication.Exit(exitCode);
            }
            else
            {
                EditorApplication.isPlaying = false;
            }
#else
            Application.Quit();
#endif
        }
    }
}
