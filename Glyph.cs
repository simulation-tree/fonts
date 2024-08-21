using Fonts.Components;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;
using Unmanaged.Collections;

namespace Fonts
{
    public readonly struct Glyph : IGlyph, IDisposable
    {
        private readonly Entity entity;

        public readonly char Character
        {
            get
            {
                IsGlyph component = entity.GetComponent<IsGlyph>();
                return component.character;
            }
        }

        /// <summary>
        /// Amount of distance to move the cursor when laying out text.
        /// </summary>
        public readonly Vector2 Advance
        {
            get
            {
                IsGlyph component = entity.GetComponent<IsGlyph>();
                return component.advance;
            }
        }

        /// <summary>
        /// Distance away from the cursor position on the baseline.
        /// </summary>
        public readonly Vector2 Bearing
        {
            get
            {
                IsGlyph component = entity.GetComponent<IsGlyph>();
                return component.bearing;
            }
        }

        /// <summary>
        /// Distance away from the top left corner in the image
        /// where the glyph begins.
        /// </summary>
        public readonly Vector2 Offset
        {
            get
            {
                IsGlyph component = entity.GetComponent<IsGlyph>();
                return component.offset;
            }
        }

        /// <summary>
        /// Size of this glyph's bounding box.
        /// </summary>
        public readonly Vector2 Size
        {
            get
            {
                IsGlyph component = entity.GetComponent<IsGlyph>();
                return component.size;
            }
        }

        public readonly ReadOnlySpan<Kerning> Kernings => entity.GetList<Kerning>().AsSpan();

        eint IEntity.Value => entity;
        World IEntity.World => entity;

        public Glyph(World world, eint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Glyph(World world, char character, Vector2 advance, Vector2 bearing, Vector2 offset, Vector2 size, ReadOnlySpan<Kerning> kernings)
        {
            this.entity = new(world);
            entity.AddComponent(new IsGlyph(character, advance, bearing, offset, size));

            UnmanagedList<Kerning> kerningsList = entity.CreateList<Kerning>((uint)(kernings.Length + 1));
            kerningsList.AddRange(kernings);
        }

        public readonly override string ToString()
        {
            char character = Character;
            Span<char> buffer = ['\'', character, '\''];
            return new string(buffer);
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        Query IEntity.GetQuery(World world)
        {
            return new Query(world, RuntimeType.Get<IsGlyph>());
        }

        public readonly Vector2 GetKerning(char nextCharacter)
        {
            UnmanagedList<Kerning> kernings = entity.GetList<Kerning>();
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
            UnmanagedList<Kerning> kernings = entity.GetList<Kerning>();
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
            UnmanagedList<Kerning> kernings = entity.GetList<Kerning>();
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
            UnmanagedList<Kerning> kernings = entity.GetList<Kerning>();
            kernings.Clear();
        }

        public static implicit operator Entity(Glyph glyph)
        {
            return glyph.entity;
        }
    }
}