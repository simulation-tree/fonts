using System.Numerics;

namespace Fonts.Components
{
    public readonly struct IsGlyph
    {
        public readonly char character;
        public readonly Vector2 advance;
        public readonly Vector2 bearing;
        public readonly Vector2 offset;
        public readonly Vector2 size;

        public IsGlyph(char character, Vector2 advance, Vector2 bearing, Vector2 offset, Vector2 size)
        {
            this.character = character;
            this.advance = advance;
            this.bearing = bearing;
            this.offset = offset;
            this.size = size;
        }
    }
}
