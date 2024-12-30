using Worlds;

namespace Fonts.Components
{
    [ArrayElement]
    public readonly struct FontGlyph
    {
        public readonly rint value;

        public FontGlyph(rint value)
        {
            this.value = value;
        }
    }
}
