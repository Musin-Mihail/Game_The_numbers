using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

/// <summary>
/// Пересобирает TMP-атлас pixel-шрифта только из символов текущих переводов.
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
        var fontAsset = TMP_FontAsset.CreateFontAsset(
            font,
            90,
            6,
            GlyphRenderMode.SDFAA,
            1024,
            1024,
            AtlasPopulationMode.Static);

        fontAsset.name = "light_pixel-7_main";
        if (!fontAsset.TryAddCharacters(characters, out var missing) && !string.IsNullOrEmpty(missing))
        {
            Debug.LogWarning("[RebuildPixelFont] Нет глифов в TTF для: " + missing);
        }

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
        Debug.Log("[RebuildPixelFont] Записан " + AssetPath + ", символов в списке: " + characters.Length);
    }
}
