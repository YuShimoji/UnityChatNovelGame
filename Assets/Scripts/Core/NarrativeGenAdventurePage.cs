using System.Linq;
using NarrativeGen;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Core
{
    // The SDK owns the game state. A disclosure has no choice ID and cannot advance it.
    public static class NarrativeGenAdventurePage
    {
        internal static string ChoiceLabel(string role, ScreenChoice choice)
        {
            if (role == ScreenFrame.RoleTravel) return "→ " + choice.Text;
            if (role == ScreenFrame.RoleTopic && choice.Visited) return choice.Text + "（済）";
            return choice.Text;
        }

        public static AdventurePage FromFrame(ScreenFrame frame)
        {
            bool inquiry = frame.Layout == ScreenFrame.LayoutInquiry;
            var scene = AdventureBlock.Group(inquiry ? "inquiry" : "scene", frame.Speaker);
            var prose = AdventureBlock.Group("quote");
            if (!string.IsNullOrEmpty(frame.Place)) prose.children.Add(AdventureBlock.Text("label", frame.Place));
            foreach (var paragraph in frame.Paragraphs)
            {
                if (paragraph.All(span => string.IsNullOrEmpty(span.ChoiceId)))
                {
                    prose.children.Add(AdventureBlock.Text("text", string.Concat(paragraph.Select(span => span.Text))));
                    continue;
                }
                var line = AdventureBlock.Group("inline");
                foreach (var span in paragraph)
                    line.children.Add(string.IsNullOrEmpty(span.ChoiceId) ? AdventureBlock.Text("text", span.Text)
                        : new AdventureBlock { kind = "link", text = span.Text, target = "#choice/" + span.ChoiceId });
                prose.children.Add(line);
            }
            scene.children.Add(prose);

            var actions = AdventureBlock.Group("topics");
            if (!frame.Ending)
            {
                foreach (var group in frame.ChoiceGroups)
                {
                    if (!string.IsNullOrEmpty(group.Label)) actions.children.Add(AdventureBlock.Text("label", group.Label));
                    foreach (var choice in group.Choices)
                    {
                        actions.children.Add(new AdventureBlock { kind = "choice", text = ChoiceLabel(group.Role, choice), target = choice.Id });
                    }
                }
                if (!frame.AllChoices.Any()) actions.children.Add(AdventureBlock.Text("text", "選べる選択肢がありません。"));
            }
            else actions.children.Add(AdventureBlock.Text("heading", inquiry ? "依頼完了" : "再生終了"));

            if (frame.Notes.Count > 0)
            {
                var journal = AdventureBlock.Group("disclosure", "旅の手帳");
                foreach (var note in frame.Notes)
                    journal.children.Add(AdventureBlock.Text("text", string.IsNullOrEmpty(note.Source) ? note.Text : note.Source + "：" + note.Text));
                actions.children.Add(journal);
            }
            scene.children.Add(actions);
            return new AdventurePage { id = frame.NodeId, title = frame.Header?.Title, blocks = new[] { scene } };
        }
    }
}
