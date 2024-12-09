using Worlds;

namespace Fonts.Components
{
    [Component]
    public struct IsFontRequest
    {
        public uint version;
        public uint pixelSize;

        public IsFontRequest(uint version, uint pixelSize)
        {
            this.version = version;
            this.pixelSize = pixelSize;
        }
    }
}