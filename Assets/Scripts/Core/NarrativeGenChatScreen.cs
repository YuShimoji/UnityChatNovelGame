using System.Collections.Generic;
using System.Linq;
using NarrativeGen;

namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// Maps a NarrativeGen screen frame (SP-SCREEN-001) onto FoundPhone chat lines.
    /// The frame decides what is shown; this class decides only how the chat shows it,
    /// so Studio, this chat and WritingPage present the same scene.
    /// </summary>
    public static class NarrativeGenChatScreen
    {
        public const string PlayerId = "player";
        public const string EndMessage = "再生終了";
        public const string NoChoiceMessage = "選べる選択肢がありません。";

        public enum LineKind
        {
            System,
            Speaker,
            Player,
        }

        public readonly struct Line
        {
            public Line(LineKind kind, string characterId, string text)
            {
                Kind = kind;
                CharacterId = characterId;
                Text = text;
            }

            public LineKind Kind { get; }
            public string CharacterId { get; }
            public string Text { get; }
        }

        /// <summary>Chat lines for <paramref name="frame"/>, given the frame shown before it (null at the start).</summary>
        public static List<Line> Lines(ScreenFrame frame, ScreenFrame previous)
        {
            var lines = new List<Line>();
            bool inquiry = frame.Layout == ScreenFrame.LayoutInquiry;

            if (previous == null && frame.Header != null && !string.IsNullOrEmpty(frame.Header.Title))
            {
                string title = string.IsNullOrEmpty(frame.Header.Region)
                    ? frame.Header.Title
                    : $"{frame.Header.Region}｜{frame.Header.Title}";
                lines.Add(System(string.IsNullOrEmpty(frame.Header.Goal) ? title : $"{title}\n{frame.Header.Goal}"));
            }

            if (!string.IsNullOrEmpty(frame.ChoiceText))
            {
                lines.Add(new Line(LineKind.Player, PlayerId, frame.ChoiceText));
            }

            if (!string.IsNullOrEmpty(frame.Place) && frame.Place != previous?.Place)
            {
                lines.Add(System($"― {frame.Place} ―"));
            }

            foreach (string paragraph in frame.ParagraphTexts.Select(p => p.Trim()).Where(p => p.Length > 0))
            {
                lines.Add(string.IsNullOrEmpty(frame.Speaker)
                    ? System(paragraph)
                    : new Line(LineKind.Speaker, frame.Speaker, paragraph));
            }

            int before = previous?.Notes.Count ?? 0;
            if (frame.NewNote)
            {
                foreach (var note in frame.Notes.Skip(before))
                {
                    lines.Add(System($"{(inquiry ? "旅の手帳に記した" : "記録した")} — {note.Source}：{note.Text}"));
                }
            }

            if (frame.Ending)
            {
                if (inquiry)
                {
                    lines.Add(System("依頼完了"));
                    if (frame.Notes.Count > 0)
                    {
                        lines.Add(System("旅の手帳\n" + string.Join("\n", frame.Notes.Select(n => $"・{n.Source}：{n.Text}"))));
                    }
                }
                lines.Add(System(EndMessage));
            }
            else if (!frame.AllChoices.Any())
            {
                lines.Add(System(NoChoiceMessage));
            }
            return lines;
        }

        /// <summary>Choices in display order, paired with their button labels.</summary>
        public static List<(string Id, string Label)> Choices(ScreenFrame frame)
        {
            var result = new List<(string, string)>();
            foreach (var group in frame.ChoiceGroups)
            {
                foreach (var choice in group.Choices)
                {
                    string label = group.Role == ScreenFrame.RoleTravel ? $"→ {choice.Text}" : choice.Text;
                    if (group.Role == ScreenFrame.RoleTopic && choice.Visited)
                    {
                        label += "（済）";
                    }
                    result.Add((choice.Id, label));
                }
            }
            return result;
        }

        private static Line System(string text) => new Line(LineKind.System, null, text);
    }
}
