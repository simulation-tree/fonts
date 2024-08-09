using Fonts;
using Fonts.Components;
using System;
using System.Numerics;
using Unmanaged.Collections;

public static class GlyphFunctions
{
    public static char GetCharacter<T>(this T glyph) where T : IGlyph
    {
        IsGlyph component = glyph.GetComponent<T, IsGlyph>();
        return component.character;
    }

    public static Vector2 GetAdvance<T>(this T glyph) where T : IGlyph
    {
        IsGlyph component = glyph.GetComponent<T, IsGlyph>();
        return component.advance;
    }

    public static Vector2 GetOffset<T>(this T glyph) where T : IGlyph
    {
        IsGlyph component = glyph.GetComponent<T, IsGlyph>();
        return component.offset;
    }

    public static Vector2 GetSize<T>(this T glyph) where T : IGlyph
    {
        IsGlyph component = glyph.GetComponent<T, IsGlyph>();
        return component.size;
    }

    public static Vector4 GetRegion<T>(this T glyph) where T : IGlyph
    {
        IsGlyph component = glyph.GetComponent<T, IsGlyph>();
        return component.region;
    }

    public static ReadOnlySpan<Kerning> GetKernings<T>(this T glyph) where T : IGlyph
    {
        return glyph.GetList<T, Kerning>().AsSpan();
    }

    public static Vector2 GetKerning<T>(this T glyph, char nextCharacter) where T : IGlyph
    {
        UnmanagedList<Kerning> kernings = glyph.GetList<T, Kerning>();
        foreach (Kerning kerning in kernings)
        {
            if (kerning.nextCharacter == nextCharacter)
            {
                return kerning.amount;
            }
        }

        return default;
    }

    public static bool ContainsKerning<T>(this T glyph, char nextCharacter) where T : IGlyph
    {
        UnmanagedList<Kerning> kernings = glyph.GetList<T, Kerning>();
        foreach (Kerning kerning in kernings)
        {
            if (kerning.nextCharacter == nextCharacter)
            {
                return true;
            }
        }

        return false;
    }

    public static void AddKerning<T>(this T glyph, char nextCharacter, Vector2 amount) where T : IGlyph
    {
        UnmanagedList<Kerning> kernings = glyph.GetList<T, Kerning>();
        for (uint i = 0; i < kernings.Count; i++)
        {
            if (kernings[i].nextCharacter == nextCharacter)
            {
                throw new ArgumentException($"Kerning for character '{nextCharacter}' already exists");
            }
        }

        kernings.Add(new(nextCharacter, amount));
    }

    public static void ClearKernings<T>(this T glyph) where T : IGlyph
    {
        UnmanagedList<Kerning> kernings = glyph.GetList<T, Kerning>();
        kernings.Clear();
    }
}