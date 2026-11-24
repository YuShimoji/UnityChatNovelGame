#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using NarrativeGen;
using NUnit.Framework;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ProjectFoundPhone.Tests
{
    public class NarrativeGenPreviewPlayModeTests
    {
        [UnityTest]
        public IEnumerator PlaysJsonInTheFoundPhoneChatToAnEnding()
        {
            using var saveScope = new SaveStorageIsolation();
            string file = Environment.GetEnvironmentVariable("FOUNDPHONE_NARRATIVE_TEST_JSON");
            if (string.IsNullOrWhiteSpace(file))
            {
                file = Path.Combine(SaveStorageIsolation.DirectoryOverride, "preview.json");
                File.WriteAllText(file, @"{
                    ""startNode"":""start"",
                    ""nodes"":{
                        ""start"":{""id"":""start"",""text"":""Opening"",""choices"":[
                            {""id"":""short"",""text"":""Finish"",""target"":""end""},
                            {""id"":""look"",""text"":""Look"",""target"":""observe"",""effects"":[
                                {""type"":""createEvent"",""id"":""observation"",""name"":""Observed evidence""}
                            ]}
                        ]},
                        ""observe"":{""id"":""observe"",""text"":""[observation]"",""choices"":[
                            {""id"":""continue"",""text"":""Continue"",""target"":""end"",""conditions"":[
                                {""type"":""hasEvent"",""key"":""observation"",""value"":true}
                            ]}
                        ]},
                        ""end"":{""id"":""end"",""text"":""Ending""}
                    }
                }");
            }

            var model = Engine.LoadModel(File.ReadAllText(file));
            var expected = Engine.StartSession(model);
            NarrativeGenPreviewRequest.Queue(file);
            try
            {
                yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene",
                    PlayModeTestHelpers.SceneLoadTimeoutSeconds);
                var chat = Object.FindFirstObjectByType<ChatController>();
                Assert.IsNotNull(chat);

                int steps = 0;
                while (true)
                {
                    Assert.Less(steps, model.Nodes.Count, "The selected route did not reach an ending.");
                    var choices = Engine.GetAvailableChoices(expected, model);
                    string body = NarrativeDisplayText.ResolveNarrativeDisplayTextTracked(
                        model.Nodes[expected.CurrentNodeId].Text, model, expected).Text;
                    if (!string.IsNullOrEmpty(body))
                    {
                        yield return WaitFor(
                            () => chat.GetChatHistory().Any(message => message.Text == body),
                            $"Chat did not show node '{expected.CurrentNodeId}'.");
                    }

                    if (choices.Count == 0)
                    {
                        yield return WaitFor(() => chat.GetChatHistory().Any(message => message.Text == "再生終了"),
                            "The final chat message was not displayed.");
                        break;
                    }

                    // Exercise the second initial choice and subsequent choice prompts after their fade.
                    int pick = steps == 0 && choices.Count > 1 ? 1 : 0;
                    string buttonName = "Choice" + (char)('A' + pick);
                    Button button = null;
                    yield return WaitFor(() =>
                    {
                        button = Object.FindObjectsByType<Button>(FindObjectsSortMode.None).FirstOrDefault(
                            candidate => candidate.name == buttonName && candidate.interactable);
                        return button != null;
                    }, $"The choice for node '{expected.CurrentNodeId}' was not selectable.");
                    button.onClick.Invoke();
                    expected = Engine.ApplyChoice(expected, model, choices[pick].Id);
                    steps++;
                }

                Assert.GreaterOrEqual(steps, 2);
                Assert.That(Directory.GetFiles(SaveStorageIsolation.DirectoryOverride, "SaveData_*"), Is.Empty);
            }
            finally
            {
                NarrativeGenPreviewRequest.Clear();
            }
        }

        private static IEnumerator WaitFor(Func<bool> condition, string failure)
        {
            float end = Time.realtimeSinceStartup + 10f;
            while (!condition() && Time.realtimeSinceStartup < end)
            {
                yield return null;
            }
            Assert.IsTrue(condition(), failure);
        }
    }
}
#endif
