using Data;
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
        public const uint DefaultPixelSize = 32;
        public const float FixedPointScale = 64f;

        private readonly Entity entity;

        public readonly FixedString FamilyName => entity.GetComponent<FontName>().familyName;
        public readonly uint LineHeight => entity.GetComponent<FontMetrics>().lineHeight;
        public readonly uint GlyphCount => entity.GetArrayLength<FontGlyph>();
        public readonly ref uint PixelSize => ref entity.GetComponent<IsFontRequest>().pixelSize;

        public readonly Glyph this[uint index]
        {
            get
            {
                FontGlyph glyph = entity.GetArrayElement<FontGlyph>(index);
                rint glyphReference = glyph.value;
                uint glyphEntity = entity.GetReference(glyphReference);
                return new(entity.world, glyphEntity);
            }
        }

        readonly World IEntity.World => entity.world;
        readonly uint IEntity.Value => entity.value;

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsFont>();
            archetype.AddArrayElementType<FontGlyph>();
        }

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

        public Font(World world, Address address, uint pixelSize = DefaultPixelSize)
        {
            entity = new Entity<IsDataRequest, IsFontRequest>(world, new(address), new(0, pixelSize));
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

        public readonly Vector2 CalulcateSize(USpan<char> text)
        {
            USpan<Vector3> temp = stackalloc Vector3[(int)(text.Length * 4)];
            return GenerateVertices(text, temp).maxPosition;
        }

        /// <summary>
        /// Retrieves the index to the closest character at the given vertex position.
        /// </summary>
        public readonly bool TryIndexOf(USpan<char> text, Vector2 vertexPosition, out uint index)
        {
            if (text.Length == 0)
            {
                index = 0;
                return false;
            }

            USpan<Vector3> temp = stackalloc Vector3[(int)(text.Length * 4)];
            (Vector2 maxPosition, uint vertexCount) = GenerateVertices(text, temp);
            float closestDistance = float.MaxValue;
            uint closestIndex = 0;
            for (uint i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    continue;
                }
                else if (c == '\r')
                {
                    if (i < text.Length - 1 && text[i + 1] == '\n')
                    {
                        i++;
                    }

                    continue;
                }

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

        public readonly (Vector2 maxPosition, uint vertexCount) GenerateVertices(USpan<char> text, USpan<Vector3> vertices)
        {
            uint lineHeight = LineHeight;
            int penX = 0;
            int penY = 0;
            uint pixelSize = entity.GetComponent<IsFontRequest>().pixelSize;
            World world = entity.GetWorld();
            USpan<FontGlyph> glyphs = entity.GetArray<FontGlyph>();
            uint vertexIndex = 0;
            for (uint i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    penX = 0;
                    penY -= (int)(lineHeight * (pixelSize / 32f));
                    continue;
                }
                else if (c == '\r')
                {
                    penX = 0;
                    penY -= (int)(lineHeight * (pixelSize / 32f));
                    if (i < text.Length - 1 && text[i + 1] == '\n')
                    {
                        i++;
                    }

                    continue;
                }

                rint glyphReference = glyphs[c].value;
                uint glyphEntity = entity.GetReference(glyphReference);
                IsGlyph glyph = world.GetComponent<IsGlyph>(glyphEntity);
                Vector2 size = GetGlyphSize(glyph);
                Vector2 origin = GetGlyphOrigin(penX, penY, glyph);
                Vector2 first = origin;
                Vector2 second = origin + new Vector2(size.X, 0);
                Vector2 third = origin + new Vector2(size.X, size.Y);
                Vector2 fourth = origin + new Vector2(0, size.Y);

                (int x, int y) glyphAdvance = glyph.advance;
                penX += glyphAdvance.x;
                //penY += advance.y / pixelSize;

                vertices[vertexIndex + 0] = new Vector3(first, 0) / FixedPointScale / pixelSize;
                vertices[vertexIndex + 1] = new Vector3(second, 0) / FixedPointScale / pixelSize;
                vertices[vertexIndex + 2] = new Vector3(third, 0) / FixedPointScale / pixelSize;
                vertices[vertexIndex + 3] = new Vector3(fourth, 0) / FixedPointScale / pixelSize;

                vertexIndex += 4;
            }

            Vector2 maxPosition = new Vector2(penX, -penY) / FixedPointScale / pixelSize;
            return (maxPosition, vertexIndex);
        }

        private static Vector2 GetGlyphOrigin(int penX, int penY, IsGlyph glyph)
        {
            (int x, int y) glyphOffset = glyph.offset;
            (int x, int y) glyphSize = glyph.size;
            (int x, int y) glyphBearing = glyph.bearing;
            Vector2 origin = new(penX, penY);
            origin += new Vector2(glyphOffset.x, glyphOffset.y);
            origin.Y -= glyphSize.y - glyphBearing.y;
            return origin;
        }

        private static Vector2 GetGlyphSize(IsGlyph glyph)
        {
            (int x, int y) glyphSize = glyph.size;
            return new Vector2(glyphSize.x, glyphSize.y);
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