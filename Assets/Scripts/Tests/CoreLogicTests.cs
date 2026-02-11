using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.Tests
{
    /// <summary>
    /// TopicData, SynthesisRecipe, DeductionBoard 縺ｮ繧ｳ繧｢繝ｭ繧ｸ繝・け繝・せ繝・    /// </summary>
    public class CoreLogicTests
    {
        #region Helper Methods
        /// <summary>
        /// 繝・せ繝育畑縺ｮTopicData繧剃ｽ懈・縺吶ｋ
        /// </summary>
        private TopicData CreateTopicData(string topicID, string title, string description = "")
        {
            TopicData topic = ScriptableObject.CreateInstance<TopicData>();
            SerializedObject so = new SerializedObject(topic);
            so.FindProperty("m_TopicID").stringValue = topicID;
            so.FindProperty("m_Title").stringValue = title;
            so.FindProperty("m_Description").stringValue = description;
            so.ApplyModifiedPropertiesWithoutUndo();
            return topic;
        }

        /// <summary>
        /// 繝・せ繝育畑縺ｮSynthesisRecipe繧剃ｽ懈・縺吶ｋ
        /// </summary>
        private SynthesisRecipe CreateRecipe(TopicData ingredientA, TopicData ingredientB, TopicData result)
        {
            SynthesisRecipe recipe = ScriptableObject.CreateInstance<SynthesisRecipe>();
            SerializedObject so = new SerializedObject(recipe);
            so.FindProperty("m_IngredientA").objectReferenceValue = ingredientA;
            so.FindProperty("m_IngredientB").objectReferenceValue = ingredientB;
            so.FindProperty("m_Result").objectReferenceValue = result;
            so.ApplyModifiedPropertiesWithoutUndo();
            return recipe;
        }
        #endregion

        #region TopicData Tests
        [Test]
        public void TopicData_IsValid_ReturnsTrueForValidTopic()
        {
            TopicData topic = CreateTopicData("test_01", "Test Topic");

            Assert.IsTrue(topic.IsValid());
            Assert.AreEqual("test_01", topic.TopicID);
            Assert.AreEqual("Test Topic", topic.Title);
        }

        [Test]
        public void TopicData_IsValid_ReturnsFalseForEmptyID()
        {
            TopicData topic = CreateTopicData("", "Test Topic");

            Assert.IsFalse(topic.IsValid());
        }

        [Test]
        public void TopicData_IsValid_ReturnsFalseForEmptyTitle()
        {
            TopicData topic = CreateTopicData("test_01", "");

            Assert.IsFalse(topic.IsValid());
        }

        [Test]
        public void TopicData_Properties_ReturnCorrectValues()
        {
            TopicData topic = CreateTopicData("signal_01", "Strange Signal", "A mysterious signal");

            Assert.AreEqual("signal_01", topic.TopicID);
            Assert.AreEqual("Strange Signal", topic.Title);
            Assert.AreEqual("A mysterious signal", topic.Description);
            Assert.IsNull(topic.Icon);
        }
        #endregion

        #region SynthesisRecipe Tests
        [Test]
        public void SynthesisRecipe_Matches_ReturnsTrueForCorrectOrder()
        {
            TopicData topicA = CreateTopicData("a", "Topic A");
            TopicData topicB = CreateTopicData("b", "Topic B");
            TopicData result = CreateTopicData("c", "Topic C");
            SynthesisRecipe recipe = CreateRecipe(topicA, topicB, result);

            Assert.IsTrue(recipe.Matches(topicA, topicB));
        }

        [Test]
        public void SynthesisRecipe_Matches_ReturnsTrueForReverseOrder()
        {
            TopicData topicA = CreateTopicData("a", "Topic A");
            TopicData topicB = CreateTopicData("b", "Topic B");
            TopicData result = CreateTopicData("c", "Topic C");
            SynthesisRecipe recipe = CreateRecipe(topicA, topicB, result);

            Assert.IsTrue(recipe.Matches(topicB, topicA));
        }

        [Test]
        public void SynthesisRecipe_Matches_ReturnsFalseForWrongIngredients()
        {
            TopicData topicA = CreateTopicData("a", "Topic A");
            TopicData topicB = CreateTopicData("b", "Topic B");
            TopicData topicX = CreateTopicData("x", "Topic X");
            TopicData result = CreateTopicData("c", "Topic C");
            SynthesisRecipe recipe = CreateRecipe(topicA, topicB, result);

            Assert.IsFalse(recipe.Matches(topicA, topicX));
        }

        [Test]
        public void SynthesisRecipe_Matches_ReturnsFalseForNullInput()
        {
            TopicData topicA = CreateTopicData("a", "Topic A");
            TopicData topicB = CreateTopicData("b", "Topic B");
            TopicData result = CreateTopicData("c", "Topic C");
            SynthesisRecipe recipe = CreateRecipe(topicA, topicB, result);

            Assert.IsFalse(recipe.Matches(null, topicB));
            Assert.IsFalse(recipe.Matches(topicA, null));
            Assert.IsFalse(recipe.Matches(null, null));
        }

        [Test]
        public void SynthesisRecipe_IsValid_ReturnsTrueWhenAllSet()
        {
            TopicData topicA = CreateTopicData("a", "Topic A");
            TopicData topicB = CreateTopicData("b", "Topic B");
            TopicData result = CreateTopicData("c", "Topic C");
            SynthesisRecipe recipe = CreateRecipe(topicA, topicB, result);

            Assert.IsTrue(recipe.IsValid());
        }

        [Test]
        public void SynthesisRecipe_IsValid_ReturnsFalseWhenMissingResult()
        {
            TopicData topicA = CreateTopicData("a", "Topic A");
            TopicData topicB = CreateTopicData("b", "Topic B");
            SynthesisRecipe recipe = CreateRecipe(topicA, topicB, null);

            Assert.IsFalse(recipe.IsValid());
        }
        #endregion

        #region DeductionBoard Tests
        private GameObject m_BoardObject;
        private DeductionBoard m_Board;

        [SetUp]
        public void SetupDeductionBoard()
        {
            m_BoardObject = new GameObject("DeductionBoard");
            m_Board = m_BoardObject.AddComponent<DeductionBoard>();

            // CardContainer and TopicCardPrefab are required for board test setup.
            GameObject container = new GameObject("CardContainer");
            container.transform.SetParent(m_BoardObject.transform);

            GameObject cardPrefabObj = new GameObject("TopicCardPrefab");
            cardPrefabObj.transform.SetParent(m_BoardObject.transform);
            TopicCard topicCardPrefab = cardPrefabObj.AddComponent<TopicCard>();

            SerializedObject so = new SerializedObject(m_Board);
            so.FindProperty("m_CardContainer").objectReferenceValue = container.transform;
            so.FindProperty("m_TopicCardPrefab").objectReferenceValue = topicCardPrefab;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TeardownDeductionBoard()
        {
            if (m_BoardObject != null)
            {
                Object.DestroyImmediate(m_BoardObject);
            }
        }

        [Test]
        public void DeductionBoard_AddTopic_AddsSuccessfully()
        {
            TopicData topic = CreateTopicData("test_add", "Addable Topic");

            bool result = m_Board.AddTopic(topic);

            Assert.IsTrue(result);
            Assert.IsTrue(m_Board.HasTopic("test_add"));
            Assert.AreEqual(1, m_Board.UnlockedTopics.Count);
        }

        [Test]
        public void DeductionBoard_AddTopic_RejectsDuplicate()
        {
            TopicData topic = CreateTopicData("dup_01", "Duplicate Topic");

            m_Board.AddTopic(topic);
            bool secondAdd = m_Board.AddTopic(topic);

            Assert.IsFalse(secondAdd);
            Assert.AreEqual(1, m_Board.UnlockedTopics.Count);
        }

        [Test]
        public void DeductionBoard_AddTopic_RejectsNull()
        {
            bool result = m_Board.AddTopic(null);

            Assert.IsFalse(result);
            Assert.AreEqual(0, m_Board.UnlockedTopics.Count);
        }

        [Test]
        public void DeductionBoard_RemoveTopic_RemovesSuccessfully()
        {
            TopicData topic = CreateTopicData("remove_01", "Removable Topic");
            m_Board.AddTopic(topic);

            bool result = m_Board.RemoveTopic("remove_01");

            Assert.IsTrue(result);
            Assert.IsFalse(m_Board.HasTopic("remove_01"));
            Assert.AreEqual(0, m_Board.UnlockedTopics.Count);
        }

        [Test]
        public void DeductionBoard_RemoveTopic_ReturnsFalseForNonExistent()
        {
            bool result = m_Board.RemoveTopic("nonexistent");

            Assert.IsFalse(result);
        }

        [Test]
        public void DeductionBoard_ClearAllTopics_ClearsAll()
        {
            m_Board.AddTopic(CreateTopicData("clear_a", "Topic A"));
            m_Board.AddTopic(CreateTopicData("clear_b", "Topic B"));
            m_Board.AddTopic(CreateTopicData("clear_c", "Topic C"));

            m_Board.ClearAllTopics();

            Assert.AreEqual(0, m_Board.UnlockedTopics.Count);
            Assert.IsFalse(m_Board.HasTopic("clear_a"));
        }

        [Test]
        public void DeductionBoard_HasTopic_ReturnsFalseForEmpty()
        {
            Assert.IsFalse(m_Board.HasTopic("anything"));
        }

        [Test]
        public void DeductionBoard_MultipleTopics_MaintainsOrder()
        {
            TopicData topicA = CreateTopicData("order_a", "First");
            TopicData topicB = CreateTopicData("order_b", "Second");
            TopicData topicC = CreateTopicData("order_c", "Third");

            m_Board.AddTopic(topicA);
            m_Board.AddTopic(topicB);
            m_Board.AddTopic(topicC);

            Assert.AreEqual(3, m_Board.UnlockedTopics.Count);
            Assert.AreEqual("order_a", m_Board.UnlockedTopics[0].TopicID);
            Assert.AreEqual("order_b", m_Board.UnlockedTopics[1].TopicID);
            Assert.AreEqual("order_c", m_Board.UnlockedTopics[2].TopicID);
        }
        #endregion

        #region SaveData Serialization Tests
        [Test]
        public void SaveData_Serialization_RoundTrip_PreservesData()
        {
            SaveData original = new SaveData(1);
            original.CurrentNodeName = "Chapter2_Start";
            original.UnlockedTopicIDs.Add("topic_found_phone");
            original.UnlockedTopicIDs.Add("topic_missing_person");
            original.YarnVariables["$has_topic_found_phone"] = true;
            original.YarnVariables["$player_name"] = "Alex";
            original.YarnVariables["$trust_level"] = 3.5f;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(original, Newtonsoft.Json.Formatting.Indented);
            SaveData deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(original.Version, deserialized.Version);
            Assert.AreEqual(original.SlotNumber, deserialized.SlotNumber);
            Assert.AreEqual(original.CurrentNodeName, deserialized.CurrentNodeName);
            Assert.AreEqual(original.UnlockedTopicIDs.Count, deserialized.UnlockedTopicIDs.Count);
            Assert.AreEqual("topic_found_phone", deserialized.UnlockedTopicIDs[0]);
            Assert.AreEqual("topic_missing_person", deserialized.UnlockedTopicIDs[1]);
            Assert.IsTrue(deserialized.YarnVariables.ContainsKey("$has_topic_found_phone"));
            Assert.IsTrue(deserialized.YarnVariables.ContainsKey("$player_name"));
            Assert.IsTrue(deserialized.YarnVariables.ContainsKey("$trust_level"));
        }

        [Test]
        public void SaveData_Serialization_EmptyData_RoundTrip()
        {
            SaveData original = new SaveData(0);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(original);
            SaveData deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(json);

            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.IsValid());
            Assert.AreEqual(0, deserialized.UnlockedTopicIDs.Count);
            Assert.AreEqual(0, deserialized.YarnVariables.Count);
        }

        [Test]
        public void SaveData_Serialization_YarnVariables_TypePreservation()
        {
            SaveData original = new SaveData(0);
            original.YarnVariables["bool_var"] = true;
            original.YarnVariables["string_var"] = "hello";
            original.YarnVariables["float_var"] = 1.5f;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(original);
            SaveData deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(json);

            // Newtonsoft.Json deserializes object values as JToken
            var boolVal = deserialized.YarnVariables["bool_var"];
            var stringVal = deserialized.YarnVariables["string_var"];
            var floatVal = deserialized.YarnVariables["float_var"];

            Assert.IsNotNull(boolVal);
            Assert.IsNotNull(stringVal);
            Assert.IsNotNull(floatVal);

            // Values are deserialized as JToken; cast before assertion.
            if (boolVal is Newtonsoft.Json.Linq.JToken boolToken)
            {
                Assert.AreEqual(true, boolToken.ToObject<bool>());
            }
            if (stringVal is Newtonsoft.Json.Linq.JToken stringToken)
            {
                Assert.AreEqual("hello", stringToken.ToObject<string>());
            }
            if (floatVal is Newtonsoft.Json.Linq.JToken floatToken)
            {
                Assert.AreEqual(1.5f, floatToken.ToObject<float>(), 0.001f);
            }
        }
        #endregion

        #region CharacterProfile Tests
        private CharacterProfile CreateCharacterProfile(string characterID, string displayName, Color themeColor, bool isPlayer)
        {
            CharacterProfile profile = ScriptableObject.CreateInstance<CharacterProfile>();
            SerializedObject so = new SerializedObject(profile);
            so.FindProperty("m_CharacterID").stringValue = characterID;
            so.FindProperty("m_DisplayName").stringValue = displayName;
            so.FindProperty("m_ThemeColor").colorValue = themeColor;
            so.FindProperty("m_IsPlayer").boolValue = isPlayer;
            so.ApplyModifiedPropertiesWithoutUndo();
            return profile;
        }

        [Test]
        public void CharacterProfile_IsValid_ReturnsTrueForValidProfile()
        {
            CharacterProfile profile = CreateCharacterProfile("player", "Player", Color.blue, true);

            Assert.IsTrue(profile.IsValid());
            Assert.AreEqual("player", profile.CharacterID);
            Assert.AreEqual("Player", profile.DisplayName);
            Assert.IsTrue(profile.IsPlayer);
        }

        [Test]
        public void CharacterProfile_IsValid_ReturnsFalseForEmptyID()
        {
            CharacterProfile profile = CreateCharacterProfile("", "Name", Color.white, false);

            Assert.IsFalse(profile.IsValid());
        }

        [Test]
        public void CharacterProfile_IsValid_ReturnsFalseForEmptyName()
        {
            CharacterProfile profile = CreateCharacterProfile("id", "", Color.white, false);

            Assert.IsFalse(profile.IsValid());
        }

        [Test]
        public void CharacterProfile_ThemeColor_ReturnsSetValue()
        {
            Color expected = new Color(0.2f, 0.6f, 1.0f);
            CharacterProfile profile = CreateCharacterProfile("test", "Test", expected, false);

            Assert.AreEqual(expected, profile.ThemeColor);
        }
        #endregion

        #region CharacterDatabase Tests
        private GameObject m_DatabaseObject;
        private CharacterDatabase m_Database;

        private void SetupCharacterDatabase(params CharacterProfile[] profiles)
        {
            m_DatabaseObject = new GameObject("CharacterDatabase");
            m_Database = m_DatabaseObject.AddComponent<CharacterDatabase>();

            // Force fallback path to avoid loading Resources in unit tests.
            SerializedObject so = new SerializedObject(m_Database);
            so.FindProperty("m_LoadPath").stringValue = "__test_nonexistent__";
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private void TeardownCharacterDatabase()
        {
            if (m_DatabaseObject != null)
            {
                Object.DestroyImmediate(m_DatabaseObject);
            }
        }

        [Test]
        public void CharacterDatabase_GetProfile_ReturnsNullForUnknownID()
        {
            SetupCharacterDatabase();

            Assert.IsNull(m_Database.GetProfile("nonexistent"));
            Assert.AreEqual(0, m_Database.ProfileCount);

            TeardownCharacterDatabase();
        }

        [Test]
        public void CharacterDatabase_IsPlayer_FallbackForUnknownID()
        {
            SetupCharacterDatabase();

            // "player" 縺ｨ縺・≧ ID 縺ｯ繝励Ο繝輔ぃ繧､繝ｫ縺ｪ縺励〒繧ゅヵ繧ｩ繝ｼ繝ｫ繝舌ャ繧ｯ縺ｧ true 繧定ｿ斐☆
            Assert.IsTrue(m_Database.IsPlayer("player"));
            Assert.IsFalse(m_Database.IsPlayer("npc_001"));

            TeardownCharacterDatabase();
        }

        [Test]
        public void CharacterDatabase_GetThemeColor_ReturnsFallbackColors()
        {
            SetupCharacterDatabase();

            Color playerColor = m_Database.GetThemeColor("player");
            Color npcColor = m_Database.GetThemeColor("npc_001");

            // 繝・ヵ繧ｩ繝ｫ繝医・繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ繧ｫ繝ｩ繝ｼ縺瑚ｿ斐ｋ
            Assert.AreEqual(new Color(0.2f, 0.6f, 1.0f), playerColor);
            Assert.AreEqual(new Color(0.85f, 0.85f, 0.85f), npcColor);

            TeardownCharacterDatabase();
        }

        [Test]
        public void CharacterDatabase_GetDisplayName_FallbackReturnsID()
        {
            SetupCharacterDatabase();

            // 繝励Ο繝輔ぃ繧､繝ｫ縺瑚ｦ九▽縺九ｉ縺ｪ縺・ｴ蜷医！D縺後◎縺ｮ縺ｾ縺ｾ霑斐ｋ
            Assert.AreEqual("unknown_char", m_Database.GetDisplayName("unknown_char"));

            TeardownCharacterDatabase();
        }
        #endregion
    }
}




