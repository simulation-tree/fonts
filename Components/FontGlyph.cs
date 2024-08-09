using Simulation;

namespace Fonts.Components
{
    public readonly struct FontGlyph
    {
        public readonly eint value;

        public FontGlyph(Glyph glyph)
        {
            this.value = glyph.GetEntityValue();
        }
    }
}
