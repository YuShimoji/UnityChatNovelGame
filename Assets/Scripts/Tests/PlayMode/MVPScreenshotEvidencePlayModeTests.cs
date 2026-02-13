using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace ProjectFoundPhone.Tests
{
    public class MVPScreenshotEvidencePlayModeTests
    {
        private const string MvpSceneName = "MVPScene";
        private const string MvpScenePath = "Assets/Scenes/MVPScene.unity";
        private const string EvidenceRelativePath = "docs/evidence";
        private const float SceneLoadTimeoutSeconds = 10f;
        private const float StateTimeoutSeconds = 20f;
        private const float FileWriteTimeoutSeconds = 5f;

        [UnityTest]
        public IEnumerator MVPFlow_CapturesDistinctGameViewEvidence()
        {
            yield return LoadMvpScene();

            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string evidenceDir = Path.Combine(projectRoot, EvidenceRelativePath);
            Directory.CreateDirectory(evidenceDir);

            string titlePath = Path.Combine(evidenceDir, "01_Title_Screen.png");
            string chatPath = Path.Combine(evidenceDir, "02_Chat_Screen.png");
            string choicePath = Path.Combine(evidenceDir, "03_Choice_Screen.png");
            string endPath = Path.Combine(evidenceDir, "04_End_Screen.png");

            yield return CaptureAndWait(titlePath);

            Button startButton = FindButton("StartButton");
            Assert.IsNotNull(startButton, "StartButton was not found in MVPScene.");
            startButton.onClick.Invoke();

            yield return WaitUntil(
                () => IsActive("ChatPanel") && !IsActive("ChoicePanel") && ChatHasAnyBubble(),
                StateTimeoutSeconds,
                "Chat state was not reached."
            );
            yield return CaptureAndWait(chatPath);

            yield return WaitUntil(
                () => IsActive("ChoicePanel"),
                StateTimeoutSeconds,
                "Choice state was not reached."
            );
            yield return CaptureAndWait(choicePath);

            Button choiceAButton = FindButton("ChoiceA");
            Assert.IsNotNull(choiceAButton, "ChoiceA button was not found.");
            choiceAButton.onClick.Invoke();

            yield return WaitUntil(
                () => IsActive("EndPanel"),
                StateTimeoutSeconds,
                "End state was not reached."
            );
            yield return CaptureAndWait(endPath);

            string[] hashes =
            {
                ComputeSha256(titlePath),
                ComputeSha256(chatPath),
                ComputeSha256(choicePath),
                ComputeSha256(endPath),
            };

            Assert.AreEqual(
                hashes.Length,
                hashes.Distinct(StringComparer.Ordinal).Count(),
                "Evidence images are not distinct. Screenshots likely captured the wrong view/state."
            );
        }

        private static IEnumerator LoadMvpScene()
        {
#if UNITY_EDITOR
            if (!File.Exists(MvpScenePath))
            {
                Assert.Fail($"Scene path was not found: {MvpScenePath}");
            }

            AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(
                MvpScenePath,
                new LoadSceneParameters(LoadSceneMode.Single)
            );
            Assert.IsNotNull(operation, "LoadSceneAsyncInPlayMode returned null for MVPScene.");

            yield return WaitForOperation(operation, SceneLoadTimeoutSeconds, "MVPScene load timed out.");
#else
            AsyncOperation operation = SceneManager.LoadSceneAsync(MvpSceneName, LoadSceneMode.Single);
            Assert.IsNotNull(operation, "LoadSceneAsync returned null for MVPScene.");
            yield return WaitForOperation(operation, SceneLoadTimeoutSeconds, "MVPScene load timed out.");
#endif
        }

        private static IEnumerator CaptureAndWait(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(fullPath, 1);

            float start = Time.realtimeSinceStartup;
            while (!File.Exists(fullPath))
            {
                if (Time.realtimeSinceStartup - start > FileWriteTimeoutSeconds)
                {
                    Assert.Fail($"Screenshot write timed out: {fullPath}");
                }
                yield return null;
            }
        }

        private static IEnumerator WaitForOperation(AsyncOperation operation, float timeoutSeconds, string timeoutMessage)
        {
            float start = Time.realtimeSinceStartup;
            while (!operation.isDone)
            {
                if (Time.realtimeSinceStartup - start > timeoutSeconds)
                {
                    Assert.Fail(timeoutMessage);
                }
                yield return null;
            }
        }

        private static IEnumerator WaitUntil(Func<bool> predicate, float timeoutSeconds, string timeoutMessage)
        {
            float start = Time.realtimeSinceStartup;
            while (!predicate())
            {
                if (Time.realtimeSinceStartup - start > timeoutSeconds)
                {
                    Assert.Fail(timeoutMessage);
                }
                yield return null;
            }
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

        private static Button FindButton(string objectName)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
            {
                return null;
            }
            return obj.GetComponent<Button>();
        }

        private static string ComputeSha256(string fullPath)
        {
            using SHA256 sha = SHA256.Create();
            using FileStream stream = File.OpenRead(fullPath);
            byte[] hash = sha.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}
