using Fonts.Components;
using Simulation;
using System.Numerics;
using Unmanaged.Collections;

namespace Fonts
{
    public readonly struct Glyph : IDisposable
    {
        public readonly Entity entity;

        private readonly UnmanagedList<Kerning> kernings;

        public readonly char Character => entity.GetComponent<IsGlyph>().character;
        public readonly Vector2 Advance => entity.GetComponent<IsGlyph>().advance;
        public readonly Vector2 Offset => entity.GetComponent<IsGlyph>().offset;
        public readonly Vector2 Size => entity.GetComponent<IsGlyph>().size;

        /// <summary>
        /// Coordinates on the font's atlas texture.
        /// </summary>
        public readonly Vector4 Region => entity.GetComponent<IsGlyph>().region;

        public readonly ReadOnlySpan<Kerning> Kernings => kernings.AsSpan();

        public Glyph(World world, EntityID existingEntity)
        {
            entity = new(world, existingEntity);
            kernings = entity.GetCollection<Kerning>();
        }

        public Glyph(World world, char character, Vector2 advance, Vector2 offset, Vector2 size, Vector4 region, ReadOnlySpan<Kerning> kernings)
        {
            this.entity = new(world);
            entity.AddComponent(new IsGlyph(character, advance, offset, size, region));
            this.kernings = entity.CreateCollection<Kerning>((uint)(kernings.Length + 1));
            this.kernings.AddRange(kernings);
        }

        public readonly override string ToString()
        {
            Span<char> buffer = ['\'', Character, '\''];
            return new string(buffer);
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly Vector2 GetKerning(char nextCharacter)
        {
            foreach (Kerning kerning in Kernings)
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
            foreach (Kerning kerning in Kernings)
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
            for (uint i = 0; i < kernings.Count; i++)
            {
                if (kernings[i].nextCharacter == nextCharacter)
                {
                    throw new ArgumentException($"Kerning for character '{nextCharacter}' already exists");
                }
            }

            kernings.Add(new(nextCharacter, amount));
        }

        public readonly void ClearKernings()
        {
            kernings.Clear();
        }
    }
}