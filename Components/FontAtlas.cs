using Simulation;
using Textures;

namespace Fonts.Components
{
    public readonly struct FontAtlas
    {
        public readonly eint value;

        public FontAtlas(eint value)
        {
            this.value = value;
        }

        public FontAtlas(Texture texture)
        {
            value = texture.GetEntityValue();
        }
    }
}
