using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectFoundPhone.Effects
{
    /// <summary>
    /// 繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ螳溯｣・☆繧九さ繝ｳ繝昴・繝阪Φ繝・    /// UI Image繧剃ｽｿ逕ｨ縺励※逕ｻ髱｢蜈ｨ菴薙↓繝弱う繧ｺ縺ｨ繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ陦ｨ遉ｺ縺吶ｋ
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

        #region Private Fields
        private Image m_GlitchImage;
        private Canvas m_ParentCanvas;
        private Material m_GlitchMaterial;
        private Coroutine m_GlitchCoroutine;
        #pragma warning disable CS0414 // 繝輔ぅ繝ｼ繝ｫ繝峨・蜑ｲ繧雁ｽ薙※繧峨ｌ縺ｦ縺・ｋ縺後∝､縺御ｽｿ逕ｨ縺輔ｌ縺ｦ縺・↑縺・        private bool m_IsPlaying = false; // 蟆・擂縺ｮ諡｡蠑ｵ・亥柑譫懊・迥ｶ諷狗｢ｺ隱搾ｼ峨〒菴ｿ逕ｨ莠亥ｮ・        #pragma warning restore CS0414
        private int m_CurrentLevel = 0;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            m_GlitchImage = GetComponent<Image>();
            if (m_GlitchImage == null)
            {
                Debug.LogError("GlitchEffect: Image component is required!");
            }
        }

        private void OnDestroy()
        {
            // 繝槭ユ繝ｪ繧｢繝ｫ縺ｮ繧ｯ繝ｪ繝ｼ繝ｳ繧｢繝・・
            if (m_GlitchMaterial != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(m_GlitchMaterial);
                }
                else
                {
                    DestroyImmediate(m_GlitchMaterial);
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// GlitchEffect縺ｮ蛻晄悄蛹・        /// </summary>
        /// <param name="canvas">隕ｪ縺ｨ縺ｪ繧気anvas</param>
        public void Initialize(Canvas canvas)
        {
            m_ParentCanvas = canvas;

            // Image繧ｳ繝ｳ繝昴・繝阪Φ繝医・險ｭ螳・            if (m_GlitchImage != null)
            {
                // 逕ｻ髱｢蜈ｨ菴薙ｒ隕・≧繧医≧縺ｫ險ｭ螳・                RectTransform rectTransform = m_GlitchImage.rectTransform;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;

                // 蛻晄悄迥ｶ諷九〒縺ｯ髱櫁｡ｨ遉ｺ
                m_GlitchImage.enabled = false;
                m_GlitchImage.color = Color.white;

                // 繧ｰ繝ｪ繝・メ逕ｨ繝槭ユ繝ｪ繧｢繝ｫ繧剃ｽ懈・
                CreateGlitchMaterial();
            }
        }

        /// <summary>
        /// 繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ蜀咲函
        /// </summary>
        /// <param name="level">繧ｰ繝ｪ繝・メ繝ｬ繝吶Ν・・-5・・/param>
        /// <param name="duration">蜉ｹ譫懊・謖∫ｶ壽凾髢難ｼ育ｧ抵ｼ・/param>
        public void PlayGlitch(int level, float duration)
        {
            if (m_GlitchImage == null || m_GlitchMaterial == null)
            {
                Debug.LogWarning("GlitchEffect: Not properly initialized.");
                return;
            }

            // 繝ｬ繝吶Ν繧偵け繝ｩ繝ｳ繝・            int clampedLevel = Mathf.Clamp(level, c_MinLevel, c_MaxLevel);
            m_CurrentLevel = clampedLevel;

            // 譌｢蟄倥・繧ｳ繝ｫ繝ｼ繝√Φ繧貞●豁｢
            if (m_GlitchCoroutine != null)
            {
                StopCoroutine(m_GlitchCoroutine);
            }

            // 繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ髢句ｧ・            m_GlitchCoroutine = StartCoroutine(PlayGlitchCoroutine(clampedLevel, duration));
        }

        /// <summary>
        /// 繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ蛛懈ｭ｢
        /// </summary>
        public void StopGlitch()
        {
            if (m_GlitchCoroutine != null)
            {
                StopCoroutine(m_GlitchCoroutine);
                m_GlitchCoroutine = null;
            }

            if (m_GlitchImage != null)
            {
                m_GlitchImage.enabled = false;
            }

            m_IsPlaying = false;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 繧ｰ繝ｪ繝・メ逕ｨ繝槭ユ繝ｪ繧｢繝ｫ繧剃ｽ懈・
        /// Unity讓呎ｺ悶・UI/Default繧ｷ繧ｧ繝ｼ繝繝ｼ繧剃ｽｿ逕ｨ縺励√・繝ｭ繝代ユ繧｣縺ｧ繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ蛻ｶ蠕｡
        /// </summary>
        private void CreateGlitchMaterial()
        {
            // Unity讓呎ｺ悶・UI/Default繧ｷ繧ｧ繝ｼ繝繝ｼ繧剃ｽｿ逕ｨ
            Shader uiShader = Shader.Find("UI/Default");
            if (uiShader == null)
            {
                Debug.LogError("GlitchEffect: UI/Default shader not found!");
                return;
            }

            m_GlitchMaterial = new Material(uiShader);
            m_GlitchMaterial.name = "GlitchMaterial";

            // 繝弱う繧ｺ繝・け繧ｹ繝√Ε繧堤函謌撰ｼ医・繝ｭ繧ｷ繝ｼ繧ｸ繝｣繝ｫ・・            Texture2D noiseTexture = GenerateNoiseTexture(256, 256);
            m_GlitchMaterial.mainTexture = noiseTexture;

            // 繝槭ユ繝ｪ繧｢繝ｫ繧棚mage縺ｫ驕ｩ逕ｨ
            m_GlitchImage.material = m_GlitchMaterial;
        }

        /// <summary>
        /// 繝弱う繧ｺ繝・け繧ｹ繝√Ε繧堤函謌・        /// </summary>
        /// <param name="width">繝・け繧ｹ繝√Ε縺ｮ蟷・/param>
        /// <param name="height">繝・け繧ｹ繝√Ε縺ｮ鬮倥＆</param>
        /// <returns>逕滓・縺輔ｌ縺溘ヮ繧､繧ｺ繝・け繧ｹ繝√Ε</returns>
        private Texture2D GenerateNoiseTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.name = "GlitchNoiseTexture";
            texture.filterMode = FilterMode.Point; // 繝斐け繧ｻ繝ｫ繝代・繝輔ぉ繧ｯ繝医↑繝弱う繧ｺ

            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float noise = Random.Range(0f, 1f);
                    pixels[y * width + x] = new Color(noise, noise, noise, 1f);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        /// <summary>
        /// 繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ蜀咲函縺吶ｋ繧ｳ繝ｫ繝ｼ繝√Φ
        /// </summary>
        /// <param name="level">繧ｰ繝ｪ繝・メ繝ｬ繝吶Ν</param>
        /// <param name="duration">謖∫ｶ壽凾髢・/param>
        private IEnumerator PlayGlitchCoroutine(int level, float duration)
        {
            m_IsPlaying = true;
            m_GlitchImage.enabled = true;

            float elapsedTime = 0f;
            float intensity = CalculateIntensity(level);

            // 繧ｰ繝ｪ繝・メ蜉ｹ譫懊・繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / duration;

                // 繝輔ぉ繝ｼ繝峨い繧ｦ繝茨ｼ亥ｾ悟濠縺ｧ蠕舌・↓蠑ｱ繧√ｋ・・                float fadeMultiplier = 1f;
                if (normalizedTime > 0.7f)
                {
                    fadeMultiplier = Mathf.Lerp(1f, 0f, (normalizedTime - 0.7f) / 0.3f);
                }

                // 繝弱う繧ｺ縺ｮ蠑ｷ蠎ｦ繧呈峩譁ｰ
                UpdateGlitchVisuals(level, intensity * fadeMultiplier);

                yield return null;
            }

            // 蜉ｹ譫懊ｒ蛛懈ｭ｢
            StopGlitch();
        }

        /// <summary>
        /// 繝ｬ繝吶Ν縺ｫ蠢懊§縺溷ｼｷ蠎ｦ繧定ｨ育ｮ・        /// </summary>
        /// <param name="level">繧ｰ繝ｪ繝・メ繝ｬ繝吶Ν・・-5・・/param>
        /// <returns>蠑ｷ蠎ｦ蛟､・・-1・・/returns>
        private float CalculateIntensity(int level)
        {
            // 繝ｬ繝吶Ν1: 0.2, 繝ｬ繝吶Ν5: 1.0 縺ｮ遽・峇縺ｧ邱壼ｽ｢陬憺俣
            return Mathf.Lerp(0.2f, 1.0f, (level - 1) / 4f);
        }

        /// <summary>
        /// 繧ｰ繝ｪ繝・メ縺ｮ隕冶ｦ壼柑譫懊ｒ譖ｴ譁ｰ
        /// </summary>
        /// <param name="level">繧ｰ繝ｪ繝・メ繝ｬ繝吶Ν</param>
        /// <param name="intensity">蠑ｷ蠎ｦ</param>
        private void UpdateGlitchVisuals(int level, float intensity)
        {
            if (m_GlitchImage == null)
                return;

            // 繝弱う繧ｺ縺ｮ荳埼乗・蠎ｦ繧呈峩譁ｰ
            Color currentColor = m_GlitchImage.color;
            currentColor.a = intensity * 0.8f; // 譛螟ｧ80%縺ｮ荳埼乗・蠎ｦ
            m_GlitchImage.color = currentColor;

            // 繝ｬ繝吶Ν縺ｫ蠢懊§縺溯ｿｽ蜉蜉ｹ譫・            switch (level)
            {
                case 1:
                    // 霆ｽ蠕ｮ縺ｪ繝弱う繧ｺ縺ｮ縺ｿ
                    break;

                case 2:
                    // 繧上★縺九↑濶ｲ縺壹ｌ繧定ｿｽ蜉・郁牡蜿主ｷｮ縺ｮ邁｡譏鍋沿・・                    currentColor.r = 1f + (Random.Range(-0.1f, 0.1f) * intensity);
                    currentColor.g = 1f + (Random.Range(-0.1f, 0.1f) * intensity);
                    currentColor.b = 1f + (Random.Range(-0.1f, 0.1f) * intensity);
                    m_GlitchImage.color = currentColor;
                    break;

                case 3:
                case 4:
                case 5:
                    // 繧医ｊ蠑ｷ縺・牡縺壹ｌ縺ｨ繝ｩ繝ｳ繝繝縺ｪ菴咲ｽｮ繧ｪ繝輔そ繝・ヨ
                    currentColor.r = 1f + (Random.Range(-0.2f, 0.2f) * intensity);
                    currentColor.g = 1f + (Random.Range(-0.2f, 0.2f) * intensity);
                    currentColor.b = 1f + (Random.Range(-0.2f, 0.2f) * intensity);
                    m_GlitchImage.color = currentColor;

                    // 菴咲ｽｮ繧偵Λ繝ｳ繝繝縺ｫ縺壹ｉ縺呻ｼ医せ繧ｭ繝｣繝ｳ繝ｩ繧､繝ｳ蜉ｹ譫懊・邁｡譏鍋沿・・                    RectTransform rectTransform = m_GlitchImage.rectTransform;
                    float offsetX = Random.Range(-5f, 5f) * intensity;
                    float offsetY = Random.Range(-2f, 2f) * intensity;
                    rectTransform.anchoredPosition = new Vector2(offsetX, offsetY);
                    break;
            }

            // 繝弱う繧ｺ繝・け繧ｹ繝√Ε繧呈峩譁ｰ・亥虚逧・↑繝弱う繧ｺ蜉ｹ譫懶ｼ・            if (m_GlitchMaterial != null && m_GlitchMaterial.mainTexture != null)
            {
                // 繝輔Ξ繝ｼ繝縺斐→縺ｫ繝弱う繧ｺ繧呈峩譁ｰ・医Ξ繝吶Ν縺碁ｫ倥＞縺ｻ縺ｩ鬆ｻ郢√↓・・                if (Random.Range(0f, 1f) < (intensity * 0.3f))
                {
                    UpdateNoiseTexture();
                }
            }
        }

        /// <summary>
        /// 繝弱う繧ｺ繝・け繧ｹ繝√Ε繧呈峩譁ｰ・亥虚逧・↑繝弱う繧ｺ蜉ｹ譫懶ｼ・        /// </summary>
        private void UpdateNoiseTexture()
        {
            if (m_GlitchMaterial == null || m_GlitchMaterial.mainTexture == null)
                return;

            Texture2D noiseTexture = m_GlitchMaterial.mainTexture as Texture2D;
            if (noiseTexture == null)
                return;

            // 繝・け繧ｹ繝√Ε縺ｮ荳驛ｨ繧偵Λ繝ｳ繝繝縺ｫ譖ｴ譁ｰ
            int updateCount = Random.Range(10, 50);
            for (int i = 0; i < updateCount; i++)
            {
                int x = Random.Range(0, noiseTexture.width);
                int y = Random.Range(0, noiseTexture.height);
                float noise = Random.Range(0f, 1f);
                noiseTexture.SetPixel(x, y, new Color(noise, noise, noise, 1f));
            }

            noiseTexture.Apply();
        }
        #endregion
    }
}
