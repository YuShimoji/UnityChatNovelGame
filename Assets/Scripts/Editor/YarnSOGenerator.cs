#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Editor
{
    /// <summary>
    /// Yarn ファイルを走査し、不足している ScriptableObject (TopicData / CharacterProfile) を
    /// 自動検出・一括生成する Editor ツール。
    /// YarnContentValidator のパース技術を転用。
    /// </summary>
    public class YarnSOGenerator : EditorWindow
    {
        private const string ActiveYarnDir = "Assets/Resources/Yarn/active";
        private const string TopicsDir = "Assets/Resources/Topics";
        private const string CharactersDir = "Assets/Resources/Characters";

        // system/narrator はキャラクターSOを持たない特殊話者
        private static readonly HashSet<string> IgnoredSpeakers = new HashSet<string>
        {
            "system", "narrator"
        };

        private Vector2 m_ScrollPosition;
        private List<MissingTopic> m_MissingTopics = new List<MissingTopic>();
        private List<MissingCharacter> m_MissingCharacters = new List<MissingCharacter>();
        private List<string> m_ExistingTopicIDs = new List<string>();
        private List<string> m_ExistingCharacterIDs = new List<string>();
        private int m_TotalTopicRefs;
        private int m_TotalSpeakerRefs;
        private bool m_HasScanned;

        [MenuItem("Tools/FoundPhone/Yarn SO Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<YarnSOGenerator>("Yarn SO Generator");
            window.minSize = new Vector2(520, 400);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Yarn SO Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "active/ 内の Yarn ファイルを走査し、不足している TopicData / CharacterProfile を検出・生成します。",
                MessageType.Info);

            EditorGUILayout.Space(4);
            if (GUILayout.Button("Scan Yarn Files", GUILayout.Height(30)))
            {
                RunScan();
            }

            if (!m_HasScanned) return;

            EditorGUILayout.Space(8);

            // Summary
            EditorGUILayout.LabelField("Scan Results", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Topic references: {m_TotalTopicRefs} (existing: {m_ExistingTopicIDs.Count}, missing: {m_MissingTopics.Count})");
            EditorGUILayout.LabelField($"Speaker references: {m_TotalSpeakerRefs} (existing: {m_ExistingCharacterIDs.Count}, missing: {m_MissingCharacters.Count})");

            if (m_MissingTopics.Count == 0 && m_MissingCharacters.Count == 0)
            {
                EditorGUILayout.Space(8);
                EditorGUILayout.HelpBox("All SOs are present. Nothing to generate.", MessageType.Info);
                return;
            }

            EditorGUILayout.Space(8);

            // Generate All button
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button($"Generate All Missing SOs ({m_MissingTopics.Count + m_MissingCharacters.Count})", GUILayout.Height(28)))
            {
                GenerateAllMissing();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(8);

            // Scrollable list
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            // Missing Topics
            if (m_MissingTopics.Count > 0)
            {
                EditorGUILayout.LabelField($"Missing TopicData ({m_MissingTopics.Count})", EditorStyles.boldLabel);
                foreach (var topic in m_MissingTopics)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(topic.TopicID, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Referenced in: {string.Join(", ", topic.SourceFiles)}", EditorStyles.miniLabel);
                    EditorGUILayout.EndVertical();
                    if (GUILayout.Button("Generate", GUILayout.Width(80), GUILayout.Height(32)))
                    {
                        GenerateTopic(topic);
                        RunScan(); // refresh
                        GUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            // Missing Characters
            if (m_MissingCharacters.Count > 0)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField($"Missing CharacterProfile ({m_MissingCharacters.Count})", EditorStyles.boldLabel);
                foreach (var character in m_MissingCharacters)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(character.CharacterID, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Referenced in: {string.Join(", ", character.SourceFiles)}", EditorStyles.miniLabel);
                    EditorGUILayout.EndVertical();
                    if (GUILayout.Button("Generate", GUILayout.Width(80), GUILayout.Height(32)))
                    {
                        GenerateCharacter(character);
                        RunScan();
                        GUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void RunScan()
        {
            m_MissingTopics.Clear();
            m_MissingCharacters.Clear();
            m_ExistingTopicIDs.Clear();
            m_ExistingCharacterIDs.Clear();
            m_TotalTopicRefs = 0;
            m_TotalSpeakerRefs = 0;

            string fullDir = Path.GetFullPath(ActiveYarnDir);
            if (!Directory.Exists(fullDir))
            {
                Debug.LogError($"YarnSOGenerator: Directory not found: {ActiveYarnDir}");
                m_HasScanned = true;
                return;
            }

            // Phase 1: Extract references from Yarn files
            var topicRefs = new Dictionary<string, HashSet<string>>(); // topicID -> source files
            var speakerRefs = new Dictionary<string, HashSet<string>>(); // charID -> source files

            string[] yarnFiles = Directory.GetFiles(fullDir, "*.yarn");
            foreach (string filePath in yarnFiles)
            {
                string fileName = Path.GetFileName(filePath);
                string[] lines = File.ReadAllLines(filePath);

                foreach (string rawLine in lines)
                {
                    string trimmed = rawLine.Trim();

                    // <<UnlockTopic "topicID">>
                    var unlockMatch = Regex.Match(trimmed, @"<<UnlockTopic\s+""([^""]+)""\s*>>");
                    if (unlockMatch.Success)
                    {
                        string id = unlockMatch.Groups[1].Value;
                        if (!topicRefs.ContainsKey(id))
                            topicRefs[id] = new HashSet<string>();
                        topicRefs[id].Add(fileName);
                    }

                    // <<DiscoverFragment "topicID" "threadID" "message">>
                    var discoverMatch = Regex.Match(trimmed, @"<<DiscoverFragment\s+""([^""]+)""\s+""[^""]*""\s+""[^""]*""\s*>>");
                    if (discoverMatch.Success)
                    {
                        string id = discoverMatch.Groups[1].Value;
                        if (!topicRefs.ContainsKey(id))
                            topicRefs[id] = new HashSet<string>();
                        topicRefs[id].Add(fileName);
                    }

                    // <<set $speaker to "charID">>
                    var speakerMatch = Regex.Match(trimmed, @"<<set\s+\$speaker\s+to\s+""(\w+)""");
                    if (speakerMatch.Success)
                    {
                        string id = speakerMatch.Groups[1].Value;
                        if (!IgnoredSpeakers.Contains(id))
                        {
                            if (!speakerRefs.ContainsKey(id))
                                speakerRefs[id] = new HashSet<string>();
                            speakerRefs[id].Add(fileName);
                        }
                    }
                }
            }

            m_TotalTopicRefs = topicRefs.Count;
            m_TotalSpeakerRefs = speakerRefs.Count;

            // Phase 2: Check existing SOs
            CollectExistingTopics();
            CollectExistingCharacters();

            // Phase 3: Compute missing
            foreach (var (topicID, sources) in topicRefs)
            {
                if (!m_ExistingTopicIDs.Contains(topicID))
                {
                    m_MissingTopics.Add(new MissingTopic
                    {
                        TopicID = topicID,
                        SourceFiles = sources.OrderBy(s => s).ToList()
                    });
                }
            }

            foreach (var (charID, sources) in speakerRefs)
            {
                if (!m_ExistingCharacterIDs.Contains(charID))
                {
                    m_MissingCharacters.Add(new MissingCharacter
                    {
                        CharacterID = charID,
                        SourceFiles = sources.OrderBy(s => s).ToList()
                    });
                }
            }

            m_MissingTopics.Sort((a, b) => string.Compare(a.TopicID, b.TopicID, System.StringComparison.Ordinal));
            m_MissingCharacters.Sort((a, b) => string.Compare(a.CharacterID, b.CharacterID, System.StringComparison.Ordinal));

            m_HasScanned = true;
            Repaint();
        }

        private void CollectExistingTopics()
        {
            string fullPath = Path.GetFullPath(TopicsDir);
            if (!Directory.Exists(fullPath)) return;

            string[] guids = AssetDatabase.FindAssets("t:TopicData", new[] { TopicsDir });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var topic = AssetDatabase.LoadAssetAtPath<TopicData>(assetPath);
                if (topic != null && !string.IsNullOrEmpty(topic.TopicID))
                {
                    m_ExistingTopicIDs.Add(topic.TopicID);
                }
            }
        }

        private void CollectExistingCharacters()
        {
            string[] guids = AssetDatabase.FindAssets("t:CharacterProfile", new[] { CharactersDir });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var profile = AssetDatabase.LoadAssetAtPath<CharacterProfile>(assetPath);
                if (profile != null && !string.IsNullOrEmpty(profile.CharacterID))
                {
                    m_ExistingCharacterIDs.Add(profile.CharacterID);
                }
            }
        }

        private void GenerateAllMissing()
        {
            int count = 0;
            foreach (var topic in m_MissingTopics)
            {
                GenerateTopic(topic);
                count++;
            }
            foreach (var character in m_MissingCharacters)
            {
                GenerateCharacter(character);
                count++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"YarnSOGenerator: Generated {count} SO(s).");
            RunScan(); // refresh
        }

        private void GenerateTopic(MissingTopic topic)
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
            so.FindProperty("m_Description").stringValue = $"(Auto-generated from Yarn. Edit in Inspector.)";
            so.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(data, path);
            Debug.Log($"YarnSOGenerator: Created TopicData at {path}");
        }

        private void GenerateCharacter(MissingCharacter character)
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

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;

            string[] parts = folderPath.Split('/');
            string current = parts[0]; // "Assets"
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }

        /// <summary>
        /// topic_suspicious_message -> "Suspicious Message"
        /// fragment_ch1_01 -> "Fragment Ch1 01"
        /// </summary>
        private static string HumanizeTopicID(string id)
        {
            // Remove common prefixes
            string work = id;
            if (work.StartsWith("topic_")) work = work.Substring(6);
            else if (work.StartsWith("T_")) work = work.Substring(2);

            // Split by underscore and capitalize
            return string.Join(" ", work.Split('_')
                .Where(s => s.Length > 0)
                .Select(s => CapitalizeFirst(s)));
        }

        private static string CapitalizeFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        /// <summary>
        /// キャラクターIDから決定論的にテーマカラーを生成。
        /// Inspector で後から調整する前提の仮の色。
        /// </summary>
        private static Color GenerateThemeColor(string charID)
        {
            int hash = charID.GetHashCode();
            float h = Mathf.Abs(hash % 360) / 360f;
            return Color.HSVToRGB(h, 0.4f, 0.85f);
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
    }
}
#endif
