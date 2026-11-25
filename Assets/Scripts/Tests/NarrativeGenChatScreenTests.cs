using System.Linq;
using NarrativeGen;
using NUnit.Framework;
using ProjectFoundPhone.Core;
using Kind = ProjectFoundPhone.Core.NarrativeGenChatScreen.LineKind;

namespace ProjectFoundPhone.Tests
{
    // The chat draws the NarrativeGen screen frame (SP-SCREEN-001): the same speaker,
    // place, topic / destination choices, journal notes and ending as the Studio window.
    public class NarrativeGenChatScreenTests
    {
        private const string InquiryModel = @"{
            ""modelType"":""adventure-playthrough"",
            ""startNode"":""desk"",
            ""variables"":{""表示形式"":""聞き込み"",""地域"":""灰渡りの港"",""依頼名"":""試験の依頼"",""依頼"":""札の持ち主を決める。""},
            ""nodes"":{
              ""desk"":{""id"":""desk"",""speaker"":""小屋｜ユノ"",""text"":""札が二枚ある。\n\n白糸の署名を見て。"",""choices"":[
                {""id"":""ask_thread"",""text"":""白糸の署名〔蓋然 40.0%〕"",""target"":""thread"",""effects"":[{""type"":""createEvent"",""id"":""heard"",""name"":""白糸の話を聞いた""}]},
                {""id"":""go_loom"",""text"":""織場"",""target"":""loom""}
              ]},
              ""thread"":{""id"":""thread"",""speaker"":""小屋｜ユノ"",""text"":""白糸は署名だよ。"",""choices"":[
                {""id"":""ask_desk"",""text"":""札の話"",""target"":""desk""},
                {""id"":""go_loom"",""text"":""織場"",""target"":""loom""}
              ]},
              ""loom"":{""id"":""loom"",""speaker"":""織場｜メイ"",""text"":""終わりだよ。"",""choices"":[]}
            }
          }";

        [Test]
        public void InquiryFramesBecomeSpeakerBubblesPlaceLinesAndGroupedChoices()
        {
            var screen = new ScreenFrameBuilder(Engine.LoadModel(InquiryModel));
            var first = screen.Current;
            var lines = NarrativeGenChatScreen.Lines(first, null);

            Assert.That(lines.Select(l => l.Kind), Is.EqualTo(new[] { Kind.System, Kind.System, Kind.Speaker, Kind.Speaker }));
            Assert.That(lines[0].Text, Is.EqualTo("灰渡りの港｜試験の依頼\n札の持ち主を決める。"));
            Assert.That(lines[1].Text, Is.EqualTo("― 小屋 ―"));
            Assert.That(lines.Skip(2).Select(l => (l.CharacterId, l.Text)),
                Is.EqualTo(new[] { ("ユノ", "札が二枚ある。"), ("ユノ", "白糸の署名を見て。") }));
            Assert.That(NarrativeGenChatScreen.Choices(first).Select(c => c.Label), Is.EqualTo(new[] { "白糸の署名", "→ 織場" }));

            var asked = screen.Choose("ask_thread");
            var askedLines = NarrativeGenChatScreen.Lines(asked, first);
            Assert.That(askedLines[0].Kind, Is.EqualTo(Kind.Player));
            Assert.That(askedLines[0].Text, Is.EqualTo("白糸の署名"));
            Assert.That(askedLines.Any(l => l.Text.StartsWith("― ")), Is.False, "The place did not change.");
            Assert.That(askedLines.Last().Text, Is.EqualTo("旅の手帳に記した — ユノ：白糸の話を聞いた"));
            Assert.That(NarrativeGenChatScreen.Choices(asked).Select(c => c.Label), Is.EqualTo(new[] { "札の話（済）", "→ 織場" }));

            var end = screen.Choose("go_loom");
            var endLines = NarrativeGenChatScreen.Lines(end, asked).Select(l => l.Text).ToList();
            Assert.That(endLines, Is.EqualTo(new[]
            {
                "織場", "― 織場 ―", "終わりだよ。", "依頼完了", "旅の手帳\n・ユノ：白糸の話を聞いた", NarrativeGenChatScreen.EndMessage,
            }));
            Assert.That(NarrativeGenChatScreen.Choices(end), Is.Empty);
        }

        [Test]
        public void StandardFramesKeepTheWholeSpeakerLabelAndOneChoiceList()
        {
            var screen = new ScreenFrameBuilder(Engine.LoadModel(@"{
                ""modelType"":""adventure-playthrough"",""startNode"":""a"",
                ""nodes"":{""a"":{""id"":""a"",""speaker"":""Narrator"",""text"":""Opening"",""choices"":[{""id"":""go_end"",""text"":""Finish"",""target"":""b""}]},
                           ""b"":{""id"":""b"",""text"":""Ending""}}}"));
            var lines = NarrativeGenChatScreen.Lines(screen.Current, null);
            Assert.That(lines.Select(l => (l.Kind, l.CharacterId, l.Text)), Is.EqualTo(new[] { (Kind.Speaker, "Narrator", "Opening") }));
            Assert.That(NarrativeGenChatScreen.Choices(screen.Current).Select(c => c.Label), Is.EqualTo(new[] { "Finish" }));
            var opening = screen.Current;
            var end = NarrativeGenChatScreen.Lines(screen.Choose("go_end"), opening);
            Assert.That(end.Select(l => l.Text), Is.EqualTo(new[] { "Finish", "Ending", NarrativeGenChatScreen.EndMessage }));
        }
    }
}
