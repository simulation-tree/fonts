using Worlds;

namespace Fonts.Components
{
    [Component]
    public readonly struct IsFont
    {
        public readonly uint version;
        public readonly uint pixelSize;

        public IsFont(uint version, uint pixelSize)
        {
            this.version = version;
            this.pixelSize = pixelSize;
        }

        public readonly IsFont IncrementVersion()
        {
            return new IsFont(version + 1, pixelSize);
        }

        public readonly IsFont IncrementVersion(uint pixelSize)
        {
            return new IsFont(version + 1, pixelSize);
        }
    }
}
