using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// UI 全体のフォントサイズ階層を一元管理する ScriptableObject。
    /// 意味論的な7段階で定義し、各 UI コントローラーから参照する。
    /// チャット固有の設定は ChatUIConfig が引き続き管理する。
    /// </summary>
    [CreateAssetMenu(fileName = "UIFontConfig", menuName = "ProjectFoundPhone/UI Font Config")]
    public class UIFontConfig : ScriptableObject
    {
        [Header("Font Size Hierarchy")]
        [Tooltip("画面タイトル (例: HALLUCINATION SIMULATOR)")]
        public float titleFontSize = 28f;

        [Tooltip("セクション見出し、カードタイトル (例: チャンネル名、フラグメント名)")]
        public float headingFontSize = 22f;

        [Tooltip("強調本文、アクションボタン (例: HalluciCoin、確認ボタン、トピックタイトル)")]
        public float subheadingFontSize = 20f;

        [Tooltip("本文、ボタンテキスト、ラベル (例: タブボタン、戻るボタン)")]
        public float bodyFontSize = 20f;

        [Tooltip("説明文、サブタイトル (例: チャンネル説明、サブタブ)")]
        public float captionFontSize = 18f;

        [Tooltip("ステータス、バッジ、パーセント表示")]
        public float smallFontSize = 14f;

        [Tooltip("ミニ統計、ヒントテキスト")]
        public float tinyFontSize = 11f;

        [Header("Responsive Scaling")]
        [Tooltip("レスポンシブスケーリングが効き始める Canvas 幅 (px)")]
        public float responsiveBreakpoint = 900f;

        [Tooltip("最小スケール倍率 (Canvas 幅が狭いとき)")]
        [Range(0.5f, 1f)]
        public float responsiveMinScale = 0.78f;

        /// <summary>
        /// Canvas 幅に基づくレスポンシブフォントスケールを計算する。
        /// breakpoint 以上なら 1.0、未満なら線形補間で minScale まで縮小。
        /// </summary>
        public float GetResponsiveScale(float canvasWidth)
        {
            if (canvasWidth >= responsiveBreakpoint) return 1f;
            return Mathf.Clamp(canvasWidth / responsiveBreakpoint, responsiveMinScale, 1f);
        }

        private static UIFontConfig s_Instance;
        public static UIFontConfig Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = Resources.Load<UIFontConfig>("UIFontConfig");
                    if (s_Instance == null)
                    {
                        s_Instance = CreateInstance<UIFontConfig>();
                    }
                }
                return s_Instance;
            }
        }
    }
}
