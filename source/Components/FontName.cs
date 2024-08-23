using System;
using Unmanaged;

namespace Fonts.Components
{
    public readonly struct FontName
    {
        public readonly FixedString familyName;

        public FontName(ReadOnlySpan<char> familyName)
        {
            this.familyName = new FixedString(familyName);
        }

        public FontName(FixedString familyName)
        {
            this.familyName = familyName;
        }
    }
}
