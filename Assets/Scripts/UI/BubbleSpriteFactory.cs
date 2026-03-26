using UnityEngine;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// バブル表示用スプライトのランタイム生成ファクトリ。
    /// ChatController から抽出された純粋なテクスチャ生成ロジック。
    /// </summary>
    public static class BubbleSpriteFactory
    {
        private static Sprite s_CachedRoundedSprite;

        /// <summary>
        /// 角丸矩形の 9-slice 用 Sprite をランタイム生成する。
        /// 白色テクスチャを生成し、Image.color でテーマカラーを乗算する想定。
        /// 結果はキャッシュされ、同じ radius で再生成しない。
        /// </summary>
        public static Sprite GetOrCreateRoundedSprite(float cornerRadius)
        {
            if (s_CachedRoundedSprite != null) return s_CachedRoundedSprite;

            float radius = cornerRadius;
            if (radius <= 0f)
            {
                radius = 16f;
                Debug.LogWarning($"[BubbleSpriteFactory] cornerRadius is {cornerRadius}. Using default 16f.");
            }

            int r = Mathf.CeilToInt(radius);
            int size = r * 2 + 2;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            var pixels = new Color32[size * size];
            var white = new Color32(255, 255, 255, 255);
            var clear = new Color32(0, 0, 0, 0);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = 0f, dy = 0f;
                    if (x < r && y < r)                            { dx = r - x - 0.5f; dy = r - y - 0.5f; }
                    else if (x >= size - r && y < r)               { dx = x - (size - r) + 0.5f; dy = r - y - 0.5f; }
                    else if (x < r && y >= size - r)               { dx = r - x - 0.5f; dy = y - (size - r) + 0.5f; }
                    else if (x >= size - r && y >= size - r)       { dx = x - (size - r) + 0.5f; dy = y - (size - r) + 0.5f; }

                    if (dx > 0f || dy > 0f)
                    {
                        float dist = Mathf.Sqrt(dx * dx + dy * dy);
                        if (dist > r)
                            pixels[y * size + x] = clear;
                        else if (dist > r - 1f)
                        {
                            byte a = (byte)(255 * (r - dist));
                            pixels[y * size + x] = new Color32(255, 255, 255, a);
                        }
                        else
                            pixels[y * size + x] = white;
                    }
                    else
                    {
                        pixels[y * size + x] = white;
                    }
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply();

            var border = new Vector4(r, r, r, r);
            s_CachedRoundedSprite = Sprite.Create(
                tex,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                border
            );
            s_CachedRoundedSprite.name = "GeneratedRoundedBubble";
            return s_CachedRoundedSprite;
        }

        /// <summary>
        /// 円形スプライトを生成する（キャラクターアイコンのマスク用）。
        /// </summary>
        public static Sprite CreateCircleSprite(int size = 64)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;

            float center = size / 2f;
            float radius = center;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);

                    texture.SetPixel(x, y, distance <= radius ? Color.white : Color.clear);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }

        /// <summary>
        /// キャッシュされたスプライトを破棄する（テスト用）。
        /// </summary>
        public static void ClearCache()
        {
            if (s_CachedRoundedSprite != null)
            {
                Object.Destroy(s_CachedRoundedSprite);
                s_CachedRoundedSprite = null;
            }
        }
    }
}
