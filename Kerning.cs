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
            return $"Character: {nextCharacter}, Amount: {amount}";
        }
    }
}