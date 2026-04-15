using UnityEngine;
using TMPro;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// バブル (吹き出し) の見た目を定義する ScriptableObject。
    /// Yarn の &lt;&lt;BubbleStyle "presetId"&gt;&gt; で次の 1 メッセージに適用される。
    /// 各フィールドは "既定値 = 上書きしない" を意味する (仕様 SP-023 §3.2)。
    /// </summary>
    [CreateAssetMenu(fileName = "BubbleStylePreset_New", menuName = "ProjectFoundPhone/Bubble Style Preset", order = 10)]
    public class BubbleStylePreset : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Yarn から参照する ID (例: narration, whisper, shout)")]
        public string presetId;

        [Tooltip("Editor 表示名")]
        public string displayName;

        [Header("Background")]
        [Tooltip("背景色を上書きする")]
        public bool overrideBackgroundColor = false;

        [Tooltip("バブル背景色")]
        public Color backgroundColor = Color.white;

        [Tooltip("背景の不透明度 (0 = 地の文スタイル)")]
        [Range(0f, 1f)]
        public float backgroundAlpha = 1f;

        [Tooltip("9-slice スプライト (null = ChatUIConfig 既定)")]
        public Sprite bubbleSprite;

        [Tooltip("自動生成角丸の半径 (負値 = ChatUIConfig 既定)")]
        public float cornerRadius = -1f;

        [Header("Shadow")]
        [Tooltip("影設定を上書きする")]
        public bool overrideShadow = false;

        [Tooltip("影を付けるか")]
        public bool shadowEnabled = true;

        [Tooltip("影の色")]
        public Color shadowColor = new Color(0f, 0f, 0f, 0.3f);

        [Tooltip("影のオフセット")]
        public Vector2 shadowDistance = new Vector2(2f, -2f);

        [Header("Text")]
        [Tooltip("テキスト色を上書きする")]
        public bool overrideTextColor = false;

        [Tooltip("テキスト色")]
        public Color textColor = Color.white;

        [Tooltip("フォントサイズ (負値 = ChatUIConfig 既定)")]
        public float fontSize = -1f;

        [Tooltip("フォントスタイル")]
        public FontStyles fontStyle = FontStyles.Normal;

        [Tooltip("テキスト揃え")]
        public TextAlignmentOptions textAlignment = TextAlignmentOptions.TopLeft;

        [Header("Layout")]
        [Tooltip("バブル内パディング (負値 = ChatUIConfig 既定)")]
        public int paddingLeft = -1;
        public int paddingRight = -1;
        public int paddingTop = -1;
        public int paddingBottom = -1;

        [Header("Behavior")]
        [Tooltip("キャラクターラッパー・アイコンを描画しない (地の文などで全幅表示にする)")]
        public bool suppressWrapper = false;
    }
}
