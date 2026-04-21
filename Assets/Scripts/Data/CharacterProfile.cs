using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// キャラクターの表示モード（バブル横のアイコン/名前表示制御）
    /// </summary>
    public enum CharacterDisplayMode
    {
        NameOnly,
        IconOnly,
        IconAndName
    }

    /// <summary>
    /// アイコンの表示位置（SP-023: バブルの反転は行わない）
    /// </summary>
    public enum IconSide
    {
        /// <summary>IsPlayer に基づく自動判定 (Player=Right, NPC=Left)</summary>
        Auto,
        /// <summary>常にバブルの左側</summary>
        Left,
        /// <summary>常にバブルの右側</summary>
        Right
    }

    /// <summary>
    /// NPC 発話前のタイピングインジケーター速度プリセット (SP-024 S3)
    /// </summary>
    public enum TypingSpeed
    {
        Default,
        Instant,
        Fast,
        Normal,
        Slow,
        VerySlow,
        Custom
    }

    /// <summary>
    /// 将来の通信状態表示に使う既定ステータス (SP-024 S4 のデータ契約先行)
    /// </summary>
    public enum OnlineStatus
    {
        Online,
        Away,
        Offline,
        Hidden
    }

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
        [Tooltip("基底アイコン。感情固定ではなく、状態/通信状態/更新状態などの派生表現の起点として扱う")]
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private Color m_ThemeColor = Color.white;
        [SerializeField] private bool m_IsPlayer;
        [SerializeField] private CharacterDisplayMode m_DisplayMode = CharacterDisplayMode.NameOnly;

        [Tooltip("アイコンの左右配置 (Auto = IsPlayer で自動判定)")]
        [SerializeField] private IconSide m_IconSide = IconSide.Auto;

        [Tooltip("キャラクターの既定バブルスタイル。未指定時はテーマカラー由来の通常表示を使う")]
        [SerializeField] private BubbleStylePreset m_DefaultBubbleStylePreset;

        [Tooltip("NPC 発話前のタイピングインジケーター速度。Default = ChatUIConfig 既定")]
        [SerializeField] private TypingSpeed m_TypingSpeed = TypingSpeed.Default;

        [Tooltip("typingSpeed = Custom のときに使う秒数")]
        [SerializeField] private float m_CustomTypingDelay = 0f;

        [Tooltip("将来の状態表示用の既定通信状態。現時点ではデータ契約のみ先行")]
        [SerializeField] private OnlineStatus m_DefaultOnlineStatus = OnlineStatus.Online;
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

        /// <summary>
        /// バブル横の表示モード（アイコンのみ/アイコン+名前/名前のみ）
        /// </summary>
        public CharacterDisplayMode DisplayMode => m_DisplayMode;

        /// <summary>
        /// アイコンの表示位置 (Auto/Left/Right)
        /// </summary>
        public IconSide IconSide => m_IconSide;

        /// <summary>
        /// キャラクターの既定バブルスタイル
        /// </summary>
        public BubbleStylePreset DefaultBubbleStylePreset => m_DefaultBubbleStylePreset;

        /// <summary>
        /// NPC 発話前のタイピングインジケーター速度
        /// </summary>
        public TypingSpeed TypingSpeed => m_TypingSpeed;

        /// <summary>
        /// typingSpeed = Custom のときの秒数
        /// </summary>
        public float CustomTypingDelay => m_CustomTypingDelay;

        /// <summary>
        /// 将来の通信状態表示用の既定ステータス
        /// </summary>
        public OnlineStatus DefaultOnlineStatus => m_DefaultOnlineStatus;
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
