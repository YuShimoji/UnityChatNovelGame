using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// キャラクターの静的データを管理するScriptableObject
    /// Yarn Spinnerのキャラクター名と紐づけ、チャットUIの表示制御に使用する
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacterProfile", menuName = "Project FoundPhone/Character Profile", order = 0)]
    public class CharacterProfile : ScriptableObject
    {
        #region Private Fields
        [SerializeField] private string m_CharacterID;
        [SerializeField] private string m_DisplayName;
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private Color m_ThemeColor = Color.white;
        [SerializeField] private bool m_IsPlayer;
        #endregion

        #region Public Properties
        /// <summary>
        /// Yarn Spinnerで使用するキャラクターID
        /// </summary>
        public string CharacterID => m_CharacterID;

        /// <summary>
        /// UIに表示する名前
        /// </summary>
        public string DisplayName => m_DisplayName;

        /// <summary>
        /// キャラクターのアイコン画像
        /// </summary>
        public Sprite Icon => m_Icon;

        /// <summary>
        /// フキダシのベースカラー（白の9-Slice Spriteに乗算して着色）
        /// </summary>
        public Color ThemeColor => m_ThemeColor;

        /// <summary>
        /// プレイヤー自身かどうか（右寄せ/左寄せの判定に使用）
        /// </summary>
        public bool IsPlayer => m_IsPlayer;
        #endregion

        #region Public Methods
        /// <summary>
        /// プロファイルが有効かどうかを判定
        /// </summary>
        /// <returns>有効な場合true</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(m_CharacterID) && !string.IsNullOrEmpty(m_DisplayName);
        }
        #endregion
    }
}
