using System.Collections.Generic;
using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// BubbleStylePreset を presetId で引けるレジストリ。
    /// Resources/BubbleStyles 配下の全プリセットを起動時に自動収集する。
    /// アセット 0 件でも null を返すだけで例外は出さない (未適用 = 既定挙動)。
    /// </summary>
    public static class BubbleStyleDatabase
    {
        private const string ResourcesFolder = "BubbleStyles";

        private static Dictionary<string, BubbleStylePreset> s_PresetsById;

        private static Dictionary<string, BubbleStylePreset> Presets
        {
            get
            {
                if (s_PresetsById == null)
                {
                    Load();
                }
                return s_PresetsById;
            }
        }

        /// <summary>
        /// presetId からプリセットを取得する。見つからなければ null。
        /// </summary>
        public static BubbleStylePreset Get(string presetId)
        {
            if (string.IsNullOrEmpty(presetId)) return null;
            Presets.TryGetValue(presetId, out var preset);
            return preset;
        }

        /// <summary>
        /// 登録済みの presetId 一覧 (デバッグ用)。
        /// </summary>
        public static IEnumerable<string> AllPresetIds => Presets.Keys;

        /// <summary>
        /// テスト・再読込用にキャッシュを破棄する。
        /// </summary>
        public static void Reload()
        {
            s_PresetsById = null;
        }

        private static void Load()
        {
            s_PresetsById = new Dictionary<string, BubbleStylePreset>();
            var all = Resources.LoadAll<BubbleStylePreset>(ResourcesFolder);
            if (all == null) return;

            for (int i = 0; i < all.Length; i++)
            {
                var preset = all[i];
                if (preset == null || string.IsNullOrEmpty(preset.presetId)) continue;
                if (s_PresetsById.ContainsKey(preset.presetId))
                {
                    Debug.LogWarning($"[BubbleStyleDatabase] 重複する presetId: {preset.presetId} (最初の定義を優先)");
                    continue;
                }
                s_PresetsById.Add(preset.presetId, preset);
            }
        }
    }
}
