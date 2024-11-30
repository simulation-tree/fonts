using Worlds;

namespace Fonts.Components
{
    [Array]
    public readonly struct FontGlyph
    {
        public readonly rint value;

        public FontGlyph(rint value)
        {
            this.value = value;
        }
    }
}
