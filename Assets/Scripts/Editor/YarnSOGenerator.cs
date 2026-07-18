#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ProjectFoundPhone.Data;
using UnityEditor;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    /// <summary>
    /// active/ 内の Yarn を走査し、TopicData / CharacterProfile / ChannelData を同期する Editor ツール。
    /// </summary>
    public class YarnSOGenerator : EditorWindow
    {
        private const string ActiveYarnDir = "Assets/Resources/Yarn/active";
        private const string TopicsDir = "Assets/Resources/Topics";
        private const string CharactersDir = "Assets/Resources/Characters";
        private const string ChannelsDir = "Assets/Resources/Channels";
        private const string AutoDescription = "(Auto-generated from Yarn. Edit in Inspector.)";

        private static readonly Regex UnlockTopicRegex = new Regex(@"<<UnlockTopic\s+""([^""]+)""\s*>>", RegexOptions.Compiled);
        private static readonly Regex DiscoverFragmentRegex = new Regex(@"<<DiscoverFragment\s+""([^""]+)""\s+""[^""]*""\s+""[^""]*""\s*>>", RegexOptions.Compiled);
        private static readonly Regex SpeakerRegex = new Regex(@"<<set\s+\$speaker\s+to\s+""(\w+)""", RegexOptions.Compiled);
        private static readonly Regex TitleRegex = new Regex(@"^title:\s*(.+)$", RegexOptions.Compiled);
        private static readonly Regex ChapterNodeRegex = new Regex(@"^Ch(?<chapter>\d+)(?:_Day(?<day>\d+))?_(?<suffix>.+)$", RegexOptions.Compiled);
        private static readonly Regex ChapterFileRegex = new Regex(@"^Ch(?<chapter>\d+)_(?<suffix>.+)$", RegexOptions.Compiled);

        private static readonly HashSet<string> IgnoredSpeakers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "system", "narrator"
        };

        private Vector2 m_ScrollPosition;
        private List<MissingTopic> m_MissingTopics = new List<MissingTopic>();
        private List<MissingCharacter> m_MissingCharacters = new List<MissingCharacter>();
        private List<ChannelSyncInfo> m_ChannelSyncItems = new List<ChannelSyncInfo>();
        private int m_TotalTopicRefs;
        private int m_TotalSpeakerRefs;
        private int m_TotalChannelRefs;
        private bool m_HasScanned;

        public readonly struct AuthoringScanSummary
        {
            public AuthoringScanSummary(
                int yarnFileCount,
                int nodeCount,
                int topicReferenceCount,
                int speakerReferenceCount,
                int channelReferenceCount,
                int missingTopicCount,
                int missingCharacterCount,
                int channelSyncItemCount,
                string[] nodeNames,
                YarnNodeSourceLocation[] nodeLocations)
            {
                YarnFileCount = yarnFileCount;
                NodeCount = nodeCount;
                TopicReferenceCount = topicReferenceCount;
                SpeakerReferenceCount = speakerReferenceCount;
                ChannelReferenceCount = channelReferenceCount;
                MissingTopicCount = missingTopicCount;
                MissingCharacterCount = missingCharacterCount;
                ChannelSyncItemCount = channelSyncItemCount;
                NodeNames = nodeNames ?? Array.Empty<string>();
                NodeLocations = nodeLocations ?? Array.Empty<YarnNodeSourceLocation>();
            }

            public int YarnFileCount { get; }
            public int NodeCount { get; }
            public int TopicReferenceCount { get; }
            public int SpeakerReferenceCount { get; }
            public int ChannelReferenceCount { get; }
            public int MissingTopicCount { get; }
            public int MissingCharacterCount { get; }
            public int ChannelSyncItemCount { get; }
            public int PendingChangeCount => MissingTopicCount + MissingCharacterCount + ChannelSyncItemCount;
            public string[] NodeNames { get; }
            public YarnNodeSourceLocation[] NodeLocations { get; }
        }

        public readonly struct YarnNodeSourceLocation
        {
            public YarnNodeSourceLocation(string nodeName, string assetPath, int titleLine)
            {
                NodeName = nodeName ?? string.Empty;
                AssetPath = assetPath ?? string.Empty;
                TitleLine = titleLine;
            }

            public string NodeName { get; }
            public string AssetPath { get; }
            public int TitleLine { get; }
        }

        [MenuItem("Tools/FoundPhone/Yarn SO Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<YarnSOGenerator>("Yarn SO Generator");
            window.minSize = new Vector2(560, 420);
        }

        [MenuItem("Tools/FoundPhone/Sync Yarn Authoring Assets")]
        public static void SyncAllAuthoringAssetsFromMenu()
        {
            int changedCount = SyncAllAuthoringAssets();
            EditorUtility.DisplayDialog(
                "Yarn Authoring Assets",
                changedCount == 0
                    ? "同期対象はありませんでした。"
                    : $"同期完了: {changedCount} 件のアセットを生成/更新しました。",
                "OK");
        }

        public static int SyncAllAuthoringAssets()
        {
            ScanResult scanResult = ScanActiveYarnFiles();
            int changedCount = ApplyScanResult(scanResult);

            if (changedCount == 0)
            {
                Debug.Log("YarnSOGenerator: No authoring asset changes were necessary.");
            }
            else
            {
                Debug.Log($"YarnSOGenerator: Generated or synced {changedCount} authoring asset(s).");
            }

            return changedCount;
        }

        public static AuthoringScanSummary GetAuthoringScanSummary()
        {
            ScanResult scanResult = ScanActiveYarnFiles();
            return new AuthoringScanSummary(
                scanResult.YarnFileCount,
                scanResult.NodeNames.Length,
                scanResult.TotalTopicRefs,
                scanResult.TotalSpeakerRefs,
                scanResult.TotalChannelRefs,
                scanResult.MissingTopics.Count,
                scanResult.MissingCharacters.Count,
                scanResult.ChannelSyncItems.Count,
                scanResult.NodeNames,
                scanResult.NodeLocations);
        }

        public static YarnNodeSourceLocation[] ParseNodeSourceLocations(
            string assetPath,
            IEnumerable<string> lines)
        {
            if (lines == null)
            {
                return Array.Empty<YarnNodeSourceLocation>();
            }

            var locations = new List<YarnNodeSourceLocation>();
            int lineNumber = 0;
            foreach (string rawLine in lines)
            {
                lineNumber++;
                Match titleMatch = TitleRegex.Match((rawLine ?? string.Empty).Trim());
                if (!titleMatch.Success)
                {
                    continue;
                }

                string nodeName = titleMatch.Groups[1].Value.Trim();
                if (!string.IsNullOrWhiteSpace(nodeName))
                {
                    locations.Add(new YarnNodeSourceLocation(nodeName, assetPath, lineNumber));
                }
            }

            return locations.ToArray();
        }

        public static string[] GetActiveYarnNodeNames()
        {
            try
            {
                return ScanActiveYarnFiles().NodeNames;
            }
            catch (Exception ex)
            {
                Debug.LogError($"YarnSOGenerator: Failed to collect node names. {ex.Message}");
                return Array.Empty<string>();
            }
        }

        public static string GetRecommendedStartNode(IEnumerable<string> nodeNames = null)
        {
            string[] candidates = (nodeNames ?? GetActiveYarnNodeNames())
                .Where(nodeName => string.IsNullOrWhiteSpace(nodeName) == false)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(nodeName => nodeName, StringComparer.Ordinal)
                .ToArray();

            if (candidates.Length == 0)
            {
                return "Start";
            }

            if (candidates.Contains("DQT_Start", StringComparer.Ordinal))
            {
                return "DQT_Start";
            }

            string firstContentOpening = candidates.FirstOrDefault(nodeName =>
                Regex.IsMatch(nodeName, @"^Ch\d+(?:_Day1)?_Opening$", RegexOptions.CultureInvariant));

            if (string.IsNullOrEmpty(firstContentOpening) == false)
            {
                return firstContentOpening;
            }

            if (candidates.Contains("ETK_Menu", StringComparer.Ordinal))
            {
                return "ETK_Menu";
            }

            return candidates[0];
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Yarn SO Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "active/ 内の Yarn を走査し、不足 Topic / Character を生成し、ChannelData も同期します。",
                MessageType.Info);

            EditorGUILayout.Space(4);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Scan Yarn Files", GUILayout.Height(30)))
                {
                    RunScan();
                }

                if (GUILayout.Button("Sync All Authoring Assets", GUILayout.Height(30)))
                {
                    int changedCount = SyncAllAuthoringAssets();
                    RunScan();
                    EditorUtility.DisplayDialog(
                        "Yarn Authoring Assets",
                        changedCount == 0
                            ? "同期対象はありませんでした。"
                            : $"同期完了: {changedCount} 件のアセットを生成/更新しました。",
                        "OK");
                    GUIUtility.ExitGUI();
                }
            }

            if (!m_HasScanned)
            {
                return;
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Scan Results", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Topic references: {m_TotalTopicRefs} (missing: {m_MissingTopics.Count})");
            EditorGUILayout.LabelField($"Speaker references: {m_TotalSpeakerRefs} (missing: {m_MissingCharacters.Count})");
            EditorGUILayout.LabelField($"Content channels: {m_TotalChannelRefs} (action needed: {m_ChannelSyncItems.Count})");

            if (m_MissingTopics.Count == 0 && m_MissingCharacters.Count == 0 && m_ChannelSyncItems.Count == 0)
            {
                EditorGUILayout.Space(8);
                EditorGUILayout.HelpBox("不足アセットも ChannelData のズレもありません。", MessageType.Info);
                return;
            }

            EditorGUILayout.Space(8);
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            DrawMissingTopicsSection();
            DrawMissingCharactersSection();
            DrawChannelSyncSection();

            EditorGUILayout.EndScrollView();
        }

        private void DrawMissingTopicsSection()
        {
            if (m_MissingTopics.Count == 0)
            {
                return;
            }

            EditorGUILayout.LabelField($"Missing TopicData ({m_MissingTopics.Count})", EditorStyles.boldLabel);
            foreach (MissingTopic topic in m_MissingTopics)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(topic.TopicID, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Referenced in: {string.Join(", ", topic.SourceFiles)}", EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("Generate", GUILayout.Width(90), GUILayout.Height(32)))
                {
                    GenerateTopic(topic);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    RunScan();
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(6);
        }

        private void DrawMissingCharactersSection()
        {
            if (m_MissingCharacters.Count == 0)
            {
                return;
            }

            EditorGUILayout.LabelField($"Missing CharacterProfile ({m_MissingCharacters.Count})", EditorStyles.boldLabel);
            foreach (MissingCharacter character in m_MissingCharacters)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(character.CharacterID, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Referenced in: {string.Join(", ", character.SourceFiles)}", EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("Generate", GUILayout.Width(90), GUILayout.Height(32)))
                {
                    GenerateCharacter(character);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    RunScan();
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(6);
        }

        private void DrawChannelSyncSection()
        {
            if (m_ChannelSyncItems.Count == 0)
            {
                return;
            }

            EditorGUILayout.LabelField($"ChannelData Sync ({m_ChannelSyncItems.Count})", EditorStyles.boldLabel);
            foreach (ChannelSyncInfo item in m_ChannelSyncItems)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.BeginVertical();
                string actionLabel = item.State == ChannelSyncState.Missing ? "Create" : "Sync";
                EditorGUILayout.LabelField(item.Spec.ChannelID, EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    $"{actionLabel}: Start={item.Spec.StartNodeName}, Days={item.Spec.TotalDays}, Sources={string.Join(", ", item.Spec.SourceFiles)}",
                    EditorStyles.miniLabel);
                if (item.Differences.Count > 0)
                {
                    EditorGUILayout.LabelField(
                        $"Fields: {string.Join(", ", item.Differences)}",
                        EditorStyles.miniLabel);
                }
                EditorGUILayout.EndVertical();
                if (GUILayout.Button(actionLabel, GUILayout.Width(90), GUILayout.Height(32)))
                {
                    SyncChannelData(item);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    RunScan();
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void RunScan()
        {
            try
            {
                ScanResult scanResult = ScanActiveYarnFiles();
                m_MissingTopics = scanResult.MissingTopics;
                m_MissingCharacters = scanResult.MissingCharacters;
                m_ChannelSyncItems = scanResult.ChannelSyncItems;
                m_TotalTopicRefs = scanResult.TotalTopicRefs;
                m_TotalSpeakerRefs = scanResult.TotalSpeakerRefs;
                m_TotalChannelRefs = scanResult.TotalChannelRefs;
                m_HasScanned = true;
                Repaint();
            }
            catch (Exception ex)
            {
                Debug.LogError($"YarnSOGenerator: Scan failed. {ex.Message}");
                EditorUtility.DisplayDialog("Yarn SO Generator", $"Scan failed:\n{ex.Message}", "OK");
            }
        }

        private static ScanResult ScanActiveYarnFiles()
        {
            string fullDir = Path.GetFullPath(ActiveYarnDir);
            if (!Directory.Exists(fullDir))
            {
                throw new DirectoryNotFoundException($"YarnSOGenerator: Directory not found: {ActiveYarnDir}");
            }

            var topicRefs = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            var speakerRefs = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            var channelBuilders = new Dictionary<int, ChannelSpecBuilder>();
            var nodeNames = new HashSet<string>(StringComparer.Ordinal);
            var nodeLocations = new List<YarnNodeSourceLocation>();

            string[] yarnFiles = Directory.GetFiles(fullDir, "*.yarn");
            foreach (string filePath in yarnFiles)
            {
                string fileName = Path.GetFileName(filePath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string[] lines = File.ReadAllLines(filePath);
                string assetPath = $"{ActiveYarnDir}/{fileName}";
                nodeLocations.AddRange(ParseNodeSourceLocations(assetPath, lines));

                ChannelFileMatch channelFileMatch = ParseChannelFileName(fileNameWithoutExtension);

                foreach (string rawLine in lines)
                {
                    string trimmed = rawLine.Trim();

                    Match unlockMatch = UnlockTopicRegex.Match(trimmed);
                    if (unlockMatch.Success)
                    {
                        AddReference(topicRefs, unlockMatch.Groups[1].Value, fileName);
                    }

                    Match discoverMatch = DiscoverFragmentRegex.Match(trimmed);
                    if (discoverMatch.Success)
                    {
                        AddReference(topicRefs, discoverMatch.Groups[1].Value, fileName);
                    }

                    Match speakerMatch = SpeakerRegex.Match(trimmed);
                    if (speakerMatch.Success)
                    {
                        string speakerId = speakerMatch.Groups[1].Value;
                        if (!IgnoredSpeakers.Contains(speakerId))
                        {
                            AddReference(speakerRefs, speakerId, fileName);
                        }
                    }

                    Match titleMatch = TitleRegex.Match(trimmed);
                    if (!titleMatch.Success)
                    {
                        continue;
                    }

                    string nodeName = titleMatch.Groups[1].Value.Trim();
                    if (string.IsNullOrWhiteSpace(nodeName))
                    {
                        continue;
                    }

                    nodeNames.Add(nodeName);

                    Match chapterMatch = ChapterNodeRegex.Match(nodeName);
                    if (!chapterMatch.Success)
                    {
                        continue;
                    }

                    int chapterNumber = int.Parse(chapterMatch.Groups["chapter"].Value);
                    int dayNumber = chapterMatch.Groups["day"].Success
                        ? int.Parse(chapterMatch.Groups["day"].Value)
                        : 1;
                    string suffix = chapterMatch.Groups["suffix"].Value;

                    if (!channelBuilders.TryGetValue(chapterNumber, out ChannelSpecBuilder builder))
                    {
                        builder = new ChannelSpecBuilder(chapterNumber);
                        channelBuilders.Add(chapterNumber, builder);
                    }

                    builder.RegisterNode(fileName, nodeName, dayNumber, suffix);

                    if (channelFileMatch.MatchesChapter(chapterNumber))
                    {
                        builder.RegisterFileDisplayHint(channelFileMatch.SuffixHint);
                    }
                }
            }

            Dictionary<string, string> existingTopicPaths = CollectExistingAssetPaths<TopicData>(TopicsDir, topic => topic.TopicID);
            Dictionary<string, string> existingCharacterPaths = CollectExistingAssetPaths<CharacterProfile>(CharactersDir, profile => profile.CharacterID);
            Dictionary<string, ExistingChannelInfo> existingChannels = CollectExistingChannels();

            List<MissingTopic> missingTopics = topicRefs
                .Where(pair => !existingTopicPaths.ContainsKey(pair.Key))
                .Select(pair => new MissingTopic
                {
                    TopicID = pair.Key,
                    SourceFiles = pair.Value.OrderBy(source => source, StringComparer.Ordinal).ToList()
                })
                .OrderBy(item => item.TopicID, StringComparer.Ordinal)
                .ToList();

            List<MissingCharacter> missingCharacters = speakerRefs
                .Where(pair => !existingCharacterPaths.ContainsKey(pair.Key))
                .Select(pair => new MissingCharacter
                {
                    CharacterID = pair.Key,
                    SourceFiles = pair.Value.OrderBy(source => source, StringComparer.Ordinal).ToList()
                })
                .OrderBy(item => item.CharacterID, StringComparer.Ordinal)
                .ToList();

            List<ChannelSyncInfo> channelSyncItems = channelBuilders
                .OrderBy(pair => pair.Key)
                .Select(pair => BuildChannelSyncInfo(pair.Value.Build(), existingChannels))
                .Where(item => item.State != ChannelSyncState.InSync)
                .ToList();

            return new ScanResult
            {
                YarnFileCount = yarnFiles.Length,
                MissingTopics = missingTopics,
                MissingCharacters = missingCharacters,
                ChannelSyncItems = channelSyncItems,
                TotalTopicRefs = topicRefs.Count,
                TotalSpeakerRefs = speakerRefs.Count,
                TotalChannelRefs = channelBuilders.Count,
                NodeNames = nodeNames.OrderBy(nodeName => nodeName, StringComparer.Ordinal).ToArray(),
                NodeLocations = nodeLocations
                    .OrderBy(location => location.NodeName, StringComparer.Ordinal)
                    .ThenBy(location => location.AssetPath, StringComparer.Ordinal)
                    .ThenBy(location => location.TitleLine)
                    .ToArray()
            };
        }

        private static int ApplyScanResult(ScanResult scanResult)
        {
            int changedCount = 0;

            foreach (MissingTopic topic in scanResult.MissingTopics)
            {
                GenerateTopic(topic);
                changedCount++;
            }

            foreach (MissingCharacter character in scanResult.MissingCharacters)
            {
                GenerateCharacter(character);
                changedCount++;
            }

            foreach (ChannelSyncInfo item in scanResult.ChannelSyncItems)
            {
                SyncChannelData(item);
                changedCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return changedCount;
        }

        private static void GenerateTopic(MissingTopic topic)
        {
            EnsureFolder(TopicsDir);

            string path = $"{TopicsDir}/{topic.TopicID}.asset";
            if (AssetDatabase.LoadAssetAtPath<TopicData>(path) != null)
            {
                Debug.Log($"YarnSOGenerator: {path} already exists, skipping.");
                return;
            }

            TopicData data = ScriptableObject.CreateInstance<TopicData>();
            SerializedObject so = new SerializedObject(data);
            so.FindProperty("m_TopicID").stringValue = topic.TopicID;
            so.FindProperty("m_Title").stringValue = HumanizeTopicID(topic.TopicID);
            so.FindProperty("m_Description").stringValue = AutoDescription;
            so.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(data, path);
            Debug.Log($"YarnSOGenerator: Created TopicData at {path}");
        }

        private static void GenerateCharacter(MissingCharacter character)
        {
            EnsureFolder(CharactersDir);

            string fileName = character.CharacterID == "player"
                ? "CharacterProfile_Player"
                : $"CharacterProfile_NPC_{CapitalizeFirst(character.CharacterID)}";

            string path = $"{CharactersDir}/{fileName}.asset";
            if (AssetDatabase.LoadAssetAtPath<CharacterProfile>(path) != null)
            {
                Debug.Log($"YarnSOGenerator: {path} already exists, skipping.");
                return;
            }

            CharacterProfile data = ScriptableObject.CreateInstance<CharacterProfile>();
            SerializedObject so = new SerializedObject(data);
            so.FindProperty("m_CharacterID").stringValue = character.CharacterID;
            so.FindProperty("m_DisplayName").stringValue = CapitalizeFirst(character.CharacterID);
            so.FindProperty("m_ThemeColor").colorValue = GenerateThemeColor(character.CharacterID);
            so.FindProperty("m_IsPlayer").boolValue = character.CharacterID == "player";
            so.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(data, path);
            Debug.Log($"YarnSOGenerator: Created CharacterProfile at {path}");
        }

        private static void SyncChannelData(ChannelSyncInfo item)
        {
            EnsureFolder(ChannelsDir);

            string assetPath = string.IsNullOrWhiteSpace(item.AssetPath)
                ? $"{ChannelsDir}/{item.Spec.ChannelID}.asset"
                : item.AssetPath;

            ChannelData channelAsset = string.IsNullOrWhiteSpace(item.AssetPath)
                ? null
                : AssetDatabase.LoadAssetAtPath<ChannelData>(item.AssetPath);

            bool isNewAsset = channelAsset == null;
            if (isNewAsset)
            {
                channelAsset = ScriptableObject.CreateInstance<ChannelData>();
            }

            SerializedObject so = new SerializedObject(channelAsset);
            so.FindProperty("m_ChannelID").stringValue = item.Spec.ChannelID;
            so.FindProperty("m_StartNodeName").stringValue = item.Spec.StartNodeName;
            so.FindProperty("m_ChapterNumber").intValue = item.Spec.ChapterNumber;
            so.FindProperty("m_TotalDays").intValue = item.Spec.TotalDays;
            SetStringArray(so.FindProperty("m_DayStartNodeNames"), item.Spec.DayStartNodeNames);

            SerializedProperty displayNameProperty = so.FindProperty("m_DisplayName");
            if (string.IsNullOrWhiteSpace(displayNameProperty.stringValue))
            {
                displayNameProperty.stringValue = item.Spec.DefaultDisplayName;
            }

            SerializedProperty descriptionProperty = so.FindProperty("m_Description");
            if (string.IsNullOrWhiteSpace(descriptionProperty.stringValue))
            {
                descriptionProperty.stringValue = AutoDescription;
            }

            SerializedProperty requiredCompletedProperty = so.FindProperty("m_RequiredCompletedChannelID");
            if (string.IsNullOrWhiteSpace(requiredCompletedProperty.stringValue)
                && string.IsNullOrWhiteSpace(item.Spec.DefaultRequiredCompletedChannelID) == false)
            {
                requiredCompletedProperty.stringValue = item.Spec.DefaultRequiredCompletedChannelID;
            }

            SerializedProperty enableHintsProperty = so.FindProperty("m_EnableHints");
            SerializedProperty maxHintDifficultyProperty = so.FindProperty("m_MaxHintDifficulty");
            if (isNewAsset)
            {
                enableHintsProperty.boolValue = true;
                maxHintDifficultyProperty.intValue = 1;
            }
            else if (maxHintDifficultyProperty.intValue < 1)
            {
                maxHintDifficultyProperty.intValue = 1;
            }

            so.ApplyModifiedPropertiesWithoutUndo();

            if (isNewAsset)
            {
                AssetDatabase.CreateAsset(channelAsset, assetPath);
                Debug.Log($"YarnSOGenerator: Created ChannelData at {assetPath}");
            }
            else
            {
                EditorUtility.SetDirty(channelAsset);
                Debug.Log($"YarnSOGenerator: Synced ChannelData at {assetPath}");
            }
        }

        private static Dictionary<string, string> CollectExistingAssetPaths<TAsset>(
            string directory,
            Func<TAsset, string> idSelector)
            where TAsset : UnityEngine.Object
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            if (AssetDatabase.IsValidFolder(directory) == false)
            {
                return result;
            }

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(TAsset).Name}", new[] { directory });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                TAsset asset = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);
                if (asset == null)
                {
                    continue;
                }

                string id = idSelector(asset);
                if (string.IsNullOrWhiteSpace(id))
                {
                    continue;
                }

                result[id] = assetPath;
            }

            return result;
        }

        private static Dictionary<string, ExistingChannelInfo> CollectExistingChannels()
        {
            var result = new Dictionary<string, ExistingChannelInfo>(StringComparer.Ordinal);
            if (AssetDatabase.IsValidFolder(ChannelsDir) == false)
            {
                return result;
            }

            string[] guids = AssetDatabase.FindAssets("t:ChannelData", new[] { ChannelsDir });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ChannelData channel = AssetDatabase.LoadAssetAtPath<ChannelData>(assetPath);
                if (channel == null || string.IsNullOrWhiteSpace(channel.ChannelID))
                {
                    continue;
                }

                result[channel.ChannelID] = new ExistingChannelInfo
                {
                    AssetPath = assetPath,
                    Channel = channel
                };
            }

            return result;
        }

        private static ChannelSyncInfo BuildChannelSyncInfo(
            ChannelSpec spec,
            Dictionary<string, ExistingChannelInfo> existingChannels)
        {
            if (existingChannels.TryGetValue(spec.ChannelID, out ExistingChannelInfo existing) == false)
            {
                return new ChannelSyncInfo
                {
                    Spec = spec,
                    State = ChannelSyncState.Missing,
                    AssetPath = $"{ChannelsDir}/{spec.ChannelID}.asset",
                    Differences = new List<string> { "Create ChannelData" }
                };
            }

            List<string> differences = GetChannelDifferences(spec, existing.Channel);

            return new ChannelSyncInfo
            {
                Spec = spec,
                State = differences.Count == 0 ? ChannelSyncState.InSync : ChannelSyncState.UpdateNeeded,
                AssetPath = existing.AssetPath,
                Differences = differences
            };
        }

        private static List<string> GetChannelDifferences(ChannelSpec spec, ChannelData existing)
        {
            var differences = new List<string>();

            if (string.Equals(existing.StartNodeName, spec.StartNodeName, StringComparison.Ordinal) == false)
            {
                differences.Add("StartNodeName");
            }

            if (existing.ChapterNumber != spec.ChapterNumber)
            {
                differences.Add("ChapterNumber");
            }

            if (existing.TotalDays != spec.TotalDays)
            {
                differences.Add("TotalDays");
            }

            if (StringArrayEquals(existing.DayStartNodeNames, spec.DayStartNodeNames) == false)
            {
                differences.Add("DayStartNodeNames");
            }

            return differences;
        }

        private static bool StringArrayEquals(string[] left, string[] right)
        {
            left ??= Array.Empty<string>();
            right ??= Array.Empty<string>();

            if (left.Length != right.Length)
            {
                return false;
            }

            for (int i = 0; i < left.Length; i++)
            {
                if (string.Equals(left[i], right[i], StringComparison.Ordinal) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private static void SetStringArray(SerializedProperty arrayProperty, IReadOnlyList<string> values)
        {
            arrayProperty.ClearArray();
            for (int i = 0; i < values.Count; i++)
            {
                arrayProperty.InsertArrayElementAtIndex(i);
                arrayProperty.GetArrayElementAtIndex(i).stringValue = values[i];
            }
        }

        private static void AddReference(
            Dictionary<string, HashSet<string>> map,
            string key,
            string sourceFile)
        {
            if (map.TryGetValue(key, out HashSet<string> sources) == false)
            {
                sources = new HashSet<string>(StringComparer.Ordinal);
                map.Add(key, sources);
            }

            sources.Add(sourceFile);
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string[] parts = folderPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (AssetDatabase.IsValidFolder(next) == false)
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        private static string HumanizeTopicID(string id)
        {
            string work = id;
            if (work.StartsWith("topic_", StringComparison.Ordinal))
            {
                work = work.Substring(6);
            }
            else if (work.StartsWith("T_", StringComparison.Ordinal))
            {
                work = work.Substring(2);
            }

            return string.Join(" ", work.Split('_')
                .Where(part => part.Length > 0)
                .Select(CapitalizeFirst));
        }

        private static string HumanizeIdentifier(string id)
        {
            return string.Join(" ", id.Split('_')
                .Where(part => part.Length > 0)
                .Select(CapitalizeFirst));
        }

        private static string CapitalizeFirst(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }

        private static Color GenerateThemeColor(string charID)
        {
            int hash = charID.GetHashCode();
            float h = Mathf.Abs(hash % 360) / 360f;
            return Color.HSVToRGB(h, 0.4f, 0.85f);
        }

        private static ChannelFileMatch ParseChannelFileName(string fileNameWithoutExtension)
        {
            Match match = ChapterFileRegex.Match(fileNameWithoutExtension);
            if (!match.Success)
            {
                return ChannelFileMatch.None;
            }

            int chapterNumber = int.Parse(match.Groups["chapter"].Value);
            string suffix = match.Groups["suffix"].Value;

            if (suffix.StartsWith("Day", StringComparison.OrdinalIgnoreCase))
            {
                return new ChannelFileMatch(chapterNumber, null);
            }

            return new ChannelFileMatch(chapterNumber, HumanizeIdentifier(suffix));
        }

        private sealed class ScanResult
        {
            public int YarnFileCount;
            public List<MissingTopic> MissingTopics;
            public List<MissingCharacter> MissingCharacters;
            public List<ChannelSyncInfo> ChannelSyncItems;
            public int TotalTopicRefs;
            public int TotalSpeakerRefs;
            public int TotalChannelRefs;
            public string[] NodeNames;
            public YarnNodeSourceLocation[] NodeLocations;
        }

        private sealed class ChannelSpecBuilder
        {
            private readonly Dictionary<int, string> m_FirstNodeByDay = new Dictionary<int, string>();
            private readonly Dictionary<int, string> m_OpeningNodeByDay = new Dictionary<int, string>();
            private readonly HashSet<string> m_SourceFiles = new HashSet<string>(StringComparer.Ordinal);
            private string m_DisplayNameSuffix;

            public ChannelSpecBuilder(int chapterNumber)
            {
                ChapterNumber = chapterNumber;
            }

            public int ChapterNumber { get; }
            public int HighestDay { get; private set; } = 1;

            public void RegisterNode(string sourceFile, string nodeName, int dayNumber, string suffix)
            {
                HighestDay = Mathf.Max(HighestDay, dayNumber);
                m_SourceFiles.Add(sourceFile);

                if (m_FirstNodeByDay.ContainsKey(dayNumber) == false)
                {
                    m_FirstNodeByDay.Add(dayNumber, nodeName);
                }

                if (string.Equals(suffix, "Opening", StringComparison.OrdinalIgnoreCase))
                {
                    m_OpeningNodeByDay[dayNumber] = nodeName;
                }
            }

            public void RegisterFileDisplayHint(string suffixHint)
            {
                if (string.IsNullOrWhiteSpace(suffixHint))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(m_DisplayNameSuffix))
                {
                    m_DisplayNameSuffix = suffixHint;
                }
            }

            public ChannelSpec Build()
            {
                int totalDays = Mathf.Max(HighestDay, 1);
                string[] dayStartNodes = new string[totalDays];

                for (int day = 1; day <= totalDays; day++)
                {
                    if (m_OpeningNodeByDay.TryGetValue(day, out string openingNode))
                    {
                        dayStartNodes[day - 1] = openingNode;
                    }
                    else if (m_FirstNodeByDay.TryGetValue(day, out string firstNode))
                    {
                        dayStartNodes[day - 1] = firstNode;
                    }
                    else
                    {
                        dayStartNodes[day - 1] = dayStartNodes[Mathf.Max(day - 2, 0)];
                    }
                }

                string displayName = string.IsNullOrWhiteSpace(m_DisplayNameSuffix)
                    ? $"Ch.{ChapterNumber}"
                    : $"Ch.{ChapterNumber} -- {m_DisplayNameSuffix}";

                return new ChannelSpec
                {
                    ChannelID = $"ch{ChapterNumber}",
                    ChapterNumber = ChapterNumber,
                    StartNodeName = dayStartNodes[0],
                    TotalDays = totalDays,
                    DayStartNodeNames = dayStartNodes,
                    DefaultDisplayName = displayName,
                    DefaultRequiredCompletedChannelID = ChapterNumber > 1 ? $"ch{ChapterNumber - 1}" : string.Empty,
                    SourceFiles = m_SourceFiles.OrderBy(source => source, StringComparer.Ordinal).ToList()
                };
            }
        }

        private readonly struct ChannelFileMatch
        {
            public static ChannelFileMatch None => new ChannelFileMatch(-1, null);

            public ChannelFileMatch(int chapterNumber, string suffixHint)
            {
                ChapterNumber = chapterNumber;
                SuffixHint = suffixHint;
            }

            public int ChapterNumber { get; }
            public string SuffixHint { get; }

            public bool MatchesChapter(int chapterNumber)
            {
                return ChapterNumber == chapterNumber;
            }
        }

        private struct MissingTopic
        {
            public string TopicID;
            public List<string> SourceFiles;
        }

        private struct MissingCharacter
        {
            public string CharacterID;
            public List<string> SourceFiles;
        }

        private struct ChannelSpec
        {
            public string ChannelID;
            public int ChapterNumber;
            public string StartNodeName;
            public int TotalDays;
            public string[] DayStartNodeNames;
            public string DefaultDisplayName;
            public string DefaultRequiredCompletedChannelID;
            public List<string> SourceFiles;
        }

        private enum ChannelSyncState
        {
            InSync,
            Missing,
            UpdateNeeded
        }

        private struct ChannelSyncInfo
        {
            public ChannelSpec Spec;
            public ChannelSyncState State;
            public string AssetPath;
            public List<string> Differences;
        }

        private struct ExistingChannelInfo
        {
            public string AssetPath;
            public ChannelData Channel;
        }
    }
}
#endif
