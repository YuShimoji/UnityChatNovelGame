using UnityEngine;
using System.Collections.Generic;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// CharacterProfileのデータベース
    /// Resources/Characters/ からプロファイルをロードし、IDで検索可能にする
    /// </summary>
    public class CharacterDatabase : MonoBehaviour
    {
        #region Singleton
        private static CharacterDatabase s_Instance;

        /// <summary>
        /// CharacterDatabaseのシングルトンインスタンス
        /// </summary>
        public static CharacterDatabase Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<CharacterDatabase>();
                }
                return s_Instance;
            }
        }
        #endregion

        #region Private Fields
        [Header("Settings")]
        [SerializeField] private string m_LoadPath = "Characters";

        [Header("Fallback")]
        [SerializeField] private Color m_DefaultPlayerColor = new Color(0.2f, 0.6f, 1.0f);
        [SerializeField] private Color m_DefaultNPCColor = new Color(0.85f, 0.85f, 0.85f);

        /// <summary>
        /// ID -> CharacterProfile のルックアップ辞書
        /// </summary>
        private Dictionary<string, CharacterProfile> m_Profiles = new Dictionary<string, CharacterProfile>();
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            s_Instance = this;
            LoadProfiles();
        }

        private void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// キャラクターIDからプロファイルを取得
        /// </summary>
        /// <param name="characterID">検索するキャラクターID</param>
        /// <returns>該当するプロファイル。見つからない場合null</returns>
        public CharacterProfile GetProfile(string characterID)
        {
            if (string.IsNullOrEmpty(characterID))
            {
                return null;
            }

            m_Profiles.TryGetValue(characterID, out CharacterProfile profile);
            return profile;
        }

        /// <summary>
        /// 指定IDがプレイヤーかどうかを判定
        /// プロファイルが存在しない場合は "player" という文字列で判定する
        /// </summary>
        /// <param name="characterID">判定するキャラクターID</param>
        /// <returns>プレイヤーの場合true</returns>
        public bool IsPlayer(string characterID)
        {
            CharacterProfile profile = GetProfile(characterID);
            if (profile != null)
            {
                return profile.IsPlayer;
            }
            return characterID == "player";
        }

        /// <summary>
        /// 指定IDのテーマカラーを取得
        /// プロファイルが存在しない場合はデフォルトカラーを返す
        /// </summary>
        /// <param name="characterID">キャラクターID</param>
        /// <returns>テーマカラー</returns>
        public Color GetThemeColor(string characterID)
        {
            CharacterProfile profile = GetProfile(characterID);
            if (profile != null)
            {
                return profile.ThemeColor;
            }
            return IsPlayer(characterID) ? m_DefaultPlayerColor : m_DefaultNPCColor;
        }

        /// <summary>
        /// 指定IDの表示名を取得
        /// プロファイルが存在しない場合はIDをそのまま返す
        /// </summary>
        /// <param name="characterID">キャラクターID</param>
        /// <returns>表示名</returns>
        public string GetDisplayName(string characterID)
        {
            CharacterProfile profile = GetProfile(characterID);
            if (profile != null)
            {
                return profile.DisplayName;
            }
            return characterID;
        }

        /// <summary>
        /// 指定IDのアイコンを取得
        /// </summary>
        /// <param name="characterID">キャラクターID</param>
        /// <returns>アイコンSprite。存在しない場合null</returns>
        public Sprite GetIcon(string characterID)
        {
            CharacterProfile profile = GetProfile(characterID);
            return profile?.Icon;
        }

        /// <summary>
        /// 登録されているプロファイル数を取得
        /// </summary>
        public int ProfileCount => m_Profiles.Count;
        #endregion

        #region Private Methods
        /// <summary>
        /// ResourcesフォルダからCharacterProfileをロードする
        /// </summary>
        private void LoadProfiles()
        {
            m_Profiles.Clear();
            CharacterProfile[] profiles = Resources.LoadAll<CharacterProfile>(m_LoadPath);

            if (profiles != null)
            {
                foreach (var profile in profiles)
                {
                    if (profile != null && !string.IsNullOrEmpty(profile.CharacterID))
                    {
                        if (m_Profiles.ContainsKey(profile.CharacterID))
                        {
                            Debug.LogWarning($"CharacterDatabase: Duplicate CharacterID '{profile.CharacterID}' found. Skipping.");
                            continue;
                        }
                        m_Profiles[profile.CharacterID] = profile;
                    }
                }
                Debug.Log($"CharacterDatabase: Loaded {m_Profiles.Count} character profiles.");
            }
        }
        #endregion
    }
}
