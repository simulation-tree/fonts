namespace Fonts.Components
{
    public struct IsFont
    {
        public uint version;
        public uint pixelSize;

        public IsFont(uint version, uint pixelSize)
        {
            this.version = version;
            this.pixelSize = pixelSize;
        }
    }
}
