#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
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
                        ""start"":{""id"":""start"",""speaker"":""Narrator"",""text"":""Opening"",""choices"":[
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
            var expected = new ScreenFrameBuilder(model);
            NarrativeGenPreviewRequest.Queue(file);
            try
            {
                yield return PlayModeTestHelpers.LoadSceneWithTimeout("DebugChatScene",
                    PlayModeTestHelpers.SceneLoadTimeoutSeconds);
                var chat = Object.FindFirstObjectByType<ChatController>();
                Assert.IsNotNull(chat);

                int steps = 0;
                ScreenFrame previous = null;
                while (true)
                {
                    Assert.Less(steps, model.Nodes.Count, "The selected route did not reach an ending.");
                    var frame = expected.Current;
                    // Every line of the shared screen frame reaches the chat: speakers keep their
                    // own bubbles, the place, notes and ending become system lines.
                    foreach (var line in NarrativeGenChatScreen.Lines(frame, previous))
                    {
                        yield return WaitFor(
                            () => chat.GetChatHistory().Any(message => message.Text == line.Text
                                && (line.Kind == NarrativeGenChatScreen.LineKind.System
                                    || message.CharacterID == line.CharacterId)),
                            $"Chat did not show '{line.Text}' at node '{frame.NodeId}'.");
                    }

                    var choices = NarrativeGenChatScreen.Choices(frame);
                    if (frame.Ending || choices.Count == 0)
                    {
                        Assert.IsTrue(frame.Ending, $"Node '{frame.NodeId}' has no selectable choice.");
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
                    }, $"The choice for node '{frame.NodeId}' was not selectable.");
                    button.onClick.Invoke();
                    previous = frame;
                    expected.Choose(choices[pick].Id);
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
