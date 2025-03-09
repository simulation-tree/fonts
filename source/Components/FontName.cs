using System;
using Unmanaged;

namespace Fonts.Components
{
    public readonly struct FontName
    {
        public readonly ASCIIText256 familyName;

        public FontName(Span<char> familyName)
        {
            this.familyName = new(familyName);
        }

        public FontName(ASCIIText256 familyName)
        {
            this.familyName = familyName;
        }
    }
}
