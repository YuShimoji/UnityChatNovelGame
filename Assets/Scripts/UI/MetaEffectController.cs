using UnityEngine;
using DG.Tweening;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 画面全体に影響を与える演出エフェクト(グリッチ、フェード、ノイズ等)を管理するコントローラー。
    /// ScenarioManagerのGlitchCommandから呼び出される。
    /// </summary>
    public class MetaEffectController : MonoBehaviour
    {
        #region Singleton
        private static MetaEffectController s_Instance;
        public static MetaEffectController Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<MetaEffectController>();
                    if (s_Instance == null)
                    {
                        Debug.LogWarning("MetaEffectController: Instance not found in scene.");
                    }
                }
                return s_Instance;
            }
        }
        #endregion

        #region Serialized Fields
        [SerializeField] private GlitchEffect m_GlitchEffect;
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

            if (m_GlitchEffect == null)
            {
                m_GlitchEffect = GetComponentInChildren<GlitchEffect>();
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

        #region Public Methods
        /// <summary>
        /// グリッチエフェクトを再生する。
        /// </summary>
        /// <param name="level">エフェクトの強度レベル (0-3程度)</param>
        /// <param name="duration">エフェクトの持続時間（秒）。0以下の場合は無限</param>
        public void PlayGlitch(int level, float duration = 2.0f)
        {
            if (m_GlitchEffect == null)
            {
                Debug.LogWarning("MetaEffectController: GlitchEffect component is not assigned.");
                return;
            }

            m_GlitchEffect.Play(level, duration);
            Debug.Log($"MetaEffectController: PlayGlitch - Level: {level}, Duration: {duration}s");
        }

        /// <summary>
        /// 現在再生中のエフェクトを停止する。
        /// </summary>
        public void StopEffect()
        {
            if (m_GlitchEffect != null)
            {
                m_GlitchEffect.Stop();
            }
            Debug.Log("MetaEffectController: StopEffect called.");
        }
        #endregion
    }
}
