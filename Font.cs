using Data.Components;
using Fonts.Components;
using Simulation;
using System;
using Unmanaged;

namespace Fonts
{
    public readonly struct Font : IFont, IDisposable
    {
        private readonly Entity entity;

        public readonly FixedString FamilyName => entity.GetComponent<FontName>().familyName;
        public readonly float LineHeight => entity.GetComponent<FontMetrics>().lineHeight;
        public readonly uint GlyphCount => entity.GetList<FontGlyph>().Count;
        public readonly bool IsLoaded => entity.ContainsComponent<IsFont>();

        public readonly Glyph this[uint index]
        {
            get
            {
                FontGlyph glyph = entity.GetListElement<FontGlyph>(index);
                rint glyphReference = glyph.value;
                eint glyphEntity = entity.GetReference(glyphReference);
                return new(entity.world, glyphEntity);
            }
        }

        World IEntity.World => entity.world;
        eint IEntity.Value => entity.value;

#if NET
        [Obsolete("Default constructor not available", true)]
        public Font()
        {
            throw new Exception();
        }
#endif

        public Font(World world, eint existingEntity)
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

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public override string ToString()
        {
            return this.FamilyName.ToString();
        }

        Query IEntity.GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsFont>());
        }
    }
}