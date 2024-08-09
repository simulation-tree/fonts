using Fonts;
using Fonts.Components;
using Textures;
using Unmanaged;

public static class FontFunctions
{
    public static FixedString GetFamilyName<T>(this T font) where T : IFont
    {
        return font.GetComponent<T, FontName>().familyName;
    }

    public static float GetLineHeight<T>(this T font) where T : IFont
    {
        return font.GetComponent<T, FontMetrics>().lineHeight;
    }

    public static AtlasTexture GetAtlasTexture<T>(this T font) where T : IFont
    {
        return new(font.World, font.GetComponent<T, FontAtlas>().value);
    }

    public static uint GetGlyphCount<T>(this T font) where T : IFont
    {
        return font.GetList<T, FontGlyph>().Count;
    }

    public static Glyph GetGlyph<T>(this T font, uint index) where T : IFont
    {
        return new(font.World, font.GetList<T, FontGlyph>()[index].value);
    }
}
