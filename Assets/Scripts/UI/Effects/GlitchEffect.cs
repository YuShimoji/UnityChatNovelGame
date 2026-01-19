using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// UI上にグリッチエフェクトを表示するコンポーネント。
    /// RectTransformのシェイクとノイズオーバーレイ画像で演出を行う。
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

            // 初期状態でオーバーレイを非表示
            SetOverlaysActive(false);
        }

        private void OnDestroy()
        {
            KillAllTweens();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// グリッチエフェクトを再生する。
        /// </summary>
        /// <param name="level">強度レベル (0: 停止, 1-3: 弱-強)</param>
        /// <param name="duration">持続時間（秒）。0以下で無限</param>
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

            // レベルに応じた設定
            float shakeStrength = m_ShakeStrengthBase * level;
            Color noiseColor = GetNoiseColorForLevel(level);
            float aberrationAlpha = level >= 2 ? m_AberrationColor.a * level : 0f;

            // シェイクアニメーション
            if (m_ShakeTarget != null)
            {
                m_ShakeTween = m_ShakeTarget
                    .DOShakeAnchorPos(duration > 0 ? duration : 10f, shakeStrength, m_ShakeVibrato, m_ShakeRandomness)
                    .SetLoops(duration <= 0 ? -1 : 1, LoopType.Restart)
                    .SetEase(Ease.Linear);
            }

            // ノイズオーバーレイのフェードイン＆点滅
            if (m_NoiseOverlay != null)
            {
                m_NoiseOverlay.color = new Color(noiseColor.r, noiseColor.g, noiseColor.b, 0f);
                m_NoiseTween = m_NoiseOverlay
                    .DOFade(noiseColor.a, 0.1f)
                    .OnComplete(() =>
                    {
                        // 点滅効果
                        m_NoiseTween = m_NoiseOverlay
                            .DOFade(noiseColor.a * 0.5f, 0.05f)
                            .SetLoops(-1, LoopType.Yoyo);
                    });
            }

            // 色収差オーバーレイ (Level 2以上)
            if (m_ColorAberrationOverlay != null && level >= 2)
            {
                Color aberColor = new Color(m_AberrationColor.r, m_AberrationColor.g, m_AberrationColor.b, 0f);
                m_ColorAberrationOverlay.color = aberColor;
                m_AberrationTween = m_ColorAberrationOverlay
                    .DOFade(aberrationAlpha, 0.2f)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            // 自動停止
            if (duration > 0)
            {
                m_StopDelayTween = DOVirtual.DelayedCall(duration, Stop);
            }
        }

        /// <summary>
        /// グリッチエフェクトを停止する。
        /// </summary>
        public void Stop()
        {
            if (!m_IsPlaying) return;

            m_IsPlaying = false;
            KillAllTweens();

            // 元の位置に戻す
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
