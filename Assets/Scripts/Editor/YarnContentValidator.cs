using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    /// <summary>
    /// Yarn スクリプトのコンテンツバリデーション Editor ツール。
    /// active/ 内の全 .yarn ファイルを静的解析し、以下を検出:
    /// - 未定義ノードへの jump/detour
    /// - 重複 #line: タグ
    /// - 未知のカスタムコマンド
    /// - 未登録キャラクターID
    /// - 未宣言変数の使用
    /// </summary>
    public class YarnContentValidator : EditorWindow
    {
        private const string ActiveYarnDir = "Assets/Resources/Yarn/active";

        // Yarn built-in commands
        private static readonly HashSet<string> BuiltInCommands = new HashSet<string>
        {
            "jump", "stop", "wait", "declare", "if", "elseif", "else",
            "endif", "set", "call"
        };

        private Vector2 m_ScrollPosition;
        private List<ValidationResult> m_Results = new List<ValidationResult>();
        private int m_ErrorCount;
        private int m_WarningCount;
        private int m_InfoCount;
        private bool m_HasRun;

        public readonly struct ValidationSummary
        {
            public ValidationSummary(int errorCount, int warningCount, int infoCount, int resultCount)
            {
                ErrorCount = errorCount;
                WarningCount = warningCount;
                InfoCount = infoCount;
                ResultCount = resultCount;
            }

            public int ErrorCount { get; }
            public int WarningCount { get; }
            public int InfoCount { get; }
            public int ResultCount { get; }
            public bool HasErrors => ErrorCount > 0;

            public string ToDisplayText()
            {
                return $"errors={ErrorCount}, warnings={WarningCount}, info={InfoCount}";
            }
        }

        public enum ValidationLevel
        {
            Error,
            Warning,
            Info
        }

        public readonly struct ValidationResult
        {
            public ValidationResult(ValidationLevel level, string file, int line, string message)
            {
                Level = level;
                File = file ?? string.Empty;
                Line = line;
                Message = message ?? string.Empty;
            }

            public ValidationLevel Level { get; }
            public string File { get; }
            public int Line { get; }
            public string Message { get; }
        }

        public sealed class ValidationReport
        {
            public ValidationReport(ValidationSummary summary, IEnumerable<ValidationResult> results)
            {
                Summary = summary;
                Results = results?.ToArray() ?? Array.Empty<ValidationResult>();
            }

            public ValidationSummary Summary { get; }
            public ValidationResult[] Results { get; }
        }

        [MenuItem("Tools/FoundPhone/Yarn Content Validator", false, 20)]
        public static void ShowWindow()
        {
            var window = GetWindow<YarnContentValidator>("Yarn Validator");
            window.minSize = new Vector2(500, 400);
        }

        /// <summary>
        /// batchmode / -executeMethod 用。Console に出力し、エラー件数を返す（0 なら OK）。
        /// </summary>
        public static int ValidateAllYarnFilesLogToConsole()
        {
            ValidationReport report = GetValidationReport();
            ValidationSummary summary = report.Summary;

            Debug.Log(
                $"YarnContentValidator (batch): {summary.ToDisplayText()}.");

            foreach (ValidationResult result in report.Results)
            {
                string prefix = string.IsNullOrEmpty(result.File) || result.Line <= 0
                    ? ""
                    : $"[{result.File}:{result.Line}] ";

                switch (result.Level)
                {
                    case ValidationLevel.Error:
                        Debug.LogError($"{prefix}{result.Message}");
                        break;
                    case ValidationLevel.Warning:
                        Debug.LogWarning($"{prefix}{result.Message}");
                        break;
                    default:
                        Debug.Log($"{prefix}{result.Message}");
                        break;
                }
            }

            return summary.ErrorCount;
        }

        public static ValidationSummary GetValidationSummary()
        {
            return GetValidationReport().Summary;
        }

        public static ValidationReport GetValidationReport()
        {
            List<ValidationResult> results = BuildValidationResults(
                out int errorCount,
                out int warningCount,
                out int infoCount);
            var summary = new ValidationSummary(errorCount, warningCount, infoCount, results.Count);
            return new ValidationReport(summary, results);
        }

        public static bool IsKnownCommand(string command)
        {
            return IsKnownCommand(command, YarnAuthoringRegistry.GetRegisteredCommandNames());
        }

        public static bool TryGetUnknownCommandDiagnostic(
            string command,
            string file,
            int line,
            out ValidationResult result)
        {
            if (IsKnownCommand(command))
            {
                result = default;
                return false;
            }

            result = new ValidationResult(
                ValidationLevel.Warning,
                file,
                line,
                $"Unknown command '<<{command}>>'. Is it registered in runtime command handlers?");
            return true;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Yarn Content Validator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                $"active/ 内の .yarn ファイルを静的解析します。\n対象: {ActiveYarnDir}",
                MessageType.Info);

            EditorGUILayout.Space(4);
            if (GUILayout.Button("Validate All Yarn Files", GUILayout.Height(30)))
            {
                RunValidationGui();
            }

            if (!m_HasRun) return;

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);

            // Summary
            var summaryStyle = new GUIStyle(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(summaryStyle);
            DrawCountLabel("Errors", m_ErrorCount, Color.red);
            DrawCountLabel("Warnings", m_WarningCount, new Color(0.9f, 0.6f, 0f));
            DrawCountLabel("Info", m_InfoCount, new Color(0.3f, 0.6f, 1f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);

            // Results list
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            foreach (var result in m_Results)
            {
                DrawResult(result);
            }
            EditorGUILayout.EndScrollView();
        }

        private void RunValidationGui()
        {
            ValidationReport report = GetValidationReport();
            m_Results = report.Results.ToList();
            m_ErrorCount = report.Summary.ErrorCount;
            m_WarningCount = report.Summary.WarningCount;
            m_InfoCount = report.Summary.InfoCount;
            m_HasRun = true;
            Repaint();
        }

        private static List<ValidationResult> BuildValidationResults(
            out int errorCount,
            out int warningCount,
            out int infoCount)
        {
            var results = new List<ValidationResult>();
            // ローカルに集計（ローカル関数から out パラメータを直接触ると CS1628 になる）
            int errCount = 0;
            int warnCount = 0;
            int infoC = 0;

            void AddResult(ValidationLevel level, string file, int line, string message)
            {
                results.Add(new ValidationResult(level, file, line, message));
                switch (level)
                {
                    case ValidationLevel.Error:
                        errCount++;
                        break;
                    case ValidationLevel.Warning:
                        warnCount++;
                        break;
                    case ValidationLevel.Info:
                        infoC++;
                        break;
                }
            }

            string fullDir = Path.GetFullPath(ActiveYarnDir);
            if (!Directory.Exists(fullDir))
            {
                AddResult(ValidationLevel.Error, "", 0, $"Directory not found: {ActiveYarnDir}");
                errorCount = errCount;
                warningCount = warnCount;
                infoCount = infoC;
                return results;
            }

            string[] yarnFiles = Directory.GetFiles(fullDir, "*.yarn");
            if (yarnFiles.Length == 0)
            {
                AddResult(ValidationLevel.Warning, "", 0, "No .yarn files found in active directory");
                errorCount = errCount;
                warningCount = warnCount;
                infoCount = infoC;
                return results;
            }

            // Phase 1: Collect all node names and #line: tags
            var allNodes = new HashSet<string>();
            var allLineTags = new Dictionary<string, List<(string file, int line)>>();
            var allJumps = new List<(string file, int line, string target)>();
            var allVariableDeclarations = new HashSet<string>();
            var allVariableUsages = new List<(string file, int line, string variable)>();
            var allSpeakers = new List<(string file, int line, string speaker)>();
            var allCommands = new List<(string file, int line, string command, string fullLine)>();
            HashSet<string> registeredCommands = YarnAuthoringRegistry.GetRegisteredCommandNames();
            HashSet<string> knownCharacters = YarnAuthoringRegistry.GetKnownCharacterIds();

            foreach (string filePath in yarnFiles)
            {
                string relativePath = $"{ActiveYarnDir}/{Path.GetFileName(filePath)}";
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    string trimmed = lines[i].Trim();
                    int lineNum = i + 1;

                    // Node title
                    if (trimmed.StartsWith("title:"))
                    {
                        string currentNode = trimmed.Substring(6).Trim();
                        allNodes.Add(currentNode);
                    }

                    // #line: tags
                    var lineTagMatch = Regex.Match(trimmed, @"#line:(\S+)");
                    if (lineTagMatch.Success)
                    {
                        string tag = lineTagMatch.Groups[1].Value;
                        if (!allLineTags.ContainsKey(tag))
                        {
                            allLineTags[tag] = new List<(string, int)>();
                        }

                        allLineTags[tag].Add((relativePath, lineNum));
                    }

                    // <<jump NodeName>>
                    var jumpMatch = Regex.Match(trimmed, @"<<jump\s+(\w+)\s*>>");
                    if (jumpMatch.Success)
                    {
                        allJumps.Add((relativePath, lineNum, jumpMatch.Groups[1].Value));
                    }

                    // <<declare $var = value>>
                    var declareMatch = Regex.Match(trimmed, @"<<declare\s+(\$\w+)");
                    if (declareMatch.Success)
                    {
                        allVariableDeclarations.Add(declareMatch.Groups[1].Value);
                    }

                    // Variable usage: $variable_name (not in declare)
                    if (!trimmed.StartsWith("<<declare"))
                    {
                        foreach (Match varMatch in Regex.Matches(trimmed, @"\$(\w+)"))
                        {
                            string varName = "$" + varMatch.Groups[1].Value;
                            allVariableUsages.Add((relativePath, lineNum, varName));
                        }
                    }

                    // <<set $speaker to "charID">>
                    var speakerMatch = Regex.Match(trimmed, @"<<set\s+\$speaker\s+to\s+""(\w+)""");
                    if (speakerMatch.Success)
                    {
                        allSpeakers.Add((relativePath, lineNum, speakerMatch.Groups[1].Value));
                    }

                    // <<CommandName ...>> (custom commands)
                    var commandMatch = Regex.Match(trimmed, @"<<(\w+)\s");
                    if (commandMatch.Success)
                    {
                        string cmd = commandMatch.Groups[1].Value;
                        allCommands.Add((relativePath, lineNum, cmd, trimmed));
                    }
                }
            }

            // Phase 2: Validate

            // Check jumps to undefined nodes
            foreach (var (file, line, target) in allJumps)
            {
                if (!allNodes.Contains(target))
                {
                    AddResult(ValidationLevel.Error, file, line,
                        $"Jump to undefined node '{target}'");
                }
            }

            // Check duplicate #line: tags
            foreach (var (tag, locations) in allLineTags)
            {
                if (locations.Count > 1)
                {
                    string locs = string.Join(", ",
                        locations.Select(l => $"{l.file}:{l.line}"));
                    AddResult(ValidationLevel.Error, locations[0].file, locations[0].line,
                        $"Duplicate #line: tag '{tag}' found at: {locs}");
                }
            }

            // Check unknown commands
            foreach (var (file, line, command, _) in allCommands)
            {
                if (!IsKnownCommand(command, registeredCommands))
                {
                    AddResult(ValidationLevel.Warning, file, line,
                        $"Unknown command '<<{command}>>'. Is it registered in runtime command handlers?");
                }
            }

            // Check unknown character IDs
            foreach (var (file, line, speaker) in allSpeakers)
            {
                if (!knownCharacters.Contains(speaker))
                {
                    AddResult(ValidationLevel.Warning, file, line,
                        $"Unknown character ID '{speaker}'. Is there a CharacterProfile asset?");
                }
            }

            // Check undeclared variables (warning only, some may be declared elsewhere)
            var undeclaredVars = new HashSet<string>();
            foreach (var (file, line, variable) in allVariableUsages)
            {
                if (!allVariableDeclarations.Contains(variable) && !undeclaredVars.Contains(variable))
                {
                    // $speaker is set dynamically, not declared
                    if (variable == "$speaker" || variable == "$current_node")
                    {
                        continue;
                    }

                    undeclaredVars.Add(variable);
                    AddResult(ValidationLevel.Info, file, line,
                        $"Variable '{variable}' used without <<declare>> in active Yarn files (may be declared in Yarn Project settings)");
                }
            }

            // Summary info
            AddResult(ValidationLevel.Info, "", 0,
                $"Scanned {yarnFiles.Length} files, {allNodes.Count} nodes, {allLineTags.Count} #line: tags, {allVariableDeclarations.Count} declared variables");

            errorCount = errCount;
            warningCount = warnCount;
            infoCount = infoC;
            return results;
        }

        private static void DrawCountLabel(string label, int count, Color color)
        {
            var style = new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = color } };
            EditorGUILayout.LabelField($"{label}: {count}", style);
        }

        private static void DrawResult(ValidationResult result)
        {
            MessageType msgType = result.Level switch
            {
                ValidationLevel.Error => MessageType.Error,
                ValidationLevel.Warning => MessageType.Warning,
                _ => MessageType.Info
            };

            string prefix = !string.IsNullOrEmpty(result.File) && result.Line > 0
                ? $"[{result.File}:{result.Line}] "
                : "";

            EditorGUILayout.HelpBox($"{prefix}{result.Message}", msgType);
        }

        private static bool IsKnownCommand(string command, ISet<string> registeredCommands)
        {
            return !string.IsNullOrWhiteSpace(command) &&
                (BuiltInCommands.Contains(command) || registeredCommands.Contains(command));
        }
    }
}
