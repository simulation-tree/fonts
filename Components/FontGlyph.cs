using Simulation;

namespace Fonts.Components
{
    public readonly struct FontGlyph
    {
        public readonly EntityID value;

        public FontGlyph(EntityID value)
        {
            this.value = value;
        }
    }
}
