using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Debug.Editor
{
    /// <summary>
    /// TopicData ScriptableObject アセットを自動生成するエディタスクリプト
    /// 初期シナリオで使用するトピックアセットを作成する
    /// </summary>
    public static class TopicDataAssetCreator
    {
        private const string c_ResourcesPath = "Assets/Resources";
        private const string c_TopicsPath = "Assets/Resources/Topics";

        /// <summary>
        /// 初期トピックアセットを作成する
        /// </summary>
        [MenuItem("Tools/FoundPhone/Create Initial Topic Assets")]
        public static void CreateInitialTopicAssets()
        {
            // Topics ディレクトリが存在しない場合は作成
            if (!AssetDatabase.IsValidFolder(c_TopicsPath))
            {
                if (!AssetDatabase.IsValidFolder(c_ResourcesPath))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateFolder(c_ResourcesPath, "Topics");
                AssetDatabase.Refresh();
            }

            // 初期トピックデータの定義
            var topics = new[]
            {
                new TopicDefinition
                {
                    TopicID = "debug_topic_01",
                    Title = "Strange Signal",
                    Description = "拾ったスマホから受信した不審な信号。ノイズが多く、内容は不明瞭だが、何か重要な情報が隠されている気がする。"
                },
                new TopicDefinition
                {
                    TopicID = "topic_missing_person",
                    Title = "Missing Person",
                    Description = "行方不明者に関する情報。スマホの持ち主かもしれない。詳細を調べる必要がある。"
                },
                new TopicDefinition
                {
                    TopicID = "topic_found_phone",
                    Title = "Found Phone",
                    Description = "道端で拾ったスマートフォン。画面は割れているが、まだ動作している。誰のものだろうか？"
                },
                new TopicDefinition
                {
                    TopicID = "topic_suspicious_message",
                    Title = "Suspicious Message",
                    Description = "受信トレイに残されていた不審なメッセージ。送信者の正体は不明だが、何か危険な計画が進行しているようだ。"
                }
            };

            int createdCount = 0;
            int skippedCount = 0;

            foreach (var topicDef in topics)
            {
                string assetPath = $"{c_TopicsPath}/{topicDef.TopicID}.asset";

                // 既にアセットが存在する場合はスキップ
                if (AssetDatabase.LoadAssetAtPath<TopicData>(assetPath) != null)
                {
                    UnityEngine.Debug.LogWarning($"TopicData asset already exists: {assetPath}. Skipping...");
                    skippedCount++;
                    continue;
                }

                // TopicData インスタンスを作成
                TopicData topicData = ScriptableObject.CreateInstance<TopicData>();

                // SerializedObject を使用して private フィールドを設定
                SerializedObject serializedObject = new SerializedObject(topicData);
                serializedObject.FindProperty("m_TopicID").stringValue = topicDef.TopicID;
                serializedObject.FindProperty("m_Title").stringValue = topicDef.Title;
                serializedObject.FindProperty("m_Description").stringValue = topicDef.Description;
                serializedObject.ApplyModifiedProperties();

                // アセットとして保存
                AssetDatabase.CreateAsset(topicData, assetPath);
                createdCount++;

                UnityEngine.Debug.Log($"Created TopicData asset: {assetPath} (ID: {topicDef.TopicID}, Title: {topicDef.Title})");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UnityEngine.Debug.Log($"TopicData asset creation completed. Created: {createdCount}, Skipped: {skippedCount}");
        }

        /// <summary>
        /// トピック定義のデータ構造
        /// </summary>
        private class TopicDefinition
        {
            public string TopicID;
            public string Title;
            public string Description;
        }

        /// <summary>
        /// Resources.Load での読み込み確認用のテストメソッド
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
                    UnityEngine.Debug.Log($"✓ Successfully loaded: {topicID} - {topicData.Title}");
                    successCount++;
                }
                else
                {
                    UnityEngine.Debug.LogError($"✗ Failed to load: Topics/{topicID}");
                    failCount++;
                }
            }

            UnityEngine.Debug.Log($"=== Test Results: {successCount} succeeded, {failCount} failed ===");
        }
    }
}
