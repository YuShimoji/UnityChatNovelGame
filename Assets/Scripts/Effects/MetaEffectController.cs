using UnityEngine;
using System.Collections;

namespace ProjectFoundPhone.Effects
{
    /// <summary>
    /// 繝｡繧ｿ貍泌・・医げ繝ｪ繝・メ蜉ｹ譫懃ｭ会ｼ峨ｒ蛻ｶ蠕｡縺吶ｋ繧ｳ繝ｳ繝医Ο繝ｼ繝ｩ繝ｼ
    /// 繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ繝代ち繝ｼ繝ｳ縺ｧ螳溯｣・＆繧後ヾcenarioManager縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧・    /// </summary>
    public class MetaEffectController : MonoBehaviour
    {
        #region Singleton
        private static MetaEffectController s_Instance;

        /// <summary>
        /// 繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ縺ｸ縺ｮ繧｢繧ｯ繧ｻ繧ｹ
        /// 繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ縺悟ｭ伜惠縺励↑縺・ｴ蜷医・閾ｪ蜍慕噪縺ｫ菴懈・縺吶ｋ
        /// </summary>
        public static MetaEffectController Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    // 譌｢蟄倥・繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ繧呈､懃ｴ｢
                    s_Instance = FindFirstObjectByType<MetaEffectController>();

                    // 隕九▽縺九ｉ縺ｪ縺・ｴ蜷医・譁ｰ隕丈ｽ懈・
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
        [SerializeField] private int m_GlitchEffectLayer = 100; // 譛蜑埼擇縺ｫ陦ｨ遉ｺ縺吶ｋ縺溘ａ縺ｮ繝ｬ繧､繝､繝ｼ
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // 繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ縺ｮ蛻晄悄蛹・            if (s_Instance == null)
            {
                s_Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeEffects();
            }
            else if (s_Instance != this)
            {
                // 譌｢縺ｫ繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ縺悟ｭ伜惠縺吶ｋ蝣ｴ蜷医・遐ｴ譽・                Destroy(gameObject);
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
        /// 繧ｨ繝輔ぉ繧ｯ繝医す繧ｹ繝・Β縺ｮ蛻晄悄蛹・        /// </summary>
        private void InitializeEffects()
        {
            // 繧ｨ繝輔ぉ繧ｯ繝育畑縺ｮCanvas繧剃ｽ懈・
            if (m_EffectCanvas == null)
            {
                GameObject canvasObject = new GameObject("EffectCanvas");
                canvasObject.transform.SetParent(transform);
                m_EffectCanvas = canvasObject.AddComponent<Canvas>();
                m_EffectCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                m_EffectCanvas.sortingOrder = m_GlitchEffectLayer;

                // CanvasScaler繧定ｿｽ蜉縺励※逕ｻ髱｢繧ｵ繧､繧ｺ縺ｫ蟇ｾ蠢・                UnityEngine.UI.CanvasScaler scaler = canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                // GraphicRaycaster繧定ｿｽ蜉・亥ｿ・ｦ√↓蠢懊§縺ｦ・・                canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }

            // GlitchEffect繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ蛻晄悄蛹・            if (m_GlitchEffect == null)
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
        /// 繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ蜀咲函
        /// </summary>
        /// <param name="level">繧ｰ繝ｪ繝・メ縺ｮ蠑ｷ蠎ｦ繝ｬ繝吶Ν・・-5・・/param>
        /// <param name="duration">蜉ｹ譫懊・謖∫ｶ壽凾髢難ｼ育ｧ抵ｼ峨・莉･荳九・蝣ｴ蜷医・閾ｪ蜍慕噪縺ｫ險育ｮ励＆繧後ｋ</param>
        public void PlayGlitchEffect(int level, float duration = 0f)
        {
            if (m_GlitchEffect == null)
            {
                Debug.LogWarning("MetaEffectController: GlitchEffect is not initialized.");
                InitializeEffects();
            }

            // 繝ｬ繝吶Ν繧・-5縺ｮ遽・峇縺ｫ繧ｯ繝ｩ繝ｳ繝・            int clampedLevel = Mathf.Clamp(level, 1, 5);

            // 繝・ヵ繧ｩ繝ｫ繝医・謖∫ｶ壽凾髢薙ｒ險育ｮ暦ｼ医Ξ繝吶Ν縺ｫ蠢懊§縺ｦ・・            if (duration <= 0f)
            {
                duration = CalculateDefaultDuration(clampedLevel);
            }

            m_GlitchEffect?.PlayGlitch(clampedLevel, duration);
        }

        /// <summary>
        /// 繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ蛛懈ｭ｢
        /// </summary>
        public void StopGlitchEffect()
        {
            m_GlitchEffect?.StopGlitch();
        }

        /// <summary>
        /// 繝ｬ繝吶Ν縺ｫ蠢懊§縺溘ョ繝輔か繝ｫ繝医・謖∫ｶ壽凾髢薙ｒ險育ｮ・        /// </summary>
        /// <param name="level">繧ｰ繝ｪ繝・メ繝ｬ繝吶Ν・・-5・・/param>
        /// <returns>謖∫ｶ壽凾髢難ｼ育ｧ抵ｼ・/returns>
        private float CalculateDefaultDuration(int level)
        {
            // 繝ｬ繝吶Ν縺碁ｫ倥＞縺ｻ縺ｩ髟ｷ縺剰｡ｨ遉ｺ
            return 0.2f + (level * 0.1f);
        }
        /// <summary>
        /// 謖・ｮ壹＠縺溷錐蜑阪・繧ｨ繝輔ぉ繧ｯ繝医ｒ蜀咲函縺吶ｋ
        /// </summary>
        /// <param name="effectName">Resources/Effects/蜀・・Prefab蜷・/param>
        /// <param name="position">蜀咲函菴咲ｽｮ・医Ρ繝ｼ繝ｫ繝牙ｺｧ讓呻ｼ・/param>
        /// <param name="duration">閾ｪ蜍慕ｴ螢翫∪縺ｧ縺ｮ譎る俣・・縺ｪ繧峨ヱ繝ｼ繝・ぅ繧ｯ繝ｫ遲峨・險ｭ螳壹↓蠕薙≧/謇句虚遐ｴ螢奇ｼ・/param>
        public void PlayEffect(string effectName, Vector3 position, float duration = 2.0f)
        {
            // 邁｡譏灘ｮ溯｣・ Resources縺九ｉ繝ｭ繝ｼ繝峨＠縺ｦ繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ蛹・            GameObject prefab = Resources.Load<GameObject>($"Effects/{effectName}");
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);
                // Canvas蜀・°World縺・.. 縺・▲縺溘ｓWorld蠎ｧ讓吶〒逕滓・
                
                if (duration > 0f)
                {
                    Destroy(instance, duration);
                }
            }
            else
            {
                Debug.LogWarning($"MetaEffectController: Effect '{effectName}' not found in Resources/Effects/");
            }
        }
        #endregion
    }
}
