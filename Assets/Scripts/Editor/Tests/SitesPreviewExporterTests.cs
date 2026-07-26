#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ProjectFoundPhone.Editor.Tests
{
    public class SitesPreviewExporterTests
    {
        private const string VerificationNode = "SP023_NarrationMargin_Start";

        [Test]
        public void VerificationNode_ExportsDeterministicPackageWithProvenance()
        {
            YarnSOGenerator.YarnNodeSourceLocation location = GetVerificationLocation();

            SitesPreviewExportResult first = SitesPreviewExporter.BuildPackage(location);
            SitesPreviewExportResult second = SitesPreviewExporter.BuildPackage(location);

            Assert.That(first.Success, Is.True, first.Message);
            Assert.That(first.Package.schema, Is.EqualTo(SitesPreviewExporter.SchemaId));
            Assert.That(first.Package.version, Is.EqualTo(SitesPreviewExporter.SchemaVersion));
            Assert.That(first.Package.nodeName, Is.EqualTo(VerificationNode));
            Assert.That(first.Package.source.assetPath, Is.EqualTo(location.AssetPath));
            Assert.That(first.Package.source.titleLine, Is.EqualTo(10));
            Assert.That(first.Package.source.contentSha256, Has.Length.EqualTo(64));
            Assert.That(first.Package.packageIdentitySha256, Has.Length.EqualTo(64));
            Assert.That(first.Package.displayLines.Length, Is.EqualTo(8));
            Assert.That(first.Package.displayLines.Select(line => line.ordinal),
                Is.EqualTo(Enumerable.Range(1, 8)));
            Assert.That(first.Package.displayLines.Any(line =>
                line.text.Contains("左右 20% ずつ内側に寄るため細長いバブル", StringComparison.Ordinal)),
                Is.True);
            Assert.That(first.Package.displayLines.Any(line =>
                line.speakerId == "pyramid" && line.speakerLabel == "Pyramid"),
                Is.True);
            Assert.That(first.Package.diagnostics.Count(diagnostic =>
                diagnostic.command == "BubbleMargin"), Is.EqualTo(2));
            Assert.That(first.Package.diagnostics.All(diagnostic =>
                diagnostic.severity == "warning"), Is.True);
            Assert.That(
                SitesPreviewExporter.SerializePackage(first.Package),
                Is.EqualTo(SitesPreviewExporter.SerializePackage(second.Package)));
            Assert.That(
                first.Package.packageIdentitySha256,
                Is.EqualTo(second.Package.packageIdentitySha256));
        }

        [Test]
        public void VerificationNode_SourceHashMatchesNormalizedNodeSource()
        {
            YarnSOGenerator.YarnNodeSourceLocation location = GetVerificationLocation();
            string fullPath = Path.GetFullPath(location.AssetPath);
            string normalized = File.ReadAllText(fullPath, Encoding.UTF8)
                .Replace("\r\n", "\n")
                .Replace('\r', '\n');
            string[] lines = normalized.Split('\n');
            int titleIndex = location.TitleLine - 1;
            int endIndex = Array.FindIndex(
                lines,
                titleIndex + 1,
                line => line.Trim() == "===");
            string nodeSource = string.Join(
                "\n",
                lines.Skip(titleIndex).Take(endIndex - titleIndex + 1));

            SitesPreviewExportResult result = SitesPreviewExporter.BuildPackage(location);

            Assert.That(result.Success, Is.True, result.Message);
            Assert.That(
                result.Package.source.contentSha256,
                Is.EqualTo(SitesPreviewExporter.ComputeSha256(nodeSource)));
        }

        [Test]
        public void UnsupportedFlowCommand_FailsClosedWithActionableDiagnostic()
        {
            var location = new YarnSOGenerator.YarnNodeSourceLocation(
                "DQT_Unsupported",
                "Assets/Resources/Yarn/active/DebugQuickTest.yarn",
                1);
            const string source = "title: DQT_Unsupported\n---\n" +
                                  "<<set $speaker to \"pyramid\">>\n" +
                                  "Visible line\n" +
                                  "<<jump Other_Node>>\n===\n";

            SitesPreviewExportResult result =
                SitesPreviewExporter.BuildPackageFromSource(location, source);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("unsupported flow"));
            Assert.That(result.Package.displayLines.Length, Is.EqualTo(1));
            Assert.That(result.Package.diagnostics.Length, Is.EqualTo(1));
            Assert.That(result.Package.diagnostics[0].severity, Is.EqualTo("error"));
            Assert.That(result.Package.diagnostics[0].command, Is.EqualTo("jump"));
            Assert.That(result.Package.diagnostics[0].sourceLine, Is.EqualTo(5));
        }

        [Test]
        public void CanonNode_IsRejectedBeforeSourceRead()
        {
            var location = new YarnSOGenerator.YarnNodeSourceLocation(
                "Ch1_Day1_Opening",
                "Assets/Resources/Yarn/active/Ch1_Day1.yarn",
                1);

            SitesPreviewExportResult result = SitesPreviewExporter.BuildPackage(location);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("non-canon verification nodes"));
            Assert.That(result.Package, Is.Null);
        }

        private static YarnSOGenerator.YarnNodeSourceLocation GetVerificationLocation()
        {
            YarnSOGenerator.YarnNodeSourceLocation location =
                YarnSOGenerator.GetAuthoringScanSummary()
                    .NodeLocations
                    .FirstOrDefault(item =>
                        string.Equals(item.NodeName, VerificationNode, StringComparison.Ordinal));

            Assert.That(location.NodeName, Is.EqualTo(VerificationNode));
            return location;
        }
    }
}
#endif
