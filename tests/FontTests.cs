using Types;
using Worlds;
using Worlds.Tests;

namespace Fonts.Tests
{
    public abstract class FontTests : WorldTests
    {
        static FontTests()
        {
            TypeRegistry.Load<FontsTypeBank>();
        }

        protected override Schema CreateSchema()
        {
            Schema schema = base.CreateSchema();
            schema.Load<FontsSchemaBank>();
            return schema;
        }
    }
}