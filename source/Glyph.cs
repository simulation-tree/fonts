using Fonts.Components;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;

namespace Fonts
{
    public readonly struct Glyph : IGlyph
    {
        public readonly Entity entity;

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

        public readonly USpan<Kerning> Kernings => entity.GetArray<Kerning>();

        readonly uint IEntity.Value => entity.value;
        readonly World IEntity.World => entity.world;
        readonly Definition IEntity.Definition => new([RuntimeType.Get<IsGlyph>()], [RuntimeType.Get<Kerning>()]);

        public Glyph(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Glyph(World world, char character, (int x, int y) advance, (int x, int y) bearing, (int x, int y) offset, (int x, int y) size, USpan<Kerning> kernings)
        {
            this.entity = new(world);
            entity.AddComponent(new IsGlyph(character, advance, bearing, offset, size));
            entity.CreateArray(kernings);
        }

        public unsafe readonly override string ToString()
        {
            char character = Character;
            USpan<char> buffer = ['\'', character, '\''];
            return buffer.ToString();
        }

        public readonly Vector2 GetKerning(char nextCharacter)
        {
            USpan<Kerning> kernings = entity.GetArray<Kerning>();
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
            USpan<Kerning> kernings = entity.GetArray<Kerning>();
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
            USpan<Kerning> kernings = entity.GetArray<Kerning>();
            for (uint i = 0; i < kernings.Length; i++)
            {
                if (kernings[i].nextCharacter == nextCharacter)
                {
                    throw new ArgumentException($"Kerning for character '{nextCharacter}' already exists");
                }
            }

            kernings = entity.ResizeArray<Kerning>(kernings.Length + 1);
            kernings[kernings.Length - 1] = new(nextCharacter, amount);
        }

        public readonly void ClearKernings()
        {
            entity.ResizeArray<Kerning>(0);
        }
    }
}