using Worlds;

namespace Fonts.Tests
{
    public class FontEntityTests : FontTests
    {
        [Test]
        public void CheckIfFontEntityIs()
        {
            using World world = CreateWorld();
            Font font = new(world, []);
            Assert.That(font.Is(), Is.True);
        }
    }
}