#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Editor
{
    public static class ChannelDataCreator
    {
        [MenuItem("Tools/FoundPhone/Create Default Channel Data")]
        public static void CreateDefaultChannels()
        {
            string folderPath = "Assets/Resources/Channels";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateFolder("Assets/Resources", "Channels");
            }

            CreateChannel(folderPath, "ch1",
                "Ch.1 -- Terminal",
                "Pyramidとの最初のコンタクト。ターミナルの奇妙な挙動。",
                "Ch1_Day1_Opening", 1, "",
                enableHints: false, maxHintDifficulty: 1);

            CreateChannel(folderPath, "ch2",
                "Ch.2 -- Location Confusion",
                "新メンバーの到着。場所をめぐる矛盾。",
                "Ch2_Opening", 2, "ch1",
                enableHints: true, maxHintDifficulty: 1);

            CreateChannel(folderPath, "ch3",
                "Ch.3 -- Institutional Fragments",
                "制度文書の発掘。不可索引物の予感。",
                "Ch3_Day1_Opening", 3, "ch2",
                enableHints: true, maxHintDifficulty: 2,
                totalDays: 3,
                dayStartNodeNames: new[] { "Ch3_Day1_Opening", "Ch3_Day2_Opening", "Ch3_Day3_Opening" });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("ChannelDataCreator: Default channel data created in " + folderPath);
        }

        private static void CreateChannel(string folder, string id, string displayName,
            string description, string startNode, int chapterNum, string prerequisite,
            bool enableHints = true, int maxHintDifficulty = 1,
            int totalDays = 1, string[] dayStartNodeNames = null)
        {
            string path = $"{folder}/{id}.asset";
            if (AssetDatabase.LoadAssetAtPath<ChannelData>(path) != null)
            {
                Debug.Log($"ChannelDataCreator: {path} already exists, skipping.");
                return;
            }

            ChannelData data = ScriptableObject.CreateInstance<ChannelData>();
            SerializedObject so = new SerializedObject(data);
            so.FindProperty("m_ChannelID").stringValue = id;
            so.FindProperty("m_DisplayName").stringValue = displayName;
            so.FindProperty("m_Description").stringValue = description;
            so.FindProperty("m_StartNodeName").stringValue = startNode;
            so.FindProperty("m_ChapterNumber").intValue = chapterNum;
            so.FindProperty("m_RequiredCompletedChannelID").stringValue = prerequisite;
            so.FindProperty("m_EnableHints").boolValue = enableHints;
            so.FindProperty("m_MaxHintDifficulty").intValue = maxHintDifficulty;
            so.FindProperty("m_TotalDays").intValue = totalDays;
            if (dayStartNodeNames != null)
            {
                var arr = so.FindProperty("m_DayStartNodeNames");
                arr.ClearArray();
                for (int i = 0; i < dayStartNodeNames.Length; i++)
                {
                    arr.InsertArrayElementAtIndex(i);
                    arr.GetArrayElementAtIndex(i).stringValue = dayStartNodeNames[i];
                }
            }
            so.ApplyModifiedProperties();

            AssetDatabase.CreateAsset(data, path);
        }
    }
}
#endif
