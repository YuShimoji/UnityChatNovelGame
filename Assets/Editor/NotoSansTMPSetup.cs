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
        if (fontAsset == null)
        {
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
            AssetDatabase.CreateAsset(fontAsset, OutputFontAssetPath);
        }

        var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(TmpSettingsPath);
        if (settings == null)
        {
            Debug.LogError("TMP Settings asset not found at Assets/TextMesh Pro/Resources/TMP Settings.asset");
            return;
        }

        var fallbackFonts = TMP_Settings.fallbackFontAssets ?? new List<TMP_FontAsset>();
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
