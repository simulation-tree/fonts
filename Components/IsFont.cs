namespace Fonts.Components
{
    public struct IsFont
    {
        public bool changed;

        public IsFont()
        {
            changed = true;
        }

        public IsFont(bool changed)
        {
            this.changed = changed;
        }
    }
}
