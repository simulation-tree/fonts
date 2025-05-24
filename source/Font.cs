using Fonts.Components;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unmanaged;
using Worlds;

namespace Fonts
{
    /// <summary>
    /// Contains a list of <see cref="Glyph"/> entities sorted by
    /// their unicode character.
    /// </summary>
    public readonly partial struct Font : IEntity
    {
        public const uint DefaultPixelSize = 32;
        public const float FixedPointScale = 64f;

        public readonly bool IsLoaded
        {
            get
            {
                if (TryGetComponent(out IsFontRequest request))
                {
                    return request.status == IsFontRequest.Status.Loaded;
                }

                return IsCompliant;
            }
        }

        public readonly ASCIIText256 FamilyName => GetComponent<FontName>().familyName;
        public readonly uint LineHeight => GetComponent<FontMetrics>().lineHeight;
        public readonly int GlyphCount => GetArrayLength<FontGlyph>();
        public readonly uint PixelSize => GetComponent<IsFont>().pixelSize;

        public readonly Glyph this[int index]
        {
            get
            {
                FontGlyph glyph = GetArrayElement<FontGlyph>(index);
                rint glyphReference = glyph.value;
                uint glyphEntity = GetReference(glyphReference);
                return new Entity(world, glyphEntity).As<Glyph>();
            }
        }

        /// <summary>
        /// Creates an empty font.
        /// </summary>
        public Font(World world, ReadOnlySpan<Glyph> glyphs, uint pixelSize = DefaultPixelSize)
        {
            this.world = world;
            value = world.CreateEntity(new IsFont(0, pixelSize));
            Values<FontGlyph> fontGlyphs = CreateArray<FontGlyph>(glyphs.Length);
            for (int i = 0; i < glyphs.Length; i++)
            {
                Glyph glyph = glyphs[i];
                fontGlyphs[i] = new FontGlyph(AddReference(glyph));
            }
        }

        /// <summary>
        /// Creates a request for a font from the given <paramref name="address"/>.
        /// </summary>
        public Font(World world, ASCIIText256 address, uint pixelSize = DefaultPixelSize, double timeout = default)
        {
            this.world = world;
            value = world.CreateEntity(new IsFontRequest(pixelSize, address, timeout));
        }

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsFont>();
            archetype.AddArrayType<FontGlyph>();
        }

        public unsafe readonly override string ToString()
        {
            Span<char> buffer = stackalloc char[256];
            int length = ToString(buffer);
            return buffer.Slice(0, length).ToString();
        }

        public readonly int ToString(Span<char> destination)
        {
            int length = FamilyName.CopyTo(destination);
            destination[length++] = ' ';
            destination[length++] = '(';
            destination[length++] = '`';
            length += value.ToString(destination.Slice(length));
            destination[length++] = '`';
            destination[length++] = ')';
            return length;
        }

        [SkipLocalsInit]
        public readonly Vector2 CalulcateSize(ReadOnlySpan<char> text)
        {
            Span<Vector3> temp = stackalloc Vector3[text.Length * 4];
            return GenerateVertices(text, temp).maxPosition;
        }

        /// <summary>
        /// Retrieves the index to the closest character at the given vertex position.
        /// </summary>
        [SkipLocalsInit]
        public readonly bool TryIndexOf(ReadOnlySpan<char> text, Vector2 vertexPosition, out int index)
        {
            if (text.Length == 0)
            {
                index = 0;
                return false;
            }

            Span<Vector3> temp = stackalloc Vector3[text.Length * 4];
            (Vector2 maxPosition, int vertexCount) = GenerateVertices(text, temp);
            float closestDistance = float.MaxValue;
            int closestIndex = 0;
            for (int i = 0; i < text.Length; i++)
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

        public readonly (Vector2 maxPosition, int vertexCount) GenerateVertices(ReadOnlySpan<char> text, Span<Vector3> vertices)
        {
            Values<FontGlyph> glyphs = GetArray<FontGlyph>();
            return GenerateVertices(world, value, text, vertices, LineHeight, PixelSize, glyphs);
        }

        public static (Vector2 maxPosition, int vertexCount) GenerateVertices(World world, uint font, ReadOnlySpan<char> text, Span<Vector3> vertices, uint lineHeight, uint pixelSize, ReadOnlySpan<FontGlyph> glyphs)
        {
            Vector2 cursor = default;
            int vertexIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    cursor.X = 0;
                    cursor.Y -= (int)(lineHeight * (pixelSize / 32f));
                    continue;
                }
                else if (c == '\r')
                {
                    cursor.X = 0;
                    cursor.Y -= (int)(lineHeight * (pixelSize / 32f));
                    if (i < text.Length - 1 && text[i + 1] == '\n')
                    {
                        i++;
                    }

                    continue;
                }

                rint glyphReference;
                if (c < glyphs.Length)
                {
                    glyphReference = glyphs[c].value;
                }
                else
                {
                    glyphReference = glyphs['?'].value;
                }

                uint glyphEntity = world.GetReference(font, glyphReference);
                IsGlyph glyph = world.GetComponent<IsGlyph>(glyphEntity);
                Vector2 size = glyph.Size;
                Vector2 origin = GetGlyphOrigin(cursor, glyph);
                Vector2 first = origin;
                Vector2 second = origin + new Vector2(size.X, 0);
                Vector2 third = origin + new Vector2(size.X, size.Y);
                Vector2 fourth = origin + new Vector2(0, size.Y);

                Vector2 glyphAdvance = glyph.Advance;
                cursor.X += glyphAdvance.X;
                //penY += advance.y / pixelSize;

                vertices[vertexIndex + 0] = new Vector3(first, 0) / FixedPointScale / pixelSize;
                vertices[vertexIndex + 1] = new Vector3(second, 0) / FixedPointScale / pixelSize;
                vertices[vertexIndex + 2] = new Vector3(third, 0) / FixedPointScale / pixelSize;
                vertices[vertexIndex + 3] = new Vector3(fourth, 0) / FixedPointScale / pixelSize;

                vertexIndex += 4;
            }

            Vector2 maxPosition = new Vector2(cursor.X, -cursor.Y) / FixedPointScale / pixelSize;
            return (maxPosition, vertexIndex);
        }

        private static Vector2 GetGlyphOrigin(Vector2 cursor, IsGlyph glyph)
        {
            Vector2 glyphOffset = glyph.Offset;
            Vector2 glyphSize = glyph.Size;
            Vector2 glyphBearing = glyph.Bearing;
            Vector2 origin = cursor;
            origin += glyphOffset;
            origin.Y -= glyphSize.Y - glyphBearing.Y;
            return origin;
        }
    }
}