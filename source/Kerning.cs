using System.Numerics;
using Unmanaged;
using Worlds;

namespace Fonts
{
    [ArrayElement]
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
            return buffer.Slice(0, length).ToString();
        }

        public readonly uint ToString(USpan<char> buffer)
        {
            uint length = 0;
            length += "Character: ".AsUSpan().CopyTo(buffer);
            buffer[length++] = nextCharacter;
            length += ", Amount: ".AsUSpan().CopyTo(buffer.Slice(length));
            length += amount.ToString(buffer.Slice(length));
            return length;
        }
    }
}