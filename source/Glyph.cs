using Fonts.Components;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;

namespace Fonts
{
    public readonly struct Glyph : IGlyph
    {
        private readonly Entity entity;

        public readonly char Character
        {
            get
            {
                IsGlyph component = entity.GetComponentRef<IsGlyph>();
                return component.character;
            }
        }

        /// <summary>
        /// Amount of distance to move the cursor when laying out text.
        /// </summary>
        public readonly (int x, int y) Advance
        {
            get
            {
                IsGlyph component = entity.GetComponentRef<IsGlyph>();
                return component.advance;
            }
        }

        /// <summary>
        /// Distance away from the cursor position on the baseline.
        /// </summary>
        public readonly (int x, int y) Bearing
        {
            get
            {
                IsGlyph component = entity.GetComponentRef<IsGlyph>();
                return component.bearing;
            }
        }

        /// <summary>
        /// Distance away from the top left corner in the image
        /// where the glyph begins.
        /// </summary>
        public readonly (int x, int y) Offset
        {
            get
            {
                IsGlyph component = entity.GetComponentRef<IsGlyph>();
                return component.offset;
            }
        }

        /// <summary>
        /// Size of this glyph's bounding box.
        /// </summary>
        public readonly (int x, int y) Size
        {
            get
            {
                IsGlyph component = entity.GetComponentRef<IsGlyph>();
                return component.size;
            }
        }

        public readonly ReadOnlySpan<Kerning> Kernings => entity.GetArray<Kerning>();

        uint IEntity.Value => entity;
        World IEntity.World => entity;

        public Glyph(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Glyph(World world, char character, (int x, int y) advance, (int x, int y) bearing, (int x, int y) offset, (int x, int y) size, ReadOnlySpan<Kerning> kernings)
        {
            this.entity = new(world);
            entity.AddComponent(new IsGlyph(character, advance, bearing, offset, size));
            entity.CreateArray(kernings);
        }

        public readonly override string ToString()
        {
            char character = Character;
            Span<char> buffer = ['\'', character, '\''];
            return new string(buffer);
        }

        Query IEntity.GetQuery(World world)
        {
            return new Query(world, RuntimeType.Get<IsGlyph>());
        }

        public readonly Vector2 GetKerning(char nextCharacter)
        {
            Span<Kerning> kernings = entity.GetArray<Kerning>();
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
            Span<Kerning> kernings = entity.GetArray<Kerning>();
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
            Span<Kerning> kernings = entity.GetArray<Kerning>();
            for (uint i = 0; i < kernings.Length; i++)
            {
                if (kernings[(int)i].nextCharacter == nextCharacter)
                {
                    throw new ArgumentException($"Kerning for character '{nextCharacter}' already exists");
                }
            }

            kernings = entity.ResizeArray<Kerning>((uint)kernings.Length + 1);
            kernings[(int)kernings.Length - 1] = new(nextCharacter, amount);
        }

        public readonly void ClearKernings()
        {
            entity.ResizeArray<Kerning>(0);
        }

        public static implicit operator Entity(Glyph glyph)
        {
            return glyph.entity;
        }
    }
}