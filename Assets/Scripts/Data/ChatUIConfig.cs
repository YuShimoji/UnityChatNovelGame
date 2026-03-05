using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// チャット UI の表示設定を一元管理する ScriptableObject。
    /// Inspector 上で全値を確認・調整でき、ハードコードを排除する。
    /// </summary>
    [CreateAssetMenu(fileName = "ChatUIConfig", menuName = "ProjectFoundPhone/Chat UI Config")]
    public class ChatUIConfig : ScriptableObject
    {
        [Header("Message Bubble")]
        [Tooltip("通常メッセージのフォントサイズ")]
        public float messageFontSize = 28f;

        [Tooltip("バブルの最小高さ")]
        public float bubbleMinHeight = 60f;

        [Tooltip("バブル内テキストの上下パディング")]
        public float bubbleTextPadding = 20f;

        [Tooltip("バブルの初期高さ (RectTransform)")]
        public float bubbleInitialHeight = 72f;

        [Tooltip("プレイヤーのテキスト色")]
        public Color playerTextColor = Color.white;

        [Tooltip("NPC のテキスト色")]
        public Color npcTextColor = new Color(0.9f, 0.9f, 0.9f, 1f);

        [Tooltip("バブル出現アニメーション時間 (秒)")]
        public float bubbleAnimationDuration = 0.4f;

        [Header("Layout")]
        [Tooltip("サイドマージン (画面幅に対する割合 0.0-0.5)")]
        [Range(0f, 0.5f)]
        public float sideMarginPercent = 0.25f;

        [Tooltip("サイドマージンの最大ピクセル値 (狭い画面での崩れ防止)")]
        public float sideMarginMaxPx = 200f;

        [Tooltip("サイドマージンの画面幅に対する最大割合 (モバイル対応)")]
        [Range(0f, 0.3f)]
        public float sideMarginMaxRatio = 0.18f;

        [Tooltip("ラッパーの端パディング (px)")]
        public int wrapperEdgePadding = 12;

        [Tooltip("ラッパーの上下パディング (px)")]
        public int wrapperVerticalPadding = 4;

        [Tooltip("VLG 最小スペーシング")]
        public float minLayoutSpacing = 10f;

        [Header("Character Icon")]
        [Tooltip("キャラクターアイコンを表示するか")]
        public bool showCharacterIcon = true;

        [Tooltip("キャラクターアイコンのサイズ (px)")]
        public float characterIconSize = 40f;

        [Tooltip("アイコンとバブルの間隔 (px)")]
        public float iconBubbleSpacing = 8f;

        [Header("System Message")]
        [Tooltip("システムメッセージのフォントサイズ")]
        public float systemMessageFontSize = 16f;

        [Tooltip("システムメッセージのテキスト色")]
        public Color systemMessageTextColor = new Color(0.75f, 0.75f, 0.8f, 1f);

        [Tooltip("システムメッセージの背景色")]
        public Color systemMessageBgColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

        [Tooltip("システムメッセージの最小高さ")]
        public float systemMessageMinHeight = 40f;

        [Header("Choice")]
        [Tooltip("選択肢ボタンの背景色")]
        public Color choiceButtonColor = new Color(0.22f, 0.25f, 0.35f, 0.85f);

        [Tooltip("選択肢ボタンのハイライト色")]
        public Color choiceButtonHighlightColor = new Color(0.3f, 0.35f, 0.5f, 0.95f);

        [Tooltip("選択肢ボタンの押下色")]
        public Color choiceButtonPressedColor = new Color(0.2f, 0.28f, 0.45f, 1.0f);

        [Tooltip("選択肢テキストの色")]
        public Color choiceTextColor = new Color(0.82f, 0.85f, 0.95f, 1f);

        [Tooltip("選択肢の最小フォントサイズ")]
        public float choiceFontSizeMin = 18f;

        [Tooltip("選択肢の最大フォントサイズ")]
        public float choiceFontSizeMax = 26f;

        [Tooltip("選択肢ボタンの最小高さ")]
        public float choiceButtonMinHeight = 42f;

        [Tooltip("選択肢ボタンの推奨高さ")]
        public float choiceButtonPreferredHeight = 50f;

        [Tooltip("選択肢コンテナのスペーシング")]
        public float choiceSpacing = 6f;

        [Tooltip("選択肢コンテナの左右パディング")]
        public int choicePaddingHorizontal = 20;

        [Header("Typing Indicator")]
        [Tooltip("入力中表示の背景色")]
        public Color typingIndicatorColor = new Color(0.3f, 0.3f, 0.35f, 0.9f);

        [Tooltip("入力中表示のテキスト色")]
        public Color typingIndicatorTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);

        [Tooltip("入力中表示のフォントサイズ")]
        public float typingIndicatorFontSize = 18f;

        [Header("Scroll")]
        [Tooltip("自動スクロールの起動遅延 (秒)")]
        public float autoScrollDelay = 0.1f;

        [Header("Image")]
        [Tooltip("画像フェードイン時間 (秒)")]
        public float imageFadeInDuration = 0.6f;

        /// <summary>
        /// Resources から ChatUIConfig をロード (singleton 風アクセス)。
        /// アセット未配置時はデフォルト値のランタイムインスタンスを返す（null にならない）。
        /// </summary>
        private static ChatUIConfig s_Instance;
        public static ChatUIConfig Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = Resources.Load<ChatUIConfig>("ChatUIConfig");
                    if (s_Instance == null)
                    {
                        s_Instance = CreateInstance<ChatUIConfig>();
                    }
                }
                return s_Instance;
            }
        }
    }
}
