#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    public static class SitesPreviewExporter
    {
        public const string SchemaId = "foundphone.sites-preview-package";
        public const int SchemaVersion = 1;
        public const string GeneratedPackageAssetPath =
            "sites/foundphone-demo/content/generated-preview.json";

        private const string PrototypeLabel = "Prototype content / not final story";
        private const string CanonStatus = "non-canon Unity/Yarn verification preview";

        private static readonly string[] SupportedConstructs =
        {
            "plain-text",
            "speaker-assignment",
            "SystemMessage",
            "Narration"
        };

        private static readonly HashSet<string> NonBlockingPresentationCommands =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "StartWait",
                "BubbleMargin",
                "BubbleStyle",
                "SetTypingSpeed",
                "SetTime"
            };

        private static readonly Regex CommandRegex = new Regex(
            @"^<<\s*(?<name>[A-Za-z][A-Za-z0-9_]*)\b(?<args>.*?)>>$",
            RegexOptions.Compiled);

        private static readonly Regex SpeakerAssignmentRegex = new Regex(
            @"^<<\s*set\s+\$speaker\s+to\s+""(?<speaker>[^""]+)""\s*>>$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex QuotedDisplayCommandRegex = new Regex(
            @"^<<\s*(?<name>SystemMessage|Narration)\s+""(?<text>(?:[^""\\]|\\.)*)""\s*>>$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex LineTagRegex = new Regex(
            @"\s+#line:[A-Za-z0-9_.:-]+\s*$",
            RegexOptions.Compiled);

        public static SitesPreviewExportResult Export(
            YarnSOGenerator.YarnNodeSourceLocation location,
            string outputAssetPath = GeneratedPackageAssetPath)
        {
            SitesPreviewExportResult buildResult = BuildPackage(location);
            if (!buildResult.Success)
            {
                return buildResult;
            }

            if (string.IsNullOrWhiteSpace(outputAssetPath))
            {
                return Fail(buildResult.Package, outputAssetPath, "Output path is empty.");
            }

            try
            {
                string outputFullPath = AssetPathToFullPath(outputAssetPath);
                string outputDirectory = Path.GetDirectoryName(outputFullPath);
                if (string.IsNullOrWhiteSpace(outputDirectory))
                {
                    return Fail(buildResult.Package, outputAssetPath, "Output directory could not be resolved.");
                }

                Directory.CreateDirectory(outputDirectory);
                File.WriteAllText(
                    outputFullPath,
                    SerializePackage(buildResult.Package),
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

                return new SitesPreviewExportResult(
                    true,
                    buildResult.Package,
                    outputAssetPath,
                    $"Exported {buildResult.DisplayLineCount} display lines with " +
                    $"{buildResult.UnsupportedConstructCount} unsupported construct diagnostic(s).");
            }
            catch (Exception ex)
            {
                return Fail(
                    buildResult.Package,
                    outputAssetPath,
                    $"Could not write Sites preview package: {ex.Message}");
            }
        }

        public static SitesPreviewExportResult BuildPackage(
            YarnSOGenerator.YarnNodeSourceLocation location)
        {
            if (!IsVerificationNode(location.NodeName))
            {
                return Fail(
                    null,
                    string.Empty,
                    "Sites preview export is limited to DQT_/SP023_/SP024_ non-canon verification nodes.");
            }

            if (!IsActiveYarnAssetPath(location.AssetPath))
            {
                return Fail(
                    null,
                    string.Empty,
                    "Selected node source must be an active Yarn asset.");
            }

            try
            {
                string sourceText = File.ReadAllText(
                    AssetPathToFullPath(location.AssetPath),
                    Encoding.UTF8);
                return BuildPackageFromSource(location, sourceText);
            }
            catch (Exception ex)
            {
                return Fail(
                    null,
                    string.Empty,
                    $"Could not read selected Yarn source: {ex.Message}");
            }
        }

        public static SitesPreviewExportResult BuildPackageFromSource(
            YarnSOGenerator.YarnNodeSourceLocation location,
            string sourceText)
        {
            string normalizedSource = NormalizeNewlines(sourceText ?? string.Empty);
            string[] lines = normalizedSource.Split('\n');
            int titleIndex = location.TitleLine - 1;

            if (titleIndex < 0 || titleIndex >= lines.Length)
            {
                return Fail(null, string.Empty, "Selected node title line is outside the source file.");
            }

            string expectedTitle = $"title: {location.NodeName}";
            if (!string.Equals(lines[titleIndex].Trim(), expectedTitle, StringComparison.Ordinal))
            {
                return Fail(
                    null,
                    string.Empty,
                    $"Selected node title does not match source line {location.TitleLine}.");
            }

            int bodyMarkerIndex = FindExactLine(lines, titleIndex + 1, "---");
            int endMarkerIndex = bodyMarkerIndex >= 0
                ? FindExactLine(lines, bodyMarkerIndex + 1, "===")
                : -1;

            if (bodyMarkerIndex < 0 || endMarkerIndex < 0)
            {
                return Fail(
                    null,
                    string.Empty,
                    "Selected node is missing a complete --- / === Yarn body boundary.");
            }

            string nodeSource = string.Join(
                "\n",
                lines.Skip(titleIndex).Take(endMarkerIndex - titleIndex + 1));
            string sourceContentHash = ComputeSha256(nodeSource);

            var displayLines = new List<SitesPreviewDisplayLineV1>();
            var diagnostics = new List<SitesPreviewDiagnosticV1>();
            string currentSpeaker = "system";

            for (int index = bodyMarkerIndex + 1; index < endMarkerIndex; index++)
            {
                string rawLine = lines[index] ?? string.Empty;
                string trimmed = rawLine.Trim();
                int sourceLine = index + 1;

                if (string.IsNullOrWhiteSpace(trimmed) ||
                    trimmed.StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }

                Match speakerMatch = SpeakerAssignmentRegex.Match(trimmed);
                if (speakerMatch.Success)
                {
                    currentSpeaker = speakerMatch.Groups["speaker"].Value.Trim();
                    continue;
                }

                Match displayCommandMatch = QuotedDisplayCommandRegex.Match(trimmed);
                if (displayCommandMatch.Success)
                {
                    string commandName = displayCommandMatch.Groups["name"].Value;
                    string text = UnescapeQuotedValue(displayCommandMatch.Groups["text"].Value);
                    string kind = commandName.Equals("Narration", StringComparison.OrdinalIgnoreCase)
                        ? "narration"
                        : "system";
                    string speakerId = kind == "narration" ? "narrator" : "system";
                    AddDisplayLine(displayLines, sourceLine, kind, speakerId, text);
                    continue;
                }

                Match commandMatch = CommandRegex.Match(trimmed);
                if (commandMatch.Success)
                {
                    string commandName = commandMatch.Groups["name"].Value;
                    bool nonBlocking = NonBlockingPresentationCommands.Contains(commandName);
                    diagnostics.Add(new SitesPreviewDiagnosticV1
                    {
                        sourceLine = sourceLine,
                        severity = nonBlocking ? "warning" : "error",
                        code = nonBlocking
                            ? "unsupported-presentation-command"
                            : "unsupported-flow-command",
                        command = commandName,
                        message = nonBlocking
                            ? $"{commandName} is not reproduced by the immediate-text Sites preview."
                            : $"{commandName} can change meaning or flow and blocks Package v1 export."
                    });
                    continue;
                }

                if (trimmed.StartsWith("->", StringComparison.Ordinal) ||
                    trimmed.StartsWith("[[", StringComparison.Ordinal))
                {
                    diagnostics.Add(new SitesPreviewDiagnosticV1
                    {
                        sourceLine = sourceLine,
                        severity = "error",
                        code = "unsupported-choice-or-link",
                        command = "choice",
                        message = "Choice/link syntax is outside the Package v1 supported subset."
                    });
                    continue;
                }

                string displayText = LineTagRegex.Replace(trimmed, string.Empty);
                AddDisplayLine(
                    displayLines,
                    sourceLine,
                    "message",
                    string.IsNullOrWhiteSpace(currentSpeaker) ? "system" : currentSpeaker,
                    displayText);
            }

            if (displayLines.Count == 0)
            {
                diagnostics.Add(new SitesPreviewDiagnosticV1
                {
                    sourceLine = location.TitleLine,
                    severity = "error",
                    code = "no-display-lines",
                    command = string.Empty,
                    message = "Selected node has no display lines in the Package v1 supported subset."
                });
            }

            var package = new SitesPreviewPackageV1
            {
                schema = SchemaId,
                version = SchemaVersion,
                packageIdentitySha256 = string.Empty,
                contentLabel = PrototypeLabel,
                canonStatus = CanonStatus,
                nodeName = location.NodeName,
                source = new SitesPreviewSourceV1
                {
                    assetPath = location.AssetPath,
                    titleLine = location.TitleLine,
                    contentSha256 = sourceContentHash
                },
                supportedConstructs = SupportedConstructs.ToArray(),
                displayLines = displayLines.ToArray(),
                diagnostics = diagnostics.ToArray()
            };

            package.packageIdentitySha256 = ComputePackageIdentity(package);
            bool hasBlockingDiagnostic = diagnostics.Any(diagnostic =>
                string.Equals(diagnostic.severity, "error", StringComparison.Ordinal));

            return new SitesPreviewExportResult(
                !hasBlockingDiagnostic,
                package,
                string.Empty,
                hasBlockingDiagnostic
                    ? "Package v1 export stopped because the node contains unsupported flow."
                    : $"Package v1 ready with {displayLines.Count} display lines and " +
                      $"{diagnostics.Count} explicit unsupported construct diagnostic(s).");
        }

        public static string SerializePackage(SitesPreviewPackageV1 package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            return NormalizeNewlines(JsonUtility.ToJson(package, prettyPrint: true)).TrimEnd() + "\n";
        }

        public static string ComputePackageIdentity(SitesPreviewPackageV1 package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            var canonical = new StringBuilder();
            AppendIdentityField(canonical, package.schema);
            AppendIdentityField(canonical, package.version.ToString(CultureInfo.InvariantCulture));
            AppendIdentityField(canonical, package.contentLabel);
            AppendIdentityField(canonical, package.canonStatus);
            AppendIdentityField(canonical, package.nodeName);
            AppendIdentityField(canonical, package.source?.assetPath);
            AppendIdentityField(
                canonical,
                (package.source?.titleLine ?? 0).ToString(CultureInfo.InvariantCulture));
            AppendIdentityField(canonical, package.source?.contentSha256);

            string[] supported = package.supportedConstructs ?? Array.Empty<string>();
            AppendIdentityField(canonical, supported.Length.ToString(CultureInfo.InvariantCulture));
            foreach (string construct in supported)
            {
                AppendIdentityField(canonical, construct);
            }

            SitesPreviewDisplayLineV1[] displayLines =
                package.displayLines ?? Array.Empty<SitesPreviewDisplayLineV1>();
            AppendIdentityField(canonical, displayLines.Length.ToString(CultureInfo.InvariantCulture));
            foreach (SitesPreviewDisplayLineV1 line in displayLines)
            {
                AppendIdentityField(canonical, line.ordinal.ToString(CultureInfo.InvariantCulture));
                AppendIdentityField(canonical, line.sourceLine.ToString(CultureInfo.InvariantCulture));
                AppendIdentityField(canonical, line.kind);
                AppendIdentityField(canonical, line.speakerId);
                AppendIdentityField(canonical, line.speakerLabel);
                AppendIdentityField(canonical, line.text);
            }

            SitesPreviewDiagnosticV1[] diagnostics =
                package.diagnostics ?? Array.Empty<SitesPreviewDiagnosticV1>();
            AppendIdentityField(canonical, diagnostics.Length.ToString(CultureInfo.InvariantCulture));
            foreach (SitesPreviewDiagnosticV1 diagnostic in diagnostics)
            {
                AppendIdentityField(
                    canonical,
                    diagnostic.sourceLine.ToString(CultureInfo.InvariantCulture));
                AppendIdentityField(canonical, diagnostic.severity);
                AppendIdentityField(canonical, diagnostic.code);
                AppendIdentityField(canonical, diagnostic.command);
                AppendIdentityField(canonical, diagnostic.message);
            }

            return ComputeSha256(canonical.ToString());
        }

        public static string ComputeSha256(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(bytes);
                var builder = new StringBuilder(hash.Length * 2);
                foreach (byte item in hash)
                {
                    builder.Append(item.ToString("x2", CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }
        }

        private static void AddDisplayLine(
            List<SitesPreviewDisplayLineV1> displayLines,
            int sourceLine,
            string kind,
            string speakerId,
            string text)
        {
            displayLines.Add(new SitesPreviewDisplayLineV1
            {
                ordinal = displayLines.Count + 1,
                sourceLine = sourceLine,
                kind = kind,
                speakerId = speakerId,
                speakerLabel = GetSpeakerLabel(speakerId),
                text = text ?? string.Empty
            });
        }

        private static int FindExactLine(string[] lines, int startIndex, string expected)
        {
            for (int index = Math.Max(startIndex, 0); index < lines.Length; index++)
            {
                if (string.Equals(lines[index].Trim(), expected, StringComparison.Ordinal))
                {
                    return index;
                }
            }

            return -1;
        }

        private static bool IsVerificationNode(string nodeName)
        {
            return nodeName?.StartsWith("DQT_", StringComparison.Ordinal) == true ||
                   nodeName?.StartsWith("SP023_", StringComparison.Ordinal) == true ||
                   nodeName?.StartsWith("SP024_", StringComparison.Ordinal) == true;
        }

        private static bool IsActiveYarnAssetPath(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return false;
            }

            string normalized = assetPath.Replace('\\', '/');
            return normalized.StartsWith(
                       "Assets/Resources/Yarn/active/",
                       StringComparison.Ordinal) &&
                   normalized.EndsWith(".yarn", StringComparison.OrdinalIgnoreCase);
        }

        private static string AssetPathToFullPath(string assetPath)
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            if (string.IsNullOrWhiteSpace(projectRoot))
            {
                throw new DirectoryNotFoundException("Unity project root could not be resolved.");
            }

            return Path.GetFullPath(
                Path.Combine(projectRoot, (assetPath ?? string.Empty).Replace('/', Path.DirectorySeparatorChar)));
        }

        private static string GetSpeakerLabel(string speakerId)
        {
            if (string.Equals(speakerId, "player", StringComparison.OrdinalIgnoreCase))
            {
                return "You";
            }

            if (string.Equals(speakerId, "system", StringComparison.OrdinalIgnoreCase))
            {
                return "SYSTEM";
            }

            if (string.Equals(speakerId, "narrator", StringComparison.OrdinalIgnoreCase))
            {
                return "NARRATION";
            }

            if (string.IsNullOrWhiteSpace(speakerId))
            {
                return "UNKNOWN";
            }

            return char.ToUpperInvariant(speakerId[0]) + speakerId.Substring(1);
        }

        private static string NormalizeNewlines(string value)
        {
            return (value ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');
        }

        private static string UnescapeQuotedValue(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\\"", "\"")
                .Replace("\\\\", "\\");
        }

        private static void AppendIdentityField(StringBuilder builder, string value)
        {
            string normalized = value ?? string.Empty;
            builder
                .Append(normalized.Length.ToString(CultureInfo.InvariantCulture))
                .Append(':')
                .Append(normalized)
                .Append('\n');
        }

        private static SitesPreviewExportResult Fail(
            SitesPreviewPackageV1 package,
            string outputPath,
            string message)
        {
            return new SitesPreviewExportResult(false, package, outputPath, message);
        }
    }
}
#endif
