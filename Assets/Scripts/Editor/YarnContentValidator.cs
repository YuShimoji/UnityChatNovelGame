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

        // 26 registered custom commands (ScenarioManager + ChatDialogueView)
        private static readonly HashSet<string> KnownCommands = new HashSet<string>
        {
            "Message", "StartWait", "SkipWait", "UnlockTopic", "Glitch",
            "SystemMessage", "Typing", "EndDay", "DeclareThread", "DeclareThreadTyped",
            "AddThreadMessage", "AddThreadChat", "Chat", "DeclareThreadLatent",
            "ManifestThread", "CompleteThread", "DeclareThreadLatentCond",
            "BeginBranch", "EndBranch", "SetBranchReflection", "DiscoverFragment",
            "AddFragmentNote", "AddHalluciCoin", "AutoBeginBranch", "Image", "set"
        };

        // Yarn built-in commands
        private static readonly HashSet<string> BuiltInCommands = new HashSet<string>
        {
            "jump", "stop", "wait", "declare", "if", "elseif", "else",
            "endif", "set", "call"
        };

        // Known character IDs from CharacterDatabase
        private static readonly HashSet<string> KnownCharacters = new HashSet<string>
        {
            "player", "pyramid", "marco", "bernardo", "mason", "oliver",
            "system", "narrator"
        };

        private Vector2 m_ScrollPosition;
        private List<ValidationResult> m_Results = new List<ValidationResult>();
        private int m_ErrorCount;
        private int m_WarningCount;
        private int m_InfoCount;
        private bool m_HasRun;

        [MenuItem("Tools/Yarn Content Validator")]
        public static void ShowWindow()
        {
            var window = GetWindow<YarnContentValidator>("Yarn Validator");
            window.minSize = new Vector2(500, 400);
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
                RunValidation();
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

        private void RunValidation()
        {
            m_Results.Clear();
            m_ErrorCount = 0;
            m_WarningCount = 0;
            m_InfoCount = 0;

            string fullDir = Path.GetFullPath(ActiveYarnDir);
            if (!Directory.Exists(fullDir))
            {
                m_Results.Add(new ValidationResult
                {
                    Level = ResultLevel.Error,
                    File = "",
                    Line = 0,
                    Message = $"Directory not found: {ActiveYarnDir}"
                });
                m_ErrorCount++;
                m_HasRun = true;
                return;
            }

            string[] yarnFiles = Directory.GetFiles(fullDir, "*.yarn");
            if (yarnFiles.Length == 0)
            {
                m_Results.Add(new ValidationResult
                {
                    Level = ResultLevel.Warning,
                    File = "",
                    Line = 0,
                    Message = "No .yarn files found in active directory"
                });
                m_WarningCount++;
                m_HasRun = true;
                return;
            }

            // Phase 1: Collect all node names and #line: tags
            var allNodes = new HashSet<string>();
            var allLineTags = new Dictionary<string, List<(string file, int line)>>();
            var allJumps = new List<(string file, int line, string target)>();
            var allVariableDeclarations = new HashSet<string>();
            var allVariableUsages = new List<(string file, int line, string variable)>();
            var allSpeakers = new List<(string file, int line, string speaker)>();
            var allCommands = new List<(string file, int line, string command, string fullLine)>();

            foreach (string filePath in yarnFiles)
            {
                string relativePath = Path.GetFileName(filePath);
                string[] lines = File.ReadAllLines(filePath);
                string currentNode = null;

                for (int i = 0; i < lines.Length; i++)
                {
                    string trimmed = lines[i].Trim();
                    int lineNum = i + 1;

                    // Node title
                    if (trimmed.StartsWith("title:"))
                    {
                        currentNode = trimmed.Substring(6).Trim();
                        allNodes.Add(currentNode);
                    }

                    // #line: tags
                    var lineTagMatch = Regex.Match(trimmed, @"#line:(\S+)");
                    if (lineTagMatch.Success)
                    {
                        string tag = lineTagMatch.Groups[1].Value;
                        if (!allLineTags.ContainsKey(tag))
                            allLineTags[tag] = new List<(string, int)>();
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
                    AddResult(ResultLevel.Error, file, line,
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
                    AddResult(ResultLevel.Error, locations[0].file, locations[0].line,
                        $"Duplicate #line: tag '{tag}' found at: {locs}");
                }
            }

            // Check unknown commands
            foreach (var (file, line, command, fullLine) in allCommands)
            {
                if (!KnownCommands.Contains(command) && !BuiltInCommands.Contains(command))
                {
                    AddResult(ResultLevel.Warning, file, line,
                        $"Unknown command '<<{command}>>'. Is it registered in ScenarioManager?");
                }
            }

            // Check unknown character IDs
            foreach (var (file, line, speaker) in allSpeakers)
            {
                if (!KnownCharacters.Contains(speaker))
                {
                    AddResult(ResultLevel.Warning, file, line,
                        $"Unknown character ID '{speaker}'. Is it in CharacterDatabase?");
                }
            }

            // Check undeclared variables (warning only, some may be declared elsewhere)
            var undeclaredVars = new HashSet<string>();
            foreach (var (file, line, variable) in allVariableUsages)
            {
                if (!allVariableDeclarations.Contains(variable) && !undeclaredVars.Contains(variable))
                {
                    // $speaker is set dynamically, not declared
                    if (variable == "$speaker" || variable == "$current_node") continue;
                    undeclaredVars.Add(variable);
                    AddResult(ResultLevel.Info, file, line,
                        $"Variable '{variable}' used without <<declare>> in active Yarn files (may be declared in Yarn Project settings)");
                }
            }

            // Summary info
            AddResult(ResultLevel.Info, "", 0,
                $"Scanned {yarnFiles.Length} files, {allNodes.Count} nodes, {allLineTags.Count} #line: tags, {allVariableDeclarations.Count} declared variables");

            m_HasRun = true;
            Repaint();
        }

        private void AddResult(ResultLevel level, string file, int line, string message)
        {
            m_Results.Add(new ValidationResult { Level = level, File = file, Line = line, Message = message });
            switch (level)
            {
                case ResultLevel.Error: m_ErrorCount++; break;
                case ResultLevel.Warning: m_WarningCount++; break;
                case ResultLevel.Info: m_InfoCount++; break;
            }
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
                ResultLevel.Error => MessageType.Error,
                ResultLevel.Warning => MessageType.Warning,
                _ => MessageType.Info
            };

            string prefix = !string.IsNullOrEmpty(result.File) && result.Line > 0
                ? $"[{result.File}:{result.Line}] "
                : "";

            EditorGUILayout.HelpBox($"{prefix}{result.Message}", msgType);
        }

        private enum ResultLevel { Error, Warning, Info }

        private struct ValidationResult
        {
            public ResultLevel Level;
            public string File;
            public int Line;
            public string Message;
        }
    }
}
