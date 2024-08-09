using Fonts.Components;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;
using Unmanaged.Collections;

namespace Fonts
{
    public readonly struct Glyph : IGlyph, IDisposable
    {
        private readonly Entity entity;

        eint IEntity.Value => entity.GetEntityValue();
        World IEntity.World => entity.GetWorld();

        public Glyph(World world, eint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Glyph(World world, char character, Vector2 advance, Vector2 offset, Vector2 size, Vector4 region, ReadOnlySpan<Kerning> kernings)
        {
            this.entity = new(world);
            entity.AddComponent(new IsGlyph(character, advance, offset, size, region));

            UnmanagedList<Kerning> kerningsList = entity.CreateList<Entity, Kerning>((uint)(kernings.Length + 1));
            kerningsList.AddRange(kernings);
        }

        public readonly override string ToString()
        {
            char character = this.GetCharacter();
            Span<char> buffer = ['\'', character, '\''];
            return new string(buffer);
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        static Query IEntity.GetQuery(World world)
        {
            return new Query(world, RuntimeType.Get<IsGlyph>());
        }
    }
}