using Simulation;

namespace Fonts.Components
{
    public readonly struct FontAtlas
    {
        public readonly EntityID value;

        public FontAtlas(EntityID value)
        {
            this.value = value;
        }
    }
}
