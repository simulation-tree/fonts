using Data.Components;
using Data.Events;
using Fonts.Components;
using Fonts.Events;
using Simulation;
using System;
using Unmanaged;

namespace Fonts
{
    public readonly struct Font : IFont, IDisposable
    {
        private readonly Entity entity;

        World IEntity.World => entity.world;
        eint IEntity.Value => entity.value;

        public Font()
        {
            throw new InvalidOperationException("Cannot create a font without a world.");
        }

        public Font(World world, eint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Font(World world, ReadOnlySpan<char> address)
        {
            entity = new(world);
            entity.AddComponent(new IsDataRequest(address));
            entity.AddComponent(new IsFont());

            world.Submit(new DataUpdate());
            world.Submit(new FontUpdate());
            world.Poll();
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public override string ToString()
        {
            return this.GetFamilyName().ToString();
        }

        Query IEntity.GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsFont>());
        }
    }
}