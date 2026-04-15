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

        // --- Legacy interactive menu items (superseded by MCP batch tools) ---
        // Kept for manual fallback in Unity Editor.
        // Primary execution path: RunEditModeTestsBatch / RunPlayModeTestsBatch (batchmode).

        [MenuItem("Tools/FoundPhone/Tests/Run PlayMode Tests")]
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

        [MenuItem("Tools/FoundPhone/Tests/Run EditMode Tests")]
        public static void RunEditModeTests()
        {
            Debug.Log("TestRunnerHelper: Starting EditMode tests...");
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            var filter = new Filter
            {
                testMode = TestMode.EditMode
            };

            testRunnerApi.RegisterCallbacks(new TestCallbacks());
            testRunnerApi.Execute(new ExecutionSettings(filter));
        }

        /// <summary>
        /// batchmode 用: EditMode 全件（または -ProjectFoundPhoneTestFilter= で 1 本指定）。
        /// 結果は -ProjectFoundPhoneResultFile= または docs/verification/editmode-batch-result.txt。
        /// </summary>
        public static void RunEditModeTestsBatch()
        {
            EnqueueBatchTestRun(
                TestMode.EditMode,
                "editmode-batch-result.txt",
                "EditMode");
        }

        public static void RunPlayModeTestsBatch()
        {
            EnqueueBatchTestRun(
                TestMode.PlayMode,
                "playmode-batch-result.txt",
                "PlayMode");
        }

        private static void EnqueueBatchTestRun(TestMode testMode, string defaultResultFileName, string labelForLog)
        {
            string filterName = ReadCommandLineValue(FilterArgPrefix);
            string resultFile = ReadCommandLineValue(ResultFileArgPrefix);

            if (string.IsNullOrWhiteSpace(resultFile))
            {
                string projectRoot = Directory.GetParent(Application.dataPath).FullName;
                resultFile = Path.Combine(projectRoot, "docs", "verification", defaultResultFileName);
            }

            Debug.Log($"TestRunnerHelper: Starting batch {labelForLog} tests. Filter='{filterName ?? "(none)"}' ResultFile='{resultFile}'");

            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            var filter = new Filter
            {
                testMode = testMode,
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

                // plain text サマリ
                string output =
                    $"ResultState: {result.ResultState}{Environment.NewLine}" +
                    $"Passed: {result.PassCount}{Environment.NewLine}" +
                    $"Failed: {result.FailCount}{Environment.NewLine}" +
                    $"Skipped: {result.SkipCount}{Environment.NewLine}" +
                    $"Time: {DateTime.Now:O}{Environment.NewLine}";

                File.WriteAllText(m_ResultFilePath, output);
                Debug.Log($"TestRunnerHelper(batch): Result written to {m_ResultFilePath}");

                // NUnit XML 出力 (.txt -> .xml に拡張子変更)
                try
                {
                    string xmlPath = Path.ChangeExtension(m_ResultFilePath, ".xml");
                    // ITestResultAdaptor.ToXml() は NUnit.Framework.Interfaces.TNode を返す
                    var tNode = result.ToXml();
                    File.WriteAllText(xmlPath, tNode.OuterXml);
                    Debug.Log($"TestRunnerHelper(batch): XML result written to {xmlPath}");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"TestRunnerHelper(batch): Failed to write XML result: {ex.Message}");
                }

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
