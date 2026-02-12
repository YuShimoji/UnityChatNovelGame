using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public static class NotoSansTMPSetup
{
    private const string DefaultFontPath = "Assets/Font/static/NotoSansJP-Regular.ttf";
    private const string VariableFontPath = "Assets/Font/NotoSansJP-VariableFont_wght.ttf";
    private const string OutputFontAssetPath = "Assets/Font/NotoSansJP-Regular SDF.asset";
    private const string TmpSettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
    private const string SeedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789あいうえおアイウエオ漢字仮名々（）「」『』、。！？ー・";

    [MenuItem("Tools/Font/Setup Noto Sans JP TMP")]
    public static void SetupNotoSansJpTmp()
    {
        var font = LoadSourceFont();
        if (font == null)
        {
            Debug.LogError("Noto Sans JP font not found. Expected at Assets/Font/static/NotoSansJP-Regular.ttf or Assets/Font/NotoSansJP-VariableFont_wght.ttf");
            return;
        }

        var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(OutputFontAssetPath);
        var needsRebuild = fontAsset == null
                           || fontAsset.material == null
                           || fontAsset.atlasTextures == null
                           || fontAsset.atlasTextures.Length == 0
                           || fontAsset.atlasTextures[0] == null;
        if (needsRebuild)
        {
            if (fontAsset != null)
            {
                AssetDatabase.DeleteAsset(OutputFontAssetPath);
            }

            fontAsset = TMP_FontAsset.CreateFontAsset(
                font,
                90,
                9,
                GlyphRenderMode.SDFAA,
                4096,
                4096,
                AtlasPopulationMode.Dynamic
            );
            fontAsset.name = "NotoSansJP-Regular SDF";
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            AssetDatabase.CreateAsset(fontAsset, OutputFontAssetPath);

            fontAsset.TryAddCharacters(SeedCharacters);

            if (fontAsset.material != null)
            {
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            }

            if (fontAsset.atlasTextures != null)
            {
                foreach (var atlasTexture in fontAsset.atlasTextures)
                {
                    if (atlasTexture != null)
                    {
                        AssetDatabase.AddObjectToAsset(atlasTexture, fontAsset);
                    }
                }
            }

            EditorUtility.SetDirty(fontAsset);
        }

        var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(TmpSettingsPath);
        if (settings == null)
        {
            Debug.LogError("TMP Settings asset not found at Assets/TextMesh Pro/Resources/TMP Settings.asset");
            return;
        }

        TMP_Settings.defaultFontAsset = fontAsset;

        var fallbackFonts = TMP_Settings.fallbackFontAssets ?? new List<TMP_FontAsset>();
        fallbackFonts.RemoveAll(item => item == null);
        if (!fallbackFonts.Contains(fontAsset))
        {
            fallbackFonts.Add(fontAsset);
            TMP_Settings.fallbackFontAssets = fallbackFonts;
            EditorUtility.SetDirty(settings);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Noto Sans JP TMP setup complete. Fallback font added to TMP Settings.");
    }

    private static Font LoadSourceFont()
    {
        var font = AssetDatabase.LoadAssetAtPath<Font>(DefaultFontPath);
        if (font != null)
        {
            return font;
        }

        return AssetDatabase.LoadAssetAtPath<Font>(VariableFontPath);
    }
}
