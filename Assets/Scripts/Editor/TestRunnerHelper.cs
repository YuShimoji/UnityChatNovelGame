using System;
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    public static class TestRunnerHelper
    {
        private const string FilterArgPrefix = "-ProjectFoundPhoneTestFilter=";
        private const string ResultFileArgPrefix = "-ProjectFoundPhoneResultFile=";

        [MenuItem("Tools/Run PlayMode Tests Manual")]
        public static void RunPlayModeTests()
        {
            Debug.Log("TestRunnerHelper: Starting PlayMode tests...");
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            var filter = new Filter
            {
                testMode = TestMode.PlayMode
            };

            testRunnerApi.RegisterCallbacks(new TestCallbacks());
            testRunnerApi.Execute(new ExecutionSettings(filter));
        }

        public static void RunPlayModeTestsBatch()
        {
            string filterName = ReadCommandLineValue(FilterArgPrefix);
            string resultFile = ReadCommandLineValue(ResultFileArgPrefix);

            if (string.IsNullOrWhiteSpace(resultFile))
            {
                string projectRoot = Directory.GetParent(Application.dataPath).FullName;
                resultFile = Path.Combine(projectRoot, "docs", "verification", "playmode-batch-result.txt");
            }

            Debug.Log($"TestRunnerHelper: Starting batch PlayMode tests. Filter='{filterName ?? "(none)"}' ResultFile='{resultFile}'");

            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            var filter = new Filter
            {
                testMode = TestMode.PlayMode,
                testNames = string.IsNullOrWhiteSpace(filterName) ? null : new[] { filterName }
            };

            testRunnerApi.RegisterCallbacks(new BatchTestCallbacks(resultFile));
            EditorApplication.delayCall += () => testRunnerApi.Execute(new ExecutionSettings(filter));
        }

        private static string ReadCommandLineValue(string prefix)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(prefix, StringComparison.Ordinal))
                {
                    return args[i].Substring(prefix.Length);
                }
            }

            return null;
        }

        private class TestCallbacks : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
                Debug.Log($"TestRunnerHelper: Run Started. Found {testsToRun.TestCaseCount} tests to run.");
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                Debug.Log($"TestRunnerHelper: Run Finished. Status: {result.ResultState}, Failed: {result.FailCount}, Passed: {result.PassCount}");

                string projectRoot = Directory.GetParent(Application.dataPath).FullName;
                string resultDir = Path.Combine(projectRoot, "docs/evidence/TASK_047");
                Directory.CreateDirectory(resultDir);

                string statusFile = Path.Combine(resultDir, "ManualPlayModeTestStatus.txt");
                File.WriteAllText(statusFile, $"Status: {result.ResultState}\nPassed: {result.PassCount}\nFailed: {result.FailCount}\nTime: {DateTime.Now:O}\n");

                Debug.Log($"TestRunnerHelper: Status written to {statusFile}");
                EditorApplication.Exit(result.ResultState == "Passed" ? 0 : 1);
            }

            public void TestStarted(ITestAdaptor test)
            {
                if (!test.HasChildren)
                {
                    Debug.Log($"TestRunnerHelper: Test Started - {test.Name}");
                }
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (!result.Test.HasChildren)
                {
                    Debug.Log($"TestRunnerHelper: Test Finished - {result.Name} Result: {result.ResultState} ({result.Message})");
                }
            }
        }

        private sealed class BatchTestCallbacks : ICallbacks
        {
            private readonly string m_ResultFilePath;

            public BatchTestCallbacks(string resultFilePath)
            {
                m_ResultFilePath = resultFilePath;
            }

            public void RunStarted(ITestAdaptor testsToRun)
            {
                Debug.Log($"TestRunnerHelper(batch): Run Started. Found {testsToRun.TestCaseCount} tests.");
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                string directory = Path.GetDirectoryName(m_ResultFilePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string output =
                    $"ResultState: {result.ResultState}{Environment.NewLine}" +
                    $"Passed: {result.PassCount}{Environment.NewLine}" +
                    $"Failed: {result.FailCount}{Environment.NewLine}" +
                    $"Skipped: {result.SkipCount}{Environment.NewLine}" +
                    $"Time: {DateTime.Now:O}{Environment.NewLine}";

                File.WriteAllText(m_ResultFilePath, output);
                Debug.Log($"TestRunnerHelper(batch): Result written to {m_ResultFilePath}");
                EditorApplication.Exit(result.FailCount == 0 ? 0 : 1);
            }

            public void TestStarted(ITestAdaptor test)
            {
                if (!test.HasChildren)
                {
                    Debug.Log($"TestRunnerHelper(batch): Test Started - {test.Name}");
                }
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (!result.Test.HasChildren)
                {
                    Debug.Log($"TestRunnerHelper(batch): Test Finished - {result.Name} Result: {result.ResultState} ({result.Message})");
                }
            }
        }
    }
}
