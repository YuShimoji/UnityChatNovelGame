using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.Data;

// Avoid namespace conflict with ProjectFoundPhone.Debug
using Debug = UnityEngine.Debug;

namespace ProjectFoundPhone.EditorTools
{
    public static class RecipeAssetCreator
    {
        [MenuItem("Tools/FoundPhone/Create Test Recipe")]
        public static void CreateTestRecipe()
        {
            // Load Ingredients (Using available topics from file list)
            // Files found: topic_found_phone, topic_suspicious_message, topic_missing_person
            TopicData ingredientA = Resources.Load<TopicData>("Topics/topic_found_phone");
            TopicData ingredientB = Resources.Load<TopicData>("Topics/topic_suspicious_message");
            TopicData result = Resources.Load<TopicData>("Topics/topic_missing_person");

            if (ingredientA == null) { Debug.LogError("Missing topic_found_phone"); return; }
            if (ingredientB == null) { Debug.LogError("Missing topic_suspicious_message"); return; }
            if (result == null) { Debug.LogError("Missing topic_missing_person"); return; }

            // Create Recipe
            SynthesisRecipe recipe = ScriptableObject.CreateInstance<SynthesisRecipe>();
            
            // Set fields via SerializedObject since they are private
            SerializedObject so = new SerializedObject(recipe);
            so.FindProperty("m_IngredientA").objectReferenceValue = ingredientA;
            so.FindProperty("m_IngredientB").objectReferenceValue = ingredientB;
            so.FindProperty("m_Result").objectReferenceValue = result;
            so.ApplyModifiedProperties();

            string path = "Assets/Resources/Recipes/Recipe_Test_Phone_Message.asset";
            AssetDatabase.CreateAsset(recipe, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[RecipeCreator] Created recipe at {path}");
        }
    }
}
