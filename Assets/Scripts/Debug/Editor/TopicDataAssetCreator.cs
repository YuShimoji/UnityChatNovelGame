using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Debug.Editor
{
    /// <summary>
    /// TopicData ScriptableObject 繧｢繧ｻ繝・ヨ繧定・蜍慕函謌舌☆繧九お繝・ぅ繧ｿ繧ｹ繧ｯ繝ｪ繝励ヨ
    /// 蛻晄悄繧ｷ繝翫Μ繧ｪ縺ｧ菴ｿ逕ｨ縺吶ｋ繝医ヴ繝・け繧｢繧ｻ繝・ヨ繧剃ｽ懈・縺吶ｋ
    /// </summary>
    public static class TopicDataAssetCreator
    {
        private const string c_ResourcesPath = "Assets/Resources";
        private const string c_TopicsPath = "Assets/Resources/Topics";

        /// <summary>
        /// 蛻晄悄繝医ヴ繝・け繧｢繧ｻ繝・ヨ繧剃ｽ懈・縺吶ｋ
        /// </summary>
        [MenuItem("Tools/FoundPhone/Create Initial Topic Assets")]
        public static void CreateInitialTopicAssets()
        {
            // Topics 繝・ぅ繝ｬ繧ｯ繝医Μ縺悟ｭ伜惠縺励↑縺・ｴ蜷医・菴懈・
            if (!AssetDatabase.IsValidFolder(c_TopicsPath))
            {
                if (!AssetDatabase.IsValidFolder(c_ResourcesPath))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateFolder(c_ResourcesPath, "Topics");
                AssetDatabase.Refresh();
            }

            // 蛻晄悄繝医ヴ繝・け繝・・繧ｿ縺ｮ螳夂ｾｩ
            var topics = new[]
            {
                new TopicDefinition
                {
                    TopicID = "debug_topic_01",
                    Title = "Strange Signal",
                    Description = "諡ｾ縺｣縺溘せ繝槭・縺九ｉ蜿嶺ｿ｡縺励◆荳榊ｯｩ縺ｪ菫｡蜿ｷ縲ゅヮ繧､繧ｺ縺悟､壹￥縲∝・螳ｹ縺ｯ荳肴・迸ｭ縺縺後∽ｽ輔°驥崎ｦ√↑諠・ｱ縺碁國縺輔ｌ縺ｦ縺・ｋ豌励′縺吶ｋ縲・
                },
                new TopicDefinition
                {
                    TopicID = "topic_missing_person",
                    Title = "Missing Person",
                    Description = "陦梧婿荳肴・閠・↓髢｢縺吶ｋ諠・ｱ縲ゅせ繝槭・縺ｮ謖√■荳ｻ縺九ｂ縺励ｌ縺ｪ縺・りｩｳ邏ｰ繧定ｪｿ縺ｹ繧句ｿ・ｦ√′縺ゅｋ縲・
                },
                new TopicDefinition
                {
                    TopicID = "topic_found_phone",
                    Title = "Found Phone",
                    Description = "驕鍋ｫｯ縺ｧ諡ｾ縺｣縺溘せ繝槭・繝医ヵ繧ｩ繝ｳ縲ら判髱｢縺ｯ蜑ｲ繧後※縺・ｋ縺後√∪縺蜍穂ｽ懊＠縺ｦ縺・ｋ縲りｪｰ縺ｮ繧ゅ・縺繧阪≧縺具ｼ・
                },
                new TopicDefinition
                {
                    TopicID = "topic_suspicious_message",
                    Title = "Suspicious Message",
                    Description = "蜿嶺ｿ｡繝医Ξ繧､縺ｫ谿九＆繧後※縺・◆荳榊ｯｩ縺ｪ繝｡繝・そ繝ｼ繧ｸ縲る∽ｿ｡閠・・豁｣菴薙・荳肴・縺縺後∽ｽ輔°蜊ｱ髯ｺ縺ｪ險育判縺碁ｲ陦後＠縺ｦ縺・ｋ繧医≧縺縲・
                }
            };

            int createdCount = 0;
            int skippedCount = 0;

            foreach (var topicDef in topics)
            {
                string assetPath = $"{c_TopicsPath}/{topicDef.TopicID}.asset";

                // 譌｢縺ｫ繧｢繧ｻ繝・ヨ縺悟ｭ伜惠縺吶ｋ蝣ｴ蜷医・繧ｹ繧ｭ繝・・
                if (AssetDatabase.LoadAssetAtPath<TopicData>(assetPath) != null)
                {
                    UnityEngine.Debug.LogWarning($"TopicData asset already exists: {assetPath}. Skipping...");
                    skippedCount++;
                    continue;
                }

                // TopicData 繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ繧剃ｽ懈・
                TopicData topicData = ScriptableObject.CreateInstance<TopicData>();

                // SerializedObject 繧剃ｽｿ逕ｨ縺励※ private 繝輔ぅ繝ｼ繝ｫ繝峨ｒ險ｭ螳・                SerializedObject serializedObject = new SerializedObject(topicData);
                serializedObject.FindProperty("m_TopicID").stringValue = topicDef.TopicID;
                serializedObject.FindProperty("m_Title").stringValue = topicDef.Title;
                serializedObject.FindProperty("m_Description").stringValue = topicDef.Description;
                serializedObject.ApplyModifiedProperties();

                // 繧｢繧ｻ繝・ヨ縺ｨ縺励※菫晏ｭ・                AssetDatabase.CreateAsset(topicData, assetPath);
                createdCount++;

                UnityEngine.Debug.Log($"Created TopicData asset: {assetPath} (ID: {topicDef.TopicID}, Title: {topicDef.Title})");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UnityEngine.Debug.Log($"TopicData asset creation completed. Created: {createdCount}, Skipped: {skippedCount}");
        }

        /// <summary>
        /// 繝医ヴ繝・け螳夂ｾｩ縺ｮ繝・・繧ｿ讒矩
        /// </summary>
        private class TopicDefinition
        {
            public string TopicID;
            public string Title;
            public string Description;
        }

        /// <summary>
        /// Resources.Load 縺ｧ縺ｮ隱ｭ縺ｿ霎ｼ縺ｿ遒ｺ隱咲畑縺ｮ繝・せ繝医Γ繧ｽ繝・ラ
        /// </summary>
        [MenuItem("Tools/FoundPhone/Test TopicData Loading")]
        public static void TestTopicDataLoading()
        {
            string[] testTopicIDs = { "debug_topic_01", "topic_missing_person", "topic_found_phone", "topic_suspicious_message" };

            UnityEngine.Debug.Log("=== Testing TopicData Loading ===");
            int successCount = 0;
            int failCount = 0;

            foreach (string topicID in testTopicIDs)
            {
                TopicData topicData = Resources.Load<TopicData>($"Topics/{topicID}");
                
                if (topicData != null)
                {
                    UnityEngine.Debug.Log($"笨・Successfully loaded: {topicID} - {topicData.Title}");
                    successCount++;
                }
                else
                {
                    UnityEngine.Debug.LogError($"笨・Failed to load: Topics/{topicID}");
                    failCount++;
                }
            }

            UnityEngine.Debug.Log($"=== Test Results: {successCount} succeeded, {failCount} failed ===");
        }
    }
}
