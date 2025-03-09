using System;
using System.Numerics;

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
            Span<char> buffer = stackalloc char[128];
            int length = ToString(buffer);
            return buffer.Slice(0, length).ToString();
        }

        public readonly int ToString(Span<char> destination)
        {
            ReadOnlySpan<char> template = "Character: ";
            template.CopyTo(destination);
            int length = template.Length;
            destination[length++] = nextCharacter;
            template = ", Amount: ".AsSpan();
            template.CopyTo(destination.Slice(length));
            length += template.Length;
            length += amount.ToString(destination.Slice(length));
            return length;
        }
    }
}