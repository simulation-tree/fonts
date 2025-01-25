using Types;
using Worlds;
using Worlds.Tests;

namespace Fonts.Tests
{
    public abstract class FontTests : WorldTests
    {
        static FontTests()
        {
            TypeRegistry.Load<Fonts.TypeBank>();
        }

        protected override Schema CreateSchema()
        {
            Schema schema = base.CreateSchema();
            schema.Load<Fonts.SchemaBank>();
            return schema;
        }
    }
}