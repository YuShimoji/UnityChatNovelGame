#if UNITY_EDITOR
using System;
using System.Linq;
using NUnit.Framework;

namespace ProjectFoundPhone.Editor.Tests
{
    public class WriterCockpitNavigationTests
    {
        [Test]
        public void ParseNodeSourceLocations_CapturesNodeFileAndTitleLine()
        {
            string[] lines =
            {
                "// preamble",
                "title: First_Node",
                "---",
                "===",
                "  title: Second_Node  ",
                "---"
            };

            YarnSOGenerator.YarnNodeSourceLocation[] locations =
                YarnSOGenerator.ParseNodeSourceLocations("Assets/Test/sample.yarn", lines);

            Assert.That(locations.Length, Is.EqualTo(2));
            Assert.That(locations[0].NodeName, Is.EqualTo("First_Node"));
            Assert.That(locations[0].AssetPath, Is.EqualTo("Assets/Test/sample.yarn"));
            Assert.That(locations[0].TitleLine, Is.EqualTo(2));
            Assert.That(locations[1].NodeName, Is.EqualTo("Second_Node"));
            Assert.That(locations[1].TitleLine, Is.EqualTo(5));
        }

        [Test]
        public void FilterNodeLocations_FiltersCaseInsensitivelyAndKeepsSourceLocation()
        {
            var locations = new[]
            {
                new YarnSOGenerator.YarnNodeSourceLocation("Ch1_Day1_Opening", "Assets/A.yarn", 4),
                new YarnSOGenerator.YarnNodeSourceLocation("DQT_Start", "Assets/B.yarn", 9),
                new YarnSOGenerator.YarnNodeSourceLocation("SP024_Immersion_Start", "Assets/C.yarn", 2)
            };

            YarnSOGenerator.YarnNodeSourceLocation[] filtered =
                WriterCockpitNavigation.FilterNodeLocations(locations, "immersion");

            Assert.That(filtered.Length, Is.EqualTo(1));
            Assert.That(filtered[0].NodeName, Is.EqualTo("SP024_Immersion_Start"));
            Assert.That(filtered[0].AssetPath, Is.EqualTo("Assets/C.yarn"));
            Assert.That(filtered[0].TitleLine, Is.EqualTo(2));
        }

        [Test]
        public void ActiveNodeIndex_CoversEveryCurrentNodeWithAValidTitleLine()
        {
            YarnSOGenerator.AuthoringScanSummary summary = YarnSOGenerator.GetAuthoringScanSummary();

            Assert.That(summary.NodeCount, Is.EqualTo(74));
            Assert.That(summary.NodeLocations.Length, Is.EqualTo(74));
            Assert.That(summary.NodeLocations.All(location => location.TitleLine > 0), Is.True);
            Assert.That(
                summary.NodeLocations.Select(location => location.NodeName).Distinct(StringComparer.Ordinal).Count(),
                Is.EqualTo(summary.NodeCount));
        }

        [Test]
        public void CommandRegistry_ExtractsGenericAndNonGenericRuntimeHandlers()
        {
            const string source = @"
                runner.AddCommandHandler<string>(""KnownOne"", HandleOne);
                runner.AddCommandHandler(""KnownTwo"", HandleTwo);
                // runner.AddCommandHandler(""CommentedOut"", HandleThree);";

            string[] commands = YarnAuthoringRegistry.ExtractRegisteredCommandNames(source);

            Assert.That(commands, Is.EqualTo(new[] { "KnownOne", "KnownTwo" }));
        }

        [TestCase("BubbleStyle")]
        [TestCase("Narration")]
        [TestCase("BubbleMargin")]
        [TestCase("SetThreadMeta")]
        [TestCase("SetTime")]
        [TestCase("DeleteMessage")]
        public void RegisteredActiveCommand_DoesNotProduceUnknownDiagnostic(string command)
        {
            bool isUnknown = YarnContentValidator.TryGetUnknownCommandDiagnostic(
                command,
                "Assets/Test/known.yarn",
                3,
                out _);

            Assert.That(isUnknown, Is.False);
        }

        [Test]
        public void IntentionalUnknownCommand_ProducesFileLineWarning()
        {
            bool isUnknown = YarnContentValidator.TryGetUnknownCommandDiagnostic(
                "DefinitelyNotRegistered",
                "Assets/Test/unknown.yarn",
                7,
                out YarnContentValidator.ValidationResult result);

            Assert.That(isUnknown, Is.True);
            Assert.That(result.Level, Is.EqualTo(YarnContentValidator.ValidationLevel.Warning));
            Assert.That(result.File, Is.EqualTo("Assets/Test/unknown.yarn"));
            Assert.That(result.Line, Is.EqualTo(7));
            Assert.That(result.Message, Does.Contain("DefinitelyNotRegistered"));
        }

        [Test]
        public void CharacterRegistry_ClassifiesUnknownProfileAsRegisteredButKeepsRealUnknownOpen()
        {
            var characterIds = YarnAuthoringRegistry.GetKnownCharacterIds();

            Assert.That(characterIds, Does.Contain("unknown"));
            Assert.That(characterIds, Does.Not.Contain("definitely_missing_character"));
        }

        [Test]
        public void ActiveValidator_HasNoRegisteredCommandOrCharacterFalsePositives()
        {
            YarnContentValidator.ValidationReport report = YarnContentValidator.GetValidationReport();

            Assert.That(report.Summary.ErrorCount, Is.EqualTo(0));
            Assert.That(report.Results.Count(result => result.Message.StartsWith("Unknown command", StringComparison.Ordinal)), Is.EqualTo(0));
            Assert.That(report.Results.Count(result => result.Message.StartsWith("Unknown character", StringComparison.Ordinal)), Is.EqualTo(0));
        }

        [Test]
        public void OpenAssetAtLine_MissingFileFailsSafely()
        {
            bool opened = WriterCockpitNavigation.TryOpenAssetAtLine(
                "Assets/Resources/Yarn/active/DefinitelyMissing.yarn",
                12,
                out string status);

            Assert.That(opened, Is.False);
            Assert.That(status, Does.Contain("missing"));
        }
    }
}
#endif
