using System;
using System.Numerics;
using Unmanaged;

namespace Fonts
{
    public readonly struct Kerning
    {
        public readonly char nextCharacter;
        public readonly Vector2 amount;

        public Kerning(char nextCharacter, Vector2 amount)
        {
            this.nextCharacter = nextCharacter;
            this.amount = amount;
        }

        public readonly override string ToString()
        {
            USpan<char> buffer = stackalloc char[128];
            uint length = ToString(buffer);
            return buffer.GetSpan(length).ToString();
        }

        public readonly uint ToString(USpan<char> buffer)
        {
            USpan<char> template = "Character: ".AsSpan();
            template.CopyTo(buffer);
            uint length = template.Length;
            buffer[length++] = nextCharacter;
            template = ", Amount: ".AsSpan();
            template.CopyTo(buffer.Slice(length));
            length += template.Length;
            length += amount.ToString(buffer.Slice(length));
            return length;
        }
    }
}