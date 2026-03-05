using System;
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    public static class TestRunnerHelper
    {
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
                if (!test.HasChildren) Debug.Log($"TestRunnerHelper: Test Started - {test.Name}");
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (!result.Test.HasChildren) Debug.Log($"TestRunnerHelper: Test Finished - {result.Name} Result: {result.ResultState} ({result.Message})");
            }
        }
    }
}
