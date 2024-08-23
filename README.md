# Fonts
Definitions for TTF/OTF fonts and their glyphs. This does not provide rendering or atlasing, only
typographic information.

### Importing fonts
```cs
using World world = new();
Font font = new(world, "*/Arial.ttf");
while (!font.Is())
{
    world.Submit(new DataUpdate()); //to load the bytes
    world.Submit(new FontUpdate()); //load import the font from the bytes
    world.Poll();
}

Assert.That(font.FamilyName.ToString(), Is.EqualTo("Arial"));
Assert.That(font.GlyphCount, Is.GreaterThan(0));
Assert.That(font.LineHeight, Is.EqualTo(2355));

Glyph a = font['a'];
Assert.That(a.Character, Is.EqualTo('a'));
Assert.That(a.Advance, Is.EqualTo((1152, 1920)));
Assert.That(a.Offset, Is.EqualTo((1, 17)));
Assert.That(a.Size, Is.EqualTo((1024, 1088)));
```