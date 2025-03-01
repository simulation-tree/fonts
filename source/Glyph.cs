using Fonts.Components;
using System;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Fonts
{
    public readonly partial struct Glyph : IEntity
    {
        public readonly char Character => GetComponent<IsGlyph>().character;

        /// <summary>
        /// Amount of distance to move the cursor when laying out text.
        /// </summary>
        public readonly (int x, int y) Advance => GetComponent<IsGlyph>().advance;

        /// <summary>
        /// Distance away from the cursor position on the baseline.
        /// </summary>
        public readonly (int x, int y) Bearing => GetComponent<IsGlyph>().bearing;

        /// <summary>
        /// Distance away from the top left corner in the image
        /// where the glyph begins.
        /// </summary>
        public readonly (int x, int y) Offset => GetComponent<IsGlyph>().offset;

        /// <summary>
        /// Size of this glyph's bounding box.
        /// </summary>
        public readonly (int x, int y) Size => GetComponent<IsGlyph>().size;

        public readonly USpan<Kerning> Kernings => GetArray<Kerning>().AsSpan();

        public Glyph(World world, char character, (int x, int y) advance, (int x, int y) bearing, (int x, int y) offset, (int x, int y) size, USpan<Kerning> kernings)
        {
            this.world = world;
            value = world.CreateEntity(new IsGlyph(character, advance, bearing, offset, size));
            CreateArray(kernings);
        }

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsGlyph>();
            archetype.AddArrayType<Kerning>();
        }

        public unsafe readonly override string ToString()
        {
            char character = Character;
            USpan<char> buffer = ['\'', character, '\''];
            return buffer.ToString();
        }

        public readonly Vector2 GetKerning(char nextCharacter)
        {
            Values<Kerning> kernings = GetArray<Kerning>();
            foreach (Kerning kerning in kernings)
            {
                if (kerning.nextCharacter == nextCharacter)
                {
                    return kerning.amount;
                }
            }

            return default;
        }

        public readonly bool ContainsKerning(char nextCharacter)
        {
            Values<Kerning> kernings = GetArray<Kerning>();
            foreach (Kerning kerning in kernings)
            {
                if (kerning.nextCharacter == nextCharacter)
                {
                    return true;
                }
            }

            return false;
        }

        public readonly void AddKerning(char nextCharacter, Vector2 amount)
        {
            Values<Kerning> kernings = GetArray<Kerning>();
            uint count = kernings.Length;
            for (uint i = 0; i < count; i++)
            {
                if (kernings[i].nextCharacter == nextCharacter)
                {
                    throw new ArgumentException($"Kerning for character '{nextCharacter}' already exists");
                }
            }

            kernings.Length++;
            kernings[count] = new(nextCharacter, amount);
        }

        public readonly void ClearKernings()
        {
            Values<Kerning> kernings = GetArray<Kerning>();
            kernings.Length = 0;
        }
    }
}