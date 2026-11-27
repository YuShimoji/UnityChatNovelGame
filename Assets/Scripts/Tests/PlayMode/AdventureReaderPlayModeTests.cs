using System.Collections;
using System.Linq;
using NarrativeGen;
using NUnit.Framework;
using ProjectFoundPhone.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace ProjectFoundPhone.Tests
{
    public sealed class AdventureReaderPlayModeTests
    {
        private static void Submit(Button button)
        {
            Assert.That(button, Is.Not.Null);
            using var submit = NavigationSubmitEvent.GetPooled();
            submit.target = button;
            button.SendEvent(submit);
        }

        private static Button Named(VisualElement root, string text) =>
            root.Query<Button>().ToList().FirstOrDefault(button => button.text == text);

        [UnityTest]
        public IEnumerator ReadsImportedChaptersWithImagesAndIndependentDisclosures()
        {
            yield return SceneManager.LoadSceneAsync("TitleScene");
            yield return null;
            var root = Object.FindAnyObjectByType<UIDocument>().rootVisualElement;
            foreach (string file in new[] { "basalt-watch", "lamp-in-the-window", "twelve-years-at-the-dam", "ash-ferry-inquiry" })
            {
                var book = Resources.Load<AdventureBook>("Adventures/" + file);
                Assert.That(book, Is.Not.Null, file);
                Submit(Named(root, book.Title));
                yield return null;
                var pages = book.ReadPages();
                for (int i = 0; i < pages.Length; i++)
                {
                    Assert.That(root.Q<Label>(className: "title").text, Is.EqualTo(pages[i].title));
                    if (i == 0) Assert.That(root.Q<Image>(className: "illustration")?.image, Is.Not.Null, file);
                    foreach (var fold in root.Query<Foldout>().ToList())
                    {
                        Assert.That(fold.value, Is.False);
                        var toggle = fold.Q<Toggle>();
                        using (var evt = NavigationSubmitEvent.GetPooled())
                        {
                            evt.target = toggle;
                            toggle.SendEvent(evt);
                        }
                        Assert.That(fold.value, Is.True);
                        Assert.That(root.Q<Label>(className: "title").text, Is.EqualTo(pages[i].title));
                    }
                    if (i + 1 < pages.Length) Submit(Named(root, "次の章 →"));
                }
                Assert.That(Named(root, "次の章 →"), Is.Null);
                Submit(Named(root, "← 前の章"));
                Submit(Named(root, "目次"));
                Submit(Named(root, pages[0].title));
                Assert.That(root.Q<Label>(className: "title").text, Is.EqualTo(pages[0].title));
                Assert.That(root.Query<Foldout>().ToList().All(fold => fold.value), Is.True);
                Submit(Named(root, "作品一覧"));
            }
        }

        [UnityTest]
        public IEnumerator PlaysAnInquiryFromTheLibraryToItsEnding()
        {
            yield return SceneManager.LoadSceneAsync("TitleScene");
            yield return null;
            var root = Object.FindAnyObjectByType<UIDocument>().rootVisualElement;
            var model = Resources.Load<TextAsset>("AdventureModels/blue-selvedge");
            var reference = new ScreenFrameBuilder(Engine.LoadModel(model.text));
            string title = reference.Current.Header.Title;
            Submit(root.Query<Button>().ToList().Last(button => button.text == title));
            foreach (string id in new[] { "go_loom", "ask_thread", "go_cloth", "match_thread", "go_desk", "give_maker" })
            {
                var frame = reference.Current;
                var choices = frame.AllChoices.ToList();
                int index = choices.FindIndex(choice => choice.Id == id);
                Assert.That(index, Is.GreaterThanOrEqualTo(0), frame.NodeId + " → " + id);
                var buttons = root.Query<Button>(className: "action").ToList();
                Assert.That(buttons.Count, Is.EqualTo(choices.Count));
                Submit(buttons[index]);
                reference.Choose(id);
                yield return null;
                var lines = root.Query<Label>().ToList().Select(label => label.userData as string ?? label.text).ToList();
                CollectionAssert.IsSubsetOf(reference.Current.ParagraphTexts.ToArray(), lines);
            }
            Assert.That(reference.Current.Ending, Is.True);
            Assert.That(root.Query<Button>(className: "action").ToList(), Is.Empty);
            Assert.That(root.Q<Foldout>().text, Is.EqualTo("旅の手帳"));
            Assert.That(root.Query<Label>().ToList().Any(label => label.text == "依頼完了"), Is.True);
        }

        [UnityTest]
        public IEnumerator GamebookLinksToAHeadingInTheSameManuscript()
        {
            yield return SceneManager.LoadSceneAsync("TitleScene");
            yield return null;
            var root = Object.FindAnyObjectByType<UIDocument>().rootVisualElement;
            Submit(Named(root, "ゲームブック分岐スタイルデモ"));
            yield return null;
            var scroll = root.Q<ScrollView>();
            Assert.That(root.Query(className: "dialog").ToList().Count, Is.EqualTo(5));
            Submit(Named(root, "事務所に入る"));
            yield return null;
            yield return null;
            Assert.That(scroll.scrollOffset.y, Is.GreaterThan(0));
            var heading = root.Query<Label>(className: "heading").ToList().First(l => l.text == "事務所");
            Assert.That(heading.worldBound.yMin, Is.EqualTo(scroll.contentViewport.worldBound.yMin).Within(4f));
            Submit(Named(root, "リンク元へ戻る"));
            yield return null;
            yield return null;
            Assert.That(root.Q<ScrollView>().scrollOffset.y, Is.EqualTo(0).Within(1f));
        }

        [UnityTest]
        public IEnumerator ManuscriptInlineChoiceUpdatesTheVariableAndEnablesTheReturnChoice()
        {
            yield return SceneManager.LoadSceneAsync("TitleScene");
            yield return null;
            var root = Object.FindAnyObjectByType<UIDocument>().rootVisualElement;
            Submit(Named(root, "リンクと変数の見本"));
            var line = root.Query<Label>().ToList().First(l => (l.userData as string)?.Contains("現在の枚数は 0 枚") == true);
            Assert.That(Named(root, "枚数をゼロに戻す").enabledSelf, Is.False);
            using (var evt = NavigationSubmitEvent.GetPooled()) { evt.target = line; line.SendEvent(evt); }
            yield return null;
            Assert.That(root.Query<Label>().ToList().Any(l => l.text == "受取記録は 3 枚です。"), Is.True);
            var reset = Named(root, "枚数をゼロに戻す");
            Assert.That(reset.enabledSelf, Is.True);
            Submit(reset);
            yield return null;
            Assert.That(root.Query<Label>().ToList().Any(l => l.text == "受取記録は 0 枚です。"), Is.True);
        }
    }
}
