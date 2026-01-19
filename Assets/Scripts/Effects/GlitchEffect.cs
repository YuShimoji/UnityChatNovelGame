using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectFoundPhone.Effects
{
    /// <summary>
    /// グリッチ効果を実装するコンポーネント
    /// UI Imageを使用して画面全体にノイズとグリッチ効果を表示する
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
        #pragma warning disable CS0414 // フィールドは割り当てられているが、値が使用されていない
        private bool m_IsPlaying = false; // 将来の拡張（効果の状態確認）で使用予定
        #pragma warning restore CS0414
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
            // マテリアルのクリーンアップ
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
        /// GlitchEffectの初期化
        /// </summary>
        /// <param name="canvas">親となるCanvas</param>
        public void Initialize(Canvas canvas)
        {
            m_ParentCanvas = canvas;

            // Imageコンポーネントの設定
            if (m_GlitchImage != null)
            {
                // 画面全体を覆うように設定
                RectTransform rectTransform = m_GlitchImage.rectTransform;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;

                // 初期状態では非表示
                m_GlitchImage.enabled = false;
                m_GlitchImage.color = Color.white;

                // グリッチ用マテリアルを作成
                CreateGlitchMaterial();
            }
        }

        /// <summary>
        /// グリッチ効果を再生
        /// </summary>
        /// <param name="level">グリッチレベル（1-5）</param>
        /// <param name="duration">効果の持続時間（秒）</param>
        public void PlayGlitch(int level, float duration)
        {
            if (m_GlitchImage == null || m_GlitchMaterial == null)
            {
                Debug.LogWarning("GlitchEffect: Not properly initialized.");
                return;
            }

            // レベルをクランプ
            int clampedLevel = Mathf.Clamp(level, c_MinLevel, c_MaxLevel);
            m_CurrentLevel = clampedLevel;

            // 既存のコルーチンを停止
            if (m_GlitchCoroutine != null)
            {
                StopCoroutine(m_GlitchCoroutine);
            }

            // グリッチ効果を開始
            m_GlitchCoroutine = StartCoroutine(PlayGlitchCoroutine(clampedLevel, duration));
        }

        /// <summary>
        /// グリッチ効果を停止
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
        /// グリッチ用マテリアルを作成
        /// Unity標準のUI/Defaultシェーダーを使用し、プロパティでグリッチ効果を制御
        /// </summary>
        private void CreateGlitchMaterial()
        {
            // Unity標準のUI/Defaultシェーダーを使用
            Shader uiShader = Shader.Find("UI/Default");
            if (uiShader == null)
            {
                Debug.LogError("GlitchEffect: UI/Default shader not found!");
                return;
            }

            m_GlitchMaterial = new Material(uiShader);
            m_GlitchMaterial.name = "GlitchMaterial";

            // ノイズテクスチャを生成（プロシージャル）
            Texture2D noiseTexture = GenerateNoiseTexture(256, 256);
            m_GlitchMaterial.mainTexture = noiseTexture;

            // マテリアルをImageに適用
            m_GlitchImage.material = m_GlitchMaterial;
        }

        /// <summary>
        /// ノイズテクスチャを生成
        /// </summary>
        /// <param name="width">テクスチャの幅</param>
        /// <param name="height">テクスチャの高さ</param>
        /// <returns>生成されたノイズテクスチャ</returns>
        private Texture2D GenerateNoiseTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.name = "GlitchNoiseTexture";
            texture.filterMode = FilterMode.Point; // ピクセルパーフェクトなノイズ

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
        /// グリッチ効果を再生するコルーチン
        /// </summary>
        /// <param name="level">グリッチレベル</param>
        /// <param name="duration">持続時間</param>
        private IEnumerator PlayGlitchCoroutine(int level, float duration)
        {
            m_IsPlaying = true;
            m_GlitchImage.enabled = true;

            float elapsedTime = 0f;
            float intensity = CalculateIntensity(level);

            // グリッチ効果のアニメーション
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / duration;

                // フェードアウト（後半で徐々に弱める）
                float fadeMultiplier = 1f;
                if (normalizedTime > 0.7f)
                {
                    fadeMultiplier = Mathf.Lerp(1f, 0f, (normalizedTime - 0.7f) / 0.3f);
                }

                // ノイズの強度を更新
                UpdateGlitchVisuals(level, intensity * fadeMultiplier);

                yield return null;
            }

            // 効果を停止
            StopGlitch();
        }

        /// <summary>
        /// レベルに応じた強度を計算
        /// </summary>
        /// <param name="level">グリッチレベル（1-5）</param>
        /// <returns>強度値（0-1）</returns>
        private float CalculateIntensity(int level)
        {
            // レベル1: 0.2, レベル5: 1.0 の範囲で線形補間
            return Mathf.Lerp(0.2f, 1.0f, (level - 1) / 4f);
        }

        /// <summary>
        /// グリッチの視覚効果を更新
        /// </summary>
        /// <param name="level">グリッチレベル</param>
        /// <param name="intensity">強度</param>
        private void UpdateGlitchVisuals(int level, float intensity)
        {
            if (m_GlitchImage == null)
                return;

            // ノイズの不透明度を更新
            Color currentColor = m_GlitchImage.color;
            currentColor.a = intensity * 0.8f; // 最大80%の不透明度
            m_GlitchImage.color = currentColor;

            // レベルに応じた追加効果
            switch (level)
            {
                case 1:
                    // 軽微なノイズのみ
                    break;

                case 2:
                    // わずかな色ずれを追加（色収差の簡易版）
                    currentColor.r = 1f + (Random.Range(-0.1f, 0.1f) * intensity);
                    currentColor.g = 1f + (Random.Range(-0.1f, 0.1f) * intensity);
                    currentColor.b = 1f + (Random.Range(-0.1f, 0.1f) * intensity);
                    m_GlitchImage.color = currentColor;
                    break;

                case 3:
                case 4:
                case 5:
                    // より強い色ずれとランダムな位置オフセット
                    currentColor.r = 1f + (Random.Range(-0.2f, 0.2f) * intensity);
                    currentColor.g = 1f + (Random.Range(-0.2f, 0.2f) * intensity);
                    currentColor.b = 1f + (Random.Range(-0.2f, 0.2f) * intensity);
                    m_GlitchImage.color = currentColor;

                    // 位置をランダムにずらす（スキャンライン効果の簡易版）
                    RectTransform rectTransform = m_GlitchImage.rectTransform;
                    float offsetX = Random.Range(-5f, 5f) * intensity;
                    float offsetY = Random.Range(-2f, 2f) * intensity;
                    rectTransform.anchoredPosition = new Vector2(offsetX, offsetY);
                    break;
            }

            // ノイズテクスチャを更新（動的なノイズ効果）
            if (m_GlitchMaterial != null && m_GlitchMaterial.mainTexture != null)
            {
                // フレームごとにノイズを更新（レベルが高いほど頻繁に）
                if (Random.Range(0f, 1f) < (intensity * 0.3f))
                {
                    UpdateNoiseTexture();
                }
            }
        }

        /// <summary>
        /// ノイズテクスチャを更新（動的なノイズ効果）
        /// </summary>
        private void UpdateNoiseTexture()
        {
            if (m_GlitchMaterial == null || m_GlitchMaterial.mainTexture == null)
                return;

            Texture2D noiseTexture = m_GlitchMaterial.mainTexture as Texture2D;
            if (noiseTexture == null)
                return;

            // テクスチャの一部をランダムに更新
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
