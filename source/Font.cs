using Data.Components;
using Fonts.Components;
using System;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Fonts
{
    /// <summary>
    /// Contains a list of <see cref="Glyph"/> entities sorted by
    /// their unicode character.
    /// </summary>
    public readonly struct Font : IFont, IEquatable<Font>
    {
        private readonly Entity entity;

        public readonly FixedString FamilyName => entity.GetComponentRef<FontName>().familyName;
        public readonly uint LineHeight => entity.GetComponentRef<FontMetrics>().lineHeight;
        public readonly uint GlyphCount => entity.GetArrayLength<FontGlyph>();

        public readonly Glyph this[uint index]
        {
            get
            {
                FontGlyph glyph = entity.GetArrayElementRef<FontGlyph>(index);
                rint glyphReference = glyph.value;
                uint glyphEntity = entity.GetReference(glyphReference);
                return new(entity.world, glyphEntity);
            }
        }

        readonly World IEntity.World => entity.world;
        readonly uint IEntity.Value => entity.value;
        readonly Definition IEntity.Definition => new Definition().AddComponentType<IsFont>().AddArrayType<FontGlyph>();

#if NET
        [Obsolete("Default constructor not available", true)]
        public Font()
        {
            throw new NotImplementedException();
        }
#endif

        public Font(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        public Font(World world, USpan<char> address)
        {
            entity = new(world);
            entity.AddComponent(new IsDataRequest(address));
            entity.AddComponent(new IsFontRequest());
        }

        public Font(World world, FixedString address)
        {
            entity = new(world);
            entity.AddComponent(new IsDataRequest(address));
            entity.AddComponent(new IsFontRequest());
        }

        public Font(World world, string address)
        {
            entity = new(world);
            entity.AddComponent(new IsDataRequest(address));
            entity.AddComponent(new IsFontRequest());
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public unsafe readonly override string ToString()
        {
            USpan<char> buffer = stackalloc char[256];
            uint length = ToString(buffer);
            return buffer.Slice(0, length).ToString();
        }

        public readonly uint ToString(USpan<char> buffer)
        {
            uint length = FamilyName.CopyTo(buffer);
            buffer[length++] = ' ';
            buffer[length++] = '(';
            buffer[length++] = '`';
            length += entity.ToString(buffer.Slice(length));
            buffer[length++] = '`';
            buffer[length++] = ')';
            return length;
        }

        public readonly Vector2 CalulcateSize(USpan<char> text, uint pixelSize)
        {
            USpan<Vector3> temp = stackalloc Vector3[(int)(text.Length * 4)];
            return GenerateVertices(text, temp, pixelSize);
        }

        /// <summary>
        /// Retrieves the index to the closest character at the given vertex position.
        /// </summary>
        public readonly bool TryIndexOf(USpan<char> text, uint pixelSize, Vector2 vertexPosition, out uint index)
        {
            if (text.Length == 0)
            {
                index = 0;
                return false;
            }

            USpan<Vector3> temp = stackalloc Vector3[(int)(text.Length * 4)];
            Vector2 maxPosition = GenerateVertices(text, temp, pixelSize);
            float closestDistance = float.MaxValue;
            uint closestIndex = 0;
            for (uint i = 0; i < text.Length; i++)
            {
                Vector3 first = temp[(i * 4) + 0];
                float distanceSquared = Vector2.DistanceSquared(vertexPosition, new Vector2(first.X, first.Y));
                if (distanceSquared < closestDistance)
                {
                    closestDistance = distanceSquared;
                    closestIndex = i;
                }
            }

            index = closestIndex;
            return true;
        }

        public readonly Vector2 GenerateVertices(USpan<char> text, USpan<Vector3> vertices, uint pixelSize)
        {
            uint lineHeight = LineHeight;
            int penX = 0;
            int penY = 0;
            Vector2 maxPosition = default;
            World world = entity.GetWorld();
            USpan<FontGlyph> glyphs = entity.GetArray<FontGlyph>();
            for (uint i = 0; i < text.Length; i++)
            {
                char c = text[i];
                rint glyphReference = glyphs[c].value;
                uint glyphEntity = entity.GetReference(glyphReference);
                IsGlyph glyph = world.GetComponent<IsGlyph>(glyphEntity);
                if (c == '\n')
                {
                    penX = 0;
                    penY -= (int)lineHeight;
                    continue;
                }

                (int x, int y) glyphOffset = glyph.offset;
                (int x, int y) glyphAdvance = glyph.advance;
                (int x, int y) glyphSize = glyph.size;
                (int x, int y) glyphBearing = glyph.bearing;
                float glyphWidth = glyphSize.x;
                float glyphHeight = glyphSize.y;
                Vector2 origin = new(penX + (glyphOffset.x), penY + (glyphOffset.y));
                origin.Y -= (glyphSize.y - glyphBearing.y);
                Vector2 size = new(glyphWidth, glyphHeight);
                origin /= 64f; //why is this divided by 64 again?
                size /= 64f;
                Vector2 first = origin;
                Vector2 second = origin + new Vector2(size.X, 0);
                Vector2 third = origin + new Vector2(size.X, size.Y);
                Vector2 fourth = origin + new Vector2(0, size.Y);
                penX += glyphAdvance.x;
                //penY += advance.y / pixelSize;

                vertices[(i * 4) + 0] = new Vector3(first, 0) / pixelSize;
                vertices[(i * 4) + 1] = new Vector3(second, 0) / pixelSize;
                vertices[(i * 4) + 2] = new Vector3(third, 0) / pixelSize;
                vertices[(i * 4) + 3] = new Vector3(fourth, 0) / pixelSize;

                maxPosition = Vector2.Max(maxPosition, first);
                maxPosition = Vector2.Max(maxPosition, second);
                maxPosition = Vector2.Max(maxPosition, third);
                maxPosition = Vector2.Max(maxPosition, fourth);
            }

            return maxPosition;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is Font font && Equals(font);
        }

        public readonly bool Equals(Font other)
        {
            return entity.Equals(other.entity);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(entity);
        }

        public static bool operator ==(Font left, Font right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Font left, Font right)
        {
            return !(left == right);
        }

        public static implicit operator Entity(Font font)
        {
            return font.entity;
        }
    }
}