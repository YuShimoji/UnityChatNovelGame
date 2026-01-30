using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// UI荳翫↓繧ｰ繝ｪ繝・メ繧ｨ繝輔ぉ繧ｯ繝医ｒ陦ｨ遉ｺ縺吶ｋ繧ｳ繝ｳ繝昴・繝阪Φ繝医・
    /// RectTransform縺ｮ繧ｷ繧ｧ繧､繧ｯ縺ｨ繝弱う繧ｺ繧ｪ繝ｼ繝舌・繝ｬ繧､逕ｻ蜒上〒貍泌・繧定｡後≧縲・
    /// </summary>
    public class GlitchEffect : MonoBehaviour
    {
        #region Serialized Fields
        [Header("References")]
        [SerializeField] private RectTransform m_ShakeTarget;
        [SerializeField] private Image m_NoiseOverlay;
        [SerializeField] private Image m_ColorAberrationOverlay;

        [Header("Settings")]
        [SerializeField] private float m_ShakeStrengthBase = 10f;
        [SerializeField] private int m_ShakeVibrato = 10;
        [SerializeField] private float m_ShakeRandomness = 90f;
        [SerializeField] private Color m_NoiseColorLevel1 = new Color(1f, 1f, 1f, 0.05f);
        [SerializeField] private Color m_NoiseColorLevel2 = new Color(1f, 1f, 1f, 0.15f);
        [SerializeField] private Color m_NoiseColorLevel3 = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color m_AberrationColor = new Color(1f, 0f, 0.5f, 0.1f);
        #endregion

        #region Private Fields
        private Tween m_ShakeTween;
        private Tween m_NoiseTween;
        private Tween m_AberrationTween;
        private Tween m_StopDelayTween;
        private Vector2 m_OriginalPosition;
        private bool m_IsPlaying = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (m_ShakeTarget == null)
            {
                m_ShakeTarget = GetComponent<RectTransform>();
            }

            if (m_ShakeTarget != null)
            {
                m_OriginalPosition = m_ShakeTarget.anchoredPosition;
            }

            // 蛻晄悄迥ｶ諷九〒繧ｪ繝ｼ繝舌・繝ｬ繧､繧帝撼陦ｨ遉ｺ
            SetOverlaysActive(false);
        }

        private void OnDestroy()
        {
            KillAllTweens();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 繧ｰ繝ｪ繝・メ繧ｨ繝輔ぉ繧ｯ繝医ｒ蜀咲函縺吶ｋ縲・
        /// </summary>
        /// <param name="level">蠑ｷ蠎ｦ繝ｬ繝吶Ν (0: 蛛懈ｭ｢, 1-3: 蠑ｱ-蠑ｷ)</param>
        /// <param name="duration">謖∫ｶ壽凾髢難ｼ育ｧ抵ｼ峨・莉･荳九〒辟｡髯・/param>
        public void Play(int level, float duration = 2.0f)
        {
            if (level <= 0)
            {
                Stop();
                return;
            }

            m_IsPlaying = true;
            KillAllTweens();
            SetOverlaysActive(true);

            // 繝ｬ繝吶Ν縺ｫ蠢懊§縺溯ｨｭ螳・
            float shakeStrength = m_ShakeStrengthBase * level;
            Color noiseColor = GetNoiseColorForLevel(level);
            float aberrationAlpha = level >= 2 ? m_AberrationColor.a * level : 0f;

            // 繧ｷ繧ｧ繧､繧ｯ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ
            if (m_ShakeTarget != null)
            {
                m_ShakeTween = m_ShakeTarget
                    .DOShakeAnchorPos(duration > 0 ? duration : 10f, shakeStrength, m_ShakeVibrato, m_ShakeRandomness)
                    .SetLoops(duration <= 0 ? -1 : 1, LoopType.Restart)
                    .SetEase(Ease.Linear);
            }

            // 繝弱う繧ｺ繧ｪ繝ｼ繝舌・繝ｬ繧､縺ｮ繝輔ぉ繝ｼ繝峨う繝ｳ・・せ貊・
            if (m_NoiseOverlay != null)
            {
                m_NoiseOverlay.color = new Color(noiseColor.r, noiseColor.g, noiseColor.b, 0f);
                m_NoiseTween = m_NoiseOverlay
                    .DOFade(noiseColor.a, 0.1f)
                    .OnComplete(() =>
                    {
                        // 轤ｹ貊・柑譫・
                        m_NoiseTween = m_NoiseOverlay
                            .DOFade(noiseColor.a * 0.5f, 0.05f)
                            .SetLoops(-1, LoopType.Yoyo);
                    });
            }

            // 濶ｲ蜿主ｷｮ繧ｪ繝ｼ繝舌・繝ｬ繧､ (Level 2莉･荳・
            if (m_ColorAberrationOverlay != null && level >= 2)
            {
                Color aberColor = new Color(m_AberrationColor.r, m_AberrationColor.g, m_AberrationColor.b, 0f);
                m_ColorAberrationOverlay.color = aberColor;
                m_AberrationTween = m_ColorAberrationOverlay
                    .DOFade(aberrationAlpha, 0.2f)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            // 閾ｪ蜍募●豁｢
            if (duration > 0)
            {
                m_StopDelayTween = DOVirtual.DelayedCall(duration, Stop);
            }
        }

        /// <summary>
        /// 繧ｰ繝ｪ繝・メ繧ｨ繝輔ぉ繧ｯ繝医ｒ蛛懈ｭ｢縺吶ｋ縲・
        /// </summary>
        public void Stop()
        {
            if (!m_IsPlaying) return;

            m_IsPlaying = false;
            KillAllTweens();

            // 蜈・・菴咲ｽｮ縺ｫ謌ｻ縺・
            if (m_ShakeTarget != null)
            {
                m_ShakeTarget.anchoredPosition = m_OriginalPosition;
            }

            SetOverlaysActive(false);
        }
        #endregion

        #region Private Methods
        private void KillAllTweens()
        {
            m_ShakeTween?.Kill();
            m_NoiseTween?.Kill();
            m_AberrationTween?.Kill();
            m_StopDelayTween?.Kill();

            m_ShakeTween = null;
            m_NoiseTween = null;
            m_AberrationTween = null;
            m_StopDelayTween = null;
        }

        private void SetOverlaysActive(bool active)
        {
            if (m_NoiseOverlay != null)
            {
                m_NoiseOverlay.gameObject.SetActive(active);
            }
            if (m_ColorAberrationOverlay != null)
            {
                m_ColorAberrationOverlay.gameObject.SetActive(active);
            }
        }

        private Color GetNoiseColorForLevel(int level)
        {
            switch (level)
            {
                case 1: return m_NoiseColorLevel1;
                case 2: return m_NoiseColorLevel2;
                case 3:
                default: return m_NoiseColorLevel3;
            }
        }
        #endregion
    }
}
