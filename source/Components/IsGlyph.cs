using Worlds;

namespace Fonts.Components
{
    [Component]
    public readonly struct IsGlyph
    {
        public readonly char character;
        public readonly (int x, int y) advance;
        public readonly (int x, int y) bearing;
        public readonly (int x, int y) offset;
        public readonly (int x, int y) size;

        public IsGlyph(char character, (int x, int y) advance, (int x, int y) bearing, (int x, int y) offset, (int x, int y) size)
        {
            this.character = character;
            this.advance = advance;
            this.bearing = bearing;
            this.offset = offset;
            this.size = size;
        }
    }
}
