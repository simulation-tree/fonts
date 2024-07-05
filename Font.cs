using Data.Components;
using Data.Events;
using Fonts.Components;
using Fonts.Events;
using Simulation;
using Textures;
using Unmanaged;

namespace Fonts
{
    public readonly struct Font : IDisposable
    {
        public readonly Entity entity;

        public readonly FixedString FamilyName => entity.GetComponent<FontName>().familyName;
        public readonly float LineHeight => entity.GetComponent<FontMetrics>().lineHeight;
        public readonly bool IsDestroyed => entity.IsDestroyed;
        public readonly AtlasTexture Atlas => new(entity.world, entity.GetComponent<FontAtlas>().value);
        public readonly uint GlyphCount => entity.GetCollection<FontGlyph>().Count;
        public readonly Glyph this[uint index] => new(entity.world, entity.GetCollection<FontGlyph>()[index].value);

        public Font()
        {
            throw new InvalidOperationException("Cannot create a font without a world.");
        }

        public Font(World world, ReadOnlySpan<char> address)
        {
            entity = new(world);
            entity.AddComponent(new IsDataRequest(address));
            entity.AddComponent(new IsFont());

            world.Submit(new DataUpdate());
            world.Submit(new FontUpdate());
            world.Poll();
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public override string ToString()
        {
            return FamilyName.ToString();
        }
    }
}