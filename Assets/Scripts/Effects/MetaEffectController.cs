using UnityEngine;
using System.Collections;

namespace ProjectFoundPhone.Effects
{
    /// <summary>
    /// メタ演出（グリッチ効果等）を制御するコントローラー
    /// シングルトンパターンで実装され、ScenarioManagerから呼び出される
    /// </summary>
    public class MetaEffectController : MonoBehaviour
    {
        #region Singleton
        private static MetaEffectController s_Instance;

        /// <summary>
        /// シングルトンインスタンスへのアクセス
        /// インスタンスが存在しない場合は自動的に作成する
        /// </summary>
        public static MetaEffectController Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    // 既存のインスタンスを検索
                    s_Instance = FindFirstObjectByType<MetaEffectController>();

                    // 見つからない場合は新規作成
                    if (s_Instance == null)
                    {
                        GameObject controllerObject = new GameObject("MetaEffectController");
                        s_Instance = controllerObject.AddComponent<MetaEffectController>();
                        DontDestroyOnLoad(controllerObject);
                    }
                }

                return s_Instance;
            }
        }
        #endregion

        #region Private Fields
        [SerializeField] private GlitchEffect m_GlitchEffect;
        [SerializeField] private Canvas m_EffectCanvas;
        [SerializeField] private int m_GlitchEffectLayer = 100; // 最前面に表示するためのレイヤー
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // シングルトンの初期化
            if (s_Instance == null)
            {
                s_Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeEffects();
            }
            else if (s_Instance != this)
            {
                // 既にインスタンスが存在する場合は破棄
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// エフェクトシステムの初期化
        /// </summary>
        private void InitializeEffects()
        {
            // エフェクト用のCanvasを作成
            if (m_EffectCanvas == null)
            {
                GameObject canvasObject = new GameObject("EffectCanvas");
                canvasObject.transform.SetParent(transform);
                m_EffectCanvas = canvasObject.AddComponent<Canvas>();
                m_EffectCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                m_EffectCanvas.sortingOrder = m_GlitchEffectLayer;

                // CanvasScalerを追加して画面サイズに対応
                UnityEngine.UI.CanvasScaler scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                // GraphicRaycasterを追加（必要に応じて）
                canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }

            // GlitchEffectコンポーネントを初期化
            if (m_GlitchEffect == null)
            {
                GameObject glitchObject = new GameObject("GlitchEffect");
                glitchObject.transform.SetParent(m_EffectCanvas.transform, false);
                m_GlitchEffect = glitchObject.AddComponent<GlitchEffect>();
                m_GlitchEffect.Initialize(m_EffectCanvas);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// グリッチ効果を再生
        /// </summary>
        /// <param name="level">グリッチの強度レベル（1-5）</param>
        /// <param name="duration">効果の持続時間（秒）。0以下の場合は自動的に計算される</param>
        public void PlayGlitchEffect(int level, float duration = 0f)
        {
            if (m_GlitchEffect == null)
            {
                Debug.LogWarning("MetaEffectController: GlitchEffect is not initialized.");
                InitializeEffects();
            }

            // レベルを1-5の範囲にクランプ
            int clampedLevel = Mathf.Clamp(level, 1, 5);

            // デフォルトの持続時間を計算（レベルに応じて）
            if (duration <= 0f)
            {
                duration = CalculateDefaultDuration(clampedLevel);
            }

            m_GlitchEffect?.PlayGlitch(clampedLevel, duration);
        }

        /// <summary>
        /// グリッチ効果を停止
        /// </summary>
        public void StopGlitchEffect()
        {
            m_GlitchEffect?.StopGlitch();
        }

        /// <summary>
        /// レベルに応じたデフォルトの持続時間を計算
        /// </summary>
        /// <param name="level">グリッチレベル（1-5）</param>
        /// <returns>持続時間（秒）</returns>
        private float CalculateDefaultDuration(int level)
        {
            // レベルが高いほど長く表示
            return 0.2f + (level * 0.1f);
        }
        #endregion
    }
}
