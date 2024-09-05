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
        public readonly Entity entity;

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
                return new(entity.world, glyphEntity);
            }
        }

        readonly World IEntity.World => entity.world;
        readonly uint IEntity.Value => entity.value;
        readonly Definition IEntity.Definition => new([RuntimeType.Get<IsFont>()], [RuntimeType.Get<FontGlyph>()]);

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

        public Font(World world, USpan<char> address)
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

        public Font(World world, string address)
        {
            entity = new(world);
            entity.AddComponent(new IsDataRequest(address));
            entity.AddComponent(new IsFontRequest());
        }

        public unsafe readonly override string ToString()
        {
            USpan<char> buffer = stackalloc char[256];
            uint length = ToString(buffer);
            return new string(buffer.pointer, 0, (int)length);
        }

        public readonly uint ToString(USpan<char> buffer)
        {
            uint length = FamilyName.CopyTo(buffer);
            buffer[length++] = ' ';
            buffer[length++] = '(';
            buffer[length++] = '`';
            length += entity.ToString(buffer.Slice(length));
            buffer[length++] = '`';
            buffer[length++] = ')';
            return length;
        }
    }
}