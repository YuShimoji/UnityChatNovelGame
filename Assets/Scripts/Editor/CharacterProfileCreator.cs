using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.EditorTools
{
    public static class CharacterProfileCreator
    {
        private const string BasePath = "Assets/Resources/Characters/";

        [MenuItem("Tools/FoundPhone/Create Default Character Profiles")]
        public static void CreateDefaultProfiles()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Characters"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Characters");
            }

            int created = 0;

            created += CreateProfile(
                fileName: "CharacterProfile_Player",
                characterID: "player",
                displayName: "あなた",
                themeColor: new Color(0.2f, 0.6f, 1.0f),
                isPlayer: true
            );

            created += CreateProfile(
                fileName: "CharacterProfile_NPC_Unknown",
                characterID: "unknown",
                displayName: "不明な連絡先",
                themeColor: new Color(0.85f, 0.85f, 0.85f),
                isPlayer: false
            );

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[CharacterProfileCreator] Complete. Created {created} profile(s).");
        }

        private static int CreateProfile(string fileName, string characterID, string displayName, Color themeColor, bool isPlayer)
        {
            string path = BasePath + fileName + ".asset";

            if (AssetDatabase.LoadAssetAtPath<CharacterProfile>(path) != null)
            {
                Debug.Log($"[CharacterProfileCreator] Skipped '{fileName}' (already exists).");
                return 0;
            }

            CharacterProfile profile = ScriptableObject.CreateInstance<CharacterProfile>();
            SerializedObject so = new SerializedObject(profile);
            so.FindProperty("m_CharacterID").stringValue = characterID;
            so.FindProperty("m_DisplayName").stringValue = displayName;
            so.FindProperty("m_ThemeColor").colorValue = themeColor;
            so.FindProperty("m_IsPlayer").boolValue = isPlayer;
            so.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(profile, path);
            Debug.Log($"[CharacterProfileCreator] Created '{fileName}' at {path}");
            return 1;
        }
    }
}
