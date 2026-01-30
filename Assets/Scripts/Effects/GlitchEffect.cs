using UnityEngine;
using UnityEngine.UI;

namespace ProjectFoundPhone.Effects
{
    /// <summary>
    /// 画面全体にノイズとグリッチ効果を表示するコンポーネント
    /// UI Imageを使用して画面全体を覆うように配置する
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class GlitchEffect : MonoBehaviour
    {
        #region Constants
        private const int c_MinLevel = 1;
        private const int c_MaxLevel = 5;
        private const float c_BaseNoiseIntensity = 0.1f;
        private const float c_BaseChromaticAberration = 0.01f;
        private const float c_BaseScanlineSpeed = 2f;
        #endregion

        [Header("Settings")]
        [SerializeField] private Material m_GlitchMaterial;

        private Image m_TargetImage;

        private void Awake()
        {
            m_TargetImage = GetComponent<Image>();
        }

        public void SetIntensity(float intensity)
        {
            if (m_TargetImage != null && m_TargetImage.material != null)
            {
                // シェーダープロパティが存在する場合のみ設定
                // _GlitchIntensity は仮のプロパティ名
                if (m_TargetImage.material.HasProperty("_GlitchIntensity"))
                {
                    m_TargetImage.material.SetFloat("_GlitchIntensity", intensity);
                }
            }
        }
    }
}
