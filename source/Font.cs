using Data.Components;
using Fonts.Components;
using Simulation;
using System;
using Unmanaged;

namespace Fonts
{
    /// <summary>
    /// Contains a list of <see cref="Glyph"/> entities sorted by
    /// their unicode character.
    /// </summary>
    public readonly struct Font : IFont
    {
        private readonly Entity entity;

        public readonly FixedString FamilyName => entity.GetComponentRef<FontName>().familyName;
        public readonly uint LineHeight => entity.GetComponentRef<FontMetrics>().lineHeight;
        public readonly uint GlyphCount => entity.GetArrayLength<FontGlyph>();

        public readonly Glyph this[uint index]
        {
            get
            {
                FontGlyph glyph = entity.GetArrayElementRef<FontGlyph>(index);
                rint glyphReference = glyph.value;
                uint glyphEntity = entity.GetReference(glyphReference);
                return new(entity, glyphEntity);
            }
        }

        World IEntity.World => entity;
        uint IEntity.Value => entity;

#if NET
        [Obsolete("Default constructor not available", true)]
        public Font()
        {
            throw new Exception();
        }
#endif

        public Font(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Font(World world, ReadOnlySpan<char> address)
        {
            entity = new(world);
            entity.AddComponent(new IsDataRequest(address));
            entity.AddComponent(new IsFontRequest());
        }

        public Font(World world, FixedString address)
        {
            entity = new(world);
            entity.AddComponent(new IsDataRequest(address));
            entity.AddComponent(new IsFontRequest());
        }

        public override string ToString()
        {
            Span<char> buffer = stackalloc char[256];
            int length = ToString(buffer);
            return new string(buffer[..length]);
        }

        public readonly int ToString(Span<char> buffer)
        {
            int length = FamilyName.ToString(buffer);
            buffer[length++] = ' ';
            buffer[length++] = '(';
            buffer[length++] = '`';
            length += entity.ToString(buffer[length..]);
            buffer[length++] = '`';
            buffer[length++] = ')';
            return length;
        }

        Query IEntity.GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsFont>());
        }

        public static implicit operator Entity(Font font)
        {
            return font.entity;
        }
    }
}