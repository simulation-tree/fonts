using Unmanaged;

namespace Fonts.Components
{
    public readonly struct FontName
    {
        public readonly FixedString familyName;

        public FontName(USpan<char> familyName)
        {
            this.familyName = new(familyName);
        }

        public FontName(FixedString familyName)
        {
            this.familyName = familyName;
        }
    }
}
