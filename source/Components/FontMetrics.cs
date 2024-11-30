using Worlds;

namespace Fonts.Components
{
    [Component]
    public readonly struct FontMetrics
    {
        public readonly uint lineHeight;

        public FontMetrics(uint lineHeight)
        {
            this.lineHeight = lineHeight;
        }
    }
}
