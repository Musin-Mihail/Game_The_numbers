using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

/// <summary>
/// Пересобирает TMP-атлас pixel-шрифта только из символов текущих переводов.
/// Пиксельный TTF нужно печь как RASTER, иначе SDF сглаживает глифы или оставляет пустой атлас.
/// </summary>
public static class RebuildPixelFontMenu
{
    private const string TtfPath = "Assets/Resources/Fonts/light_pixel-7.ttf";
    private const string AssetPath = "Assets/Resources/Fonts/light_pixel-7_main.asset";
    private const string CharactersPath = "Assets/Editor/FontCharacters.txt";

    [MenuItem("The Numbers/Rebuild Pixel Font")]
    public static void Rebuild()
    {
        var font = AssetDatabase.LoadAssetAtPath<Font>(TtfPath);
        if (font == null)
        {
            Debug.LogError("[RebuildPixelFont] Не найден TTF: " + TtfPath);
            return;
        }

        if (!File.Exists(CharactersPath))
        {
            Debug.LogError("[RebuildPixelFont] Нет списка символов: " + CharactersPath);
            return;
        }

        var characters = File.ReadAllText(CharactersPath);
        FontEngine.InitializeFontEngine();

        var fontAsset = TMP_FontAsset.CreateFontAsset(
            font,
            90,
            1,
            GlyphRenderMode.RASTER,
            1024,
            1024,
            AtlasPopulationMode.Dynamic);

        fontAsset.name = "light_pixel-7_main";
        fontAsset.TryAddCharacters(characters, out var missing);
        if (!string.IsNullOrEmpty(missing))
        {
            Debug.LogWarning("[RebuildPixelFont] Нет глифов в TTF для: " + missing);
        }

        if (fontAsset.characterTable == null || fontAsset.characterTable.Count == 0)
        {
            Debug.LogError("[RebuildPixelFont] Атлас пустой, старый ассет не тронут.");
            Object.DestroyImmediate(fontAsset);
            return;
        }

        fontAsset.atlasPopulationMode = AtlasPopulationMode.Static;
        ApplyPointFilter(fontAsset);

        if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetPath) != null)
        {
            AssetDatabase.DeleteAsset(AssetPath);
        }

        AssetDatabase.CreateAsset(fontAsset, AssetPath);

        if (fontAsset.material != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(fontAsset.material)))
        {
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
        }

        var textures = fontAsset.atlasTextures;
        if (textures != null)
        {
            foreach (var tex in textures)
            {
                if (tex != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(tex)))
                {
                    AssetDatabase.AddObjectToAsset(tex, fontAsset);
                }
            }
        }

        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[RebuildPixelFont] Записан " + AssetPath + ", глифов: " + fontAsset.characterTable.Count);
    }

    private static void ApplyPointFilter(TMP_FontAsset fontAsset)
    {
        var textures = fontAsset.atlasTextures;
        if (textures == null) return;
        foreach (var tex in textures)
        {
            if (tex == null) continue;
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
        }
    }
}
