using System;
using System.Collections.Generic;
using System.Linq;
using NarrativeGen;
using Newtonsoft.Json.Linq;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectFoundPhone.UI
{
    [DisallowMultipleComponent]
    public sealed class AdventureReader : MonoBehaviour
    {
        private PanelSettings panel;
        private VisualElement root;
        private VisualElement paper;
        private ScrollView scroll;
        private readonly Dictionary<string, VisualElement> anchors = new Dictionary<string, VisualElement>();
        private readonly Stack<(int page, float offset)> linkHistory = new Stack<(int, float)>();
        private Button returnToLink;
        private Action startFoundPhone;
        private AdventureBook book;
        private AdventurePage[] pages;
        private int pageIndex;
        private ScreenFrameBuilder game;
        private NarrativeModel model;
        private string gameTitle;
        private Color linkColor;
        private readonly HashSet<AdventureBlock> openDisclosures = new HashSet<AdventureBlock>();

        public void ShowLibrary(Action startChat = null)
        {
            if (startChat != null) startFoundPhone = startChat;
            book = null;
            pages = null;
            game = null;
            model = null;
            linkHistory.Clear();
            openDisclosures.Clear();
            BeginScreen();
            Write(paper, "作品一覧", "title");
            var books = Resources.LoadAll<AdventureBook>("Adventures").OrderBy(item => item.Title).ToArray();
            if (books.Length > 0) Write(paper, "読む", "heading");
            foreach (var entry in books)
                ActionButton(paper, entry.Title, () => OpenBook(entry), "library-item");
            var models = Resources.LoadAll<TextAsset>("AdventureModels");
            if (models.Length > 0) Write(paper, "アドベンチャーを遊ぶ", "heading");
            foreach (var model in models.OrderBy(asset => asset.name))
            {
                var title = ModelTitle(model);
                ActionButton(paper, title, () => OpenModel(model), "library-item");
            }
            if (startFoundPhone != null)
            {
                Write(paper, "チャットを遊ぶ", "heading");
                ActionButton(paper, "FoundPhone", startFoundPhone, "library-item");
            }
        }

        public void OpenBook(AdventureBook source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            try
            {
                pages = source.ReadPages();
                book = source;
                model = string.IsNullOrEmpty(book.ModelJson) ? null : Engine.LoadModel(book.ModelJson);
                game = model == null ? null : new ScreenFrameBuilder(model);
                linkHistory.Clear();
                openDisclosures.Clear();
                OpenPage(0);
            }
            catch (Exception error) { ShowError(error); }
        }

        public void OpenModel(TextAsset model, string layout = null)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            try
            {
                this.model = Engine.LoadModel(model.text);
                var session = new ScreenFrameBuilder(this.model, layout);
                gameTitle = ModelTitle(model);
                game = session;
                book = null;
                pages = null;
                linkHistory.Clear();
                openDisclosures.Clear();
                ShowFrame();
            }
            catch (Exception error) { ShowError(error); }
        }

        private static string ModelTitle(TextAsset asset)
        {
            try { return (string)JObject.Parse(asset.text)["variables"]?["依頼名"] ?? asset.name; }
            catch (Newtonsoft.Json.JsonException) { return asset.name; }
        }

        private void BeginScreen()
        {
            if (root == null)
            {
                panel = ScriptableObject.CreateInstance<PanelSettings>();
                panel.scaleMode = PanelScaleMode.ConstantPixelSize;
                panel.themeStyleSheet = Resources.Load<ThemeStyleSheet>("AdventureTheme");
                var host = new GameObject("Adventure document");
                host.SetActive(false);
                host.transform.SetParent(transform, false);
                var document = host.AddComponent<UIDocument>();
                document.panelSettings = panel;
                host.SetActive(true);
                root = document.rootVisualElement;
                root.AddToClassList("adventure");
                root.styleSheets.Add(Resources.Load<StyleSheet>("AdventureReader"));
                root.style.fontSize = UIFontConfig.Instance.smallFontSize;
                root.RegisterCallback<GeometryChangedEvent>(e =>
                    root.EnableInClassList("compact", e.newRect.width < UIFontConfig.Instance.responsiveBreakpoint));
            }
            root.Clear();
            returnToLink = null;
            anchors.Clear();
            var style = book?.Style;
            root.style.backgroundColor = ColorUtility.TryParseHtmlString(style?.background ?? "", out var bg) ? new StyleColor(bg) : StyleKeyword.Null;
            root.style.color = ColorUtility.TryParseHtmlString(style?.foreground ?? "", out var fg) ? new StyleColor(fg) : StyleKeyword.Null;
            root.EnableInClassList("light-page", style != null && bg.grayscale > 0.55f);
            linkColor = ColorUtility.TryParseHtmlString(style?.link ?? "", out var ink) ? ink : new Color(0.74f, 0.85f, 0.91f);
            if (style != null && Mathf.Abs(linkColor.grayscale - bg.grayscale) < 0.34f)
                linkColor = Color.Lerp(linkColor, fg, 0.65f);
            scroll = new ScrollView(ScrollViewMode.Vertical) { name = "reading-scroll" };
            root.Add(scroll);
            paper = Box(scroll, "paper");
            if (style != null && style.width > 0) paper.style.maxWidth = style.width + 68;
            paper.EnableInClassList("serif", (book != null || game != null) && style?.sans != true);
        }

        private void OpenPage(int index)
        {
            pageIndex = index;
            BeginScreen();
            var nav = Navigation();
            if (pages.Length > 1) ActionButton(nav, "目次", ShowContents, "nav");
            if (pages[index].title != book.Title) Write(paper, book.Title, "kicker");
            Write(paper, Expand(pages[index].title), "title");
            Draw(paper, pages[index].blocks);
            ShowReturnLink();
            if (game != null)
            {
                if (game.Current.Ending) Write(paper, "再生終了", "heading");
                else if (!HasChoiceLink(pages[index].blocks))
                {
                    foreach (var group in game.Current.ChoiceGroups)
                        foreach (var choice in group.Choices)
                            ActionButton(paper, NarrativeGenAdventurePage.ChoiceLabel(group.Role, choice), () => Choose(choice.Id), "action");
                }
            }
            if (pages.Length < 2) return;
            var footer = Box(paper, "navigation");
            if (index > 0) ActionButton(footer, "← 前の章", () => OpenPage(index - 1), "nav");
            if (index < pages.Length - 1) ActionButton(footer, "次の章 →", () => OpenPage(index + 1), "nav");
        }

        private void ShowContents()
        {
            BeginScreen();
            var nav = Navigation();
            ActionButton(nav, "本文に戻る", () => OpenPage(pageIndex), "nav");
            Write(paper, book.Title, "title");
            Write(paper, "目次", "heading");
            for (int i = 0; i < pages.Length; i++)
            {
                int target = i;
                ActionButton(paper, pages[i].title, () => OpenPage(target), "library-item");
            }
        }

        private VisualElement Navigation()
        {
            var row = Box(paper, "navigation");
            ActionButton(row, "作品一覧", () => ShowLibrary(), "nav");
            return row;
        }

        private void ShowFrame()
        {
            BeginScreen();
            Navigation();
            var frame = game.Current;
            var header = frame.Header;
            Write(paper, header?.Region, "kicker");
            Write(paper, string.IsNullOrEmpty(header?.Title) ? gameTitle : header.Title, "title");
            if (frame.Step == 0) Write(paper, header?.Goal, "text");
            Draw(paper, NarrativeGenAdventurePage.FromFrame(frame).blocks);
        }

        private void Draw(VisualElement parent, IEnumerable<AdventureBlock> blocks)
        {
            if (blocks == null) return;
            foreach (var item in blocks)
            {
                int previousCount = parent.childCount;
                switch (item.kind)
                {
                    case "diary":
                    case "inquiry":
                    case "scene":
                        var scene = Box(parent, item.kind);
                        Write(scene, Expand(item.text), item.kind == "diary" ? "diary-day" : "heading");
                        var columns = Box(scene, "columns");
                        if (item.children?.Count > 1) columns.AddToClassList("multiple");
                        Draw(columns, item.children);
                        break;
                    case "intertitle":
                        var card = Box(parent, "intertitle");
                        Write(card, Expand(item.text), "heading");
                        Draw(card, item.children);
                        break;
                    case "disclosure":
                        var fold = new Foldout { text = Expand(item.text), value = openDisclosures.Contains(item) };
                        fold.AddToClassList("disclosure");
                        fold.Q<Toggle>().Q<Label>().enableRichText = false;
                        if (book != null)
                            fold.RegisterValueChangedCallback(e =>
                            {
                                if (e.target != fold) return;
                                if (e.newValue) openDisclosures.Add(item);
                                else openDisclosures.Remove(item);
                            });
                        parent.Add(fold);
                        Draw(fold, item.children);
                        break;
                    case "image":
                        var picture = book.Illustrations[item.image];
                        var img = new Image { image = picture, scaleMode = ScaleMode.ScaleToFit, tooltip = item.text };
                        img.AddToClassList("illustration");
                        img.RegisterCallback<GeometryChangedEvent>(e =>
                        {
                            if (!Mathf.Approximately(e.newRect.width, e.oldRect.width))
                                img.style.height = e.newRect.width * picture.height / picture.width;
                        });
                        parent.Add(img);
                        break;
                    case "choice":
                        ActionButton(parent, item.text, () => Choose(item.target), "action");
                        break;
                    case "link":
                        var link = ActionButton(parent, Expand(item.text), () => Follow(item.target), "link");
                        if (!string.IsNullOrEmpty(item.role)) link.AddToClassList(item.role);
                        link.style.color = linkColor;
                        link.SetEnabled(CanFollow(item.target));
                        break;
                    case "inline":
                        var line = AdventureLinkedText.Create(item.children, Expand, CanFollow, Follow, linkColor);
                        line.AddToClassList("text");
                        parent.Add(line);
                        break;
                    case "text":
                    case "label":
                    case "caption":
                    case "heading":
                        Write(parent, Expand(item.text), item.kind);
                        break;
                    default:
                        var section = Box(parent, item.kind);
                        if (!string.IsNullOrEmpty(item.role))
                            foreach (string modifier in item.role.Split(' ')) if (modifier.Length != 0) section.AddToClassList(modifier);
                        Draw(section, item.children);
                        if (item.kind == "reveal") RevealWhenVisible(section, item.role);
                        break;
                }
                if (!string.IsNullOrEmpty(item.id) && parent.childCount > previousCount)
                    anchors.TryAdd(item.id, parent[previousCount]);
            }
        }

        private string Expand(string text) => book != null && model != null && game != null && !string.IsNullOrEmpty(text)
            ? NarrativeText.ExpandTemplate(text, model, game.Session) : text;

        private const string ChoicePrefix = "#choice/";

        private static bool HasChoiceLink(IEnumerable<AdventureBlock> blocks) => blocks != null && blocks.Any(b =>
            (b.kind == "link" && b.target?.StartsWith(ChoicePrefix, StringComparison.Ordinal) == true) || HasChoiceLink(b.children));

        private bool CanFollow(string target) => !string.IsNullOrEmpty(target) && (!target.StartsWith(ChoicePrefix, StringComparison.Ordinal)
            || game?.Current.AllChoices.Any(c => c.Id == Uri.UnescapeDataString(target.Substring(ChoicePrefix.Length))) == true);

        private void Choose(string id)
        {
            if (game == null || !game.Current.AllChoices.Any(c => c.Id == id)) return;
            try
            {
                game.Choose(id);
                linkHistory.Clear();
                if (book == null) ShowFrame();
                else
                {
                    int destination = PageFor(game.Current.NodeId);
                    OpenPage(destination < 0 ? pageIndex : destination);
                    ScrollToAnchor(game.Current.NodeId);
                }
                Animate(paper);
            }
            catch (Exception error) { ShowError(error); }
        }

        private int PageFor(string id)
        {
            bool Contains(IEnumerable<AdventureBlock> items) => items != null && items.Any(b => b.id == id || Contains(b.children));
            return Array.FindIndex(pages, p => p.id == id || p.title == id || Contains(p.blocks));
        }

        private void ScrollToAnchor(string id)
        {
            if (!anchors.TryGetValue(id, out var anchor)) return;
            var view = scroll;
            for (var parent = anchor.parent; parent != null; parent = parent.parent)
                if (parent is Foldout fold) fold.value = true;
            IVisualElementScheduledItem align = null;
            align = anchor.schedule.Execute(() =>
            {
                if (anchor.worldBound.height <= 0 || view.contentViewport.worldBound.height <= 0) return;
                align.Pause();
                view.scrollOffset += new Vector2(0, anchor.worldBound.yMin - view.contentViewport.worldBound.yMin);
                Animate(anchor);
            }).Every(16);
        }

        private void Follow(string target)
        {
            if (!CanFollow(target)) return;
            if (target.StartsWith(ChoicePrefix, StringComparison.Ordinal)) { Choose(Uri.UnescapeDataString(target.Substring(ChoicePrefix.Length))); return; }
            if (target.StartsWith("#", StringComparison.Ordinal) || target.StartsWith("chapter://", StringComparison.OrdinalIgnoreCase))
            {
                if (target == "#diary-contents") { ShowContents(); return; }
                if (pages == null) return;
                string id = Uri.UnescapeDataString(target.Substring(target[0] == '#' ? 1 : 10));
                int index = PageFor(id);
                if (index < 0) return;
                linkHistory.Push((pageIndex, scroll.scrollOffset.y));
                if (index != pageIndex) { OpenPage(index); Animate(paper); }
                ScrollToAnchor(id);
                ShowReturnLink();
            }
            else if (Uri.TryCreate(target, UriKind.Absolute, out var uri) &&
                     (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp))
                Application.OpenURL(uri.AbsoluteUri);
        }

        private void ShowReturnLink()
        {
            if (linkHistory.Count == 0 || returnToLink != null) return;
            returnToLink = ActionButton(root, "リンク元へ戻る", () =>
            {
                var previous = linkHistory.Pop();
                OpenPage(previous.page);
                var view = scroll;
                view.schedule.Execute(() => view.scrollOffset = new Vector2(0, previous.offset)).ExecuteLater(1);
            }, "return-link");
        }

        private void Animate(VisualElement element, string effect = null)
        {
            if (book?.Style?.reducedMotion == true || UIFontConfig.Instance.readerTransitionSeconds <= 0) return;
            int milliseconds = Mathf.RoundToInt(UIFontConfig.Instance.readerTransitionSeconds * 1000);
            element.experimental.animation.Start(0f, 1f, milliseconds, (el, t) =>
            {
                el.style.opacity = Mathf.Lerp(0.6f, 1f, t);
                float offset = 18 * (1 - t);
                if (effect == "slide-up") el.style.translate = new Translate(0, offset);
                if (effect == "slide-left" || effect == "slide-right") el.style.translate = new Translate(effect == "slide-left" ? -offset : offset, 0);
                if (effect == "zoom-in") el.style.scale = new Scale(Vector3.one * Mathf.Lerp(0.9f, 1f, t));
            });
        }

        private void RevealWhenVisible(VisualElement element, string effects)
        {
            string effect = effects?.Split(' ').FirstOrDefault(e => e == "fade-in" || e.StartsWith("slide-") || e == "zoom-in");
            if (effect == null || book?.Style?.reducedMotion == true || UIFontConfig.Instance.readerTransitionSeconds <= 0) return;
            var viewport = scroll.contentViewport;
            IVisualElementScheduledItem watch = null;
            watch = element.schedule.Execute(() =>
            {
                if (element.worldBound.height <= 0 || !viewport.worldBound.Overlaps(element.worldBound)) return;
                watch.Pause();
                Animate(element, effect);
            }).Every(100);
        }

        private void ShowError(Exception error)
        {
            Debug.LogException(error);
            BeginScreen();
            Navigation();
            Write(paper, "作品を開けませんでした", "heading");
            Write(paper, error.Message, "text");
        }

        private static VisualElement Box(VisualElement parent, string style)
        {
            var box = new VisualElement();
            box.AddToClassList(style);
            parent.Add(box);
            return box;
        }

        private static void Write(VisualElement parent, string text, string style)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var label = new Label(text) { enableRichText = false };
            label.AddToClassList(style);
            if (style == "title") label.style.fontSize = UIFontConfig.Instance.headingFontSize;
            if (style == "heading") label.style.fontSize = UIFontConfig.Instance.captionFontSize;
            parent.Add(label);
        }

        private static Button ActionButton(VisualElement parent, string text, Action click, string style)
        {
            var button = new Button(click) { text = text, enableRichText = false };
            button.AddToClassList(style);
            parent.Add(button);
            return button;
        }

        private void OnDestroy()
        {
            if (panel != null) Destroy(panel);
        }
    }
}
