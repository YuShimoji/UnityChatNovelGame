using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectFoundPhone.Data;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace ProjectFoundPhone.UI
{
    internal static class AdventureLinkedText
    {
        // Only the surface creates markup. Manuscript text and model values stay literal.
        private static string Escape(string value) => (value ?? "").Replace("<", "<noparse><</noparse>");

        public static Label Create(List<AdventureBlock> spans, Func<string, string> expand,
            Func<string, bool> enabled, Action<string> follow, Color color)
        {
            var label = new Label { enableRichText = true };
            var links = spans.Where(s => s.kind == "link" && enabled(s.target)).ToList();
            label.userData = string.Concat(spans.Select(s => expand(s.text)));
            int selected = -1;
            void Render()
            {
                var text = new StringBuilder();
                foreach (var span in spans)
                {
                    string words = Escape(expand(span.text));
                    int i = links.IndexOf(span);
                    if (i >= 0)
                    {
                        if (i == selected) text.Append("<b>");
                        text.Append("<link=\"").Append(i).Append("\"><color=#").Append(ColorUtility.ToHtmlStringRGB(color)).Append("><u>")
                            .Append(words).Append("</u></color></link>");
                        if (i == selected) text.Append("</b>");
                    }
                    else if (span.role == "bold") text.Append("<b>").Append(words).Append("</b>");
                    else if (span.role == "italic") text.Append("<i>").Append(words).Append("</i>");
                    else text.Append(words);
                }
                label.text = text.ToString();
            }
            Render();
            if (links.Count == 0) return label;
            label.focusable = true;
            label.tabIndex = 0;
            label.RegisterCallback<PointerUpLinkTagEvent>(e =>
            {
                if (e.button == 0 && int.TryParse(e.linkID, out int i) && i >= 0 && i < links.Count)
                    follow(links[i].target);
            });
            label.RegisterCallback<FocusInEvent>(_ => { selected = 0; Render(); });
            label.RegisterCallback<FocusOutEvent>(_ => { selected = -1; Render(); });
            label.RegisterCallback<NavigationMoveEvent>(e =>
            {
                int delta = e.direction == NavigationMoveEvent.Direction.Right ? 1 : e.direction == NavigationMoveEvent.Direction.Left ? -1 : 0;
                if (delta == 0) return;
                selected = (Math.Max(0, selected) + delta + links.Count) % links.Count;
                Render();
                e.StopPropagation();
            });
            label.RegisterCallback<NavigationSubmitEvent>(e =>
            {
                follow(links[Math.Max(selected, 0)].target);
                e.StopPropagation();
            });
            return label;
        }
    }
}
