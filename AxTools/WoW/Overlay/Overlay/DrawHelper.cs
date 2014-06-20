//!CompilerOption:AddRef:SlimDx.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;
using SlimDX;
using SlimDX.Direct3D9;
using Styx;
using Styx.CommonBot.Inventory;
using Tripper.Tools.Math;
using Color = System.Drawing.Color;
using Font = SlimDX.Direct3D9.Font;
using FontFamily = System.Drawing.FontFamily;
using Matrix = SlimDX.Matrix;
using Vector3 = SlimDX.Vector3;

namespace AeroOverlayHonorbuddy.Overlay
{
    public static class DrawHelper
    {
        private static Sprite _fontSprite;
        private static Line _line;

        /// <summary> Gets the device.</summary>
        /// <value> The device.</value>
        private static Device Device { get { return OverlayManager.Device; }}

        /// <summary> Draws text. </summary>
        /// <param name="text">      The text. </param>
        /// <param name="x">         The x coordinate. </param>
        /// <param name="y">         The y coordinate. </param>
        /// <param name="color">     The color. </param>
        /// <param name="emSize">    (optional) size of the em. </param>
        /// <param name="fontStyle"> (optional) the font style. </param>
        public static void DrawText(string text, int x, int y, Color color, float emSize = 12f, FontStyle fontStyle = FontStyle.Regular)
        {
            if (_fontSprite == null)
                _fontSprite = new Sprite(Device);

            _fontSprite.Begin(SpriteFlags.AlphaBlend);
            using (Font font = new Font(Device, new System.Drawing.Font(FontFamily.GenericMonospace, emSize, fontStyle)))
            {
                Rectangle stringRect = font.MeasureString(_fontSprite, text, DrawTextFormat.Left);
                Rectangle rect = new Rectangle(x, y, stringRect.Width + 1, stringRect.Height + 1);

                font.DrawString(_fontSprite, text, rect, DrawTextFormat.Left, color.ToArgb());
                _fontSprite.End();
            }
        }

        private static Dictionary<string, Mesh> _textMeshCache = new Dictionary<string, Mesh>();

        /// <summary> Draw 3d text.</summary>
        /// <remarks> Nesox, 2013-12-28.</remarks>
        /// <param name="text">      The text. </param>
        /// <param name="textPos">   The text position. </param>
        /// <param name="emSize">    (optional) size of the em. </param>
        /// <param name="fontStyle"> (optional) the font style. </param>
        public static void Draw3DText(string text, Vector3 textPos, float emSize = 12f,
            FontStyle fontStyle = FontStyle.Regular)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("text must be specified", "text");

            Device.SetTransform(TransformState.World, Matrix.Translation(textPos.X, textPos.Y, textPos.Z));

            Mesh m;
            if (!_textMeshCache.TryGetValue(text, out m))
                _textMeshCache[text] = Mesh.CreateText(Device, new System.Drawing.Font("Verdana", emSize, fontStyle), text, 0.001f, 0.01f);

            var mesh = _textMeshCache[text];
            mesh.DrawSubset(0);
        }

        /// <summary> Draw shadowed text. </summary>
        /// <remarks> Nesox, 2013-06-14. </remarks>
        /// <param name="text">        The text. </param>
        /// <param name="x">           The x coordinate. </param>
        /// <param name="y">           The y coordinate. </param>
        /// <param name="color">       The color. </param>
        /// <param name="shadowColor"> The shadow color. </param>
        /// <param name="emSize">      (optional) size of the em. </param>
        /// <param name="fontStyle">   (optional) the font style. </param>
        public static void DrawShadowedText(string text, int x, int y, Color color, Color shadowColor, float emSize = 12f, FontStyle fontStyle = FontStyle.Regular)
        {
            for (int i = -2; i <= 2; i++)
            {
                DrawText(text, x + i, y + i, shadowColor, emSize, fontStyle);
            }
            DrawText(text, x, y, color, emSize, fontStyle);
        }

        private static int[] _lineFaceIndices;
        private static ColoredVertex[] _lineVertices;
        /// <summary> Draws a line. </summary>
        /// <param name="vecStartPos"> The vector start position. </param>
        /// <param name="vecEndPos">   The vector end position. </param>
        /// <param name="width">       The width. </param>
        /// <param name="color">       The color. </param>
        public static void DrawLineEx(Vector3 vecStartPos, Vector3 vecEndPos, float width, Color color)
        {
            const int numCorners = 20;

            Vector3 start = new Vector3(0.5f, 0.0f, 0.0f);
            Vector3 end = new Vector3(-0.5f, 0.0f, 0.0f);

            if (_lineVertices == null)
            {
                _lineVertices = new ColoredVertex[numCorners * 2];

                for (int i = 0; i < numCorners; i++)
                {
                    Vector3 offset = new Vector3(0.0f, (float) Math.Cos((2.0f*Math.PI)/numCorners*i),
                        (float) Math.Sin((2.0f*(float) Math.PI)/numCorners*i));

                    Vector3 startPoint = start + offset;

                    ColoredVertex startVert = new ColoredVertex(new Vector3(startPoint.X, startPoint.Y, startPoint.Z), color.ToArgb());
                    _lineVertices[i] = startVert;

                    Vector3 endPoint = end + offset;
                    ColoredVertex endVert = new ColoredVertex(new Vector3(endPoint.X, endPoint.Y, endPoint.Z), color.ToArgb());
                    _lineVertices[numCorners + i] = endVert;
                }
            }

            if (_lineFaceIndices == null)
            {
                _lineFaceIndices = new int[numCorners * 2 * 3]; // The number of faces is the same as the number of corners: 2 tris per face, 3 inds per tri

                for (int i = 0; i < numCorners; i++)
                {
                    _lineFaceIndices[i * 6 + 0] = i; // ring 1, vert i
                    _lineFaceIndices[i * 6 + 1] = numCorners + i; // ring 2, vert i
                    _lineFaceIndices[i * 6 + 2] = (i + 1) % numCorners; // ring 1, vert i + 1

                    _lineFaceIndices[i * 6 + 3] = (i + 1) % numCorners; // ring 1, vert i + 1
                    _lineFaceIndices[i * 6 + 4] = i == numCorners - 1 ? numCorners : numCorners + i + 1; // ring 2, vert i + 1
                    _lineFaceIndices[i * 6 + 5] = numCorners + i; // ring 2, vert i
                }
            }
            
            Vector3 startToEnd = vecEndPos - vecStartPos;
            Matrix scaling = Matrix.Scaling(Vector3.Distance(vecStartPos, vecEndPos), width/2, width/2);
            
            Matrix rotationY = Matrix.RotationY(-(float)Math.Asin(startToEnd.Z / Vector3.Distance(vecStartPos, vecEndPos)));
            Matrix rotationZ = Matrix.RotationZ( (float)Math.Atan2(startToEnd.Y, startToEnd.X));

            Matrix rotation = Matrix.Multiply(rotationY, rotationZ);
            Matrix translation = Matrix.Translation(start.X + startToEnd.X / 2, start.Y + startToEnd.Y / 2, start.Z + startToEnd.Z / 2);

            Matrix world = Matrix.Multiply(Matrix.Multiply(scaling, rotation), translation);
            Device.SetTransform(TransformState.World, world);

            var oldDecl = Device.VertexDeclaration;
            using (var newDecl = ColoredVertex.GetDecl(Device))
            {
                Device.VertexDeclaration = newDecl;
                Device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, numCorners * 2, numCorners*2, _lineFaceIndices, Format.Index32, _lineVertices, ColoredVertex.Stride);
                Device.VertexDeclaration = oldDecl;
            }
        }

        /// <summary> Draws a line. </summary>
        /// <param name="vecStartPos"> The vector start position. </param>
        /// <param name="vecEndPos">   The vector end position. </param>
        /// <param name="width">       The width. </param>
        /// <param name="color">       The color. </param>
        public static void DrawLine(Vector3 vecStartPos, Vector3 vecEndPos, float width, Color color)
        {
            var cameraPos = StyxWoW.Camera.Position;
            Vector3 camPos = new Vector3(cameraPos.X, cameraPos.Y, cameraPos.Z);

            var forward = StyxWoW.Camera.Forward;
            Vector3 forwardPos = new Vector3(forward.X, forward.Y, forward.Z);

            float dot1 = Vector3.Dot((vecEndPos - camPos), forwardPos);
            float dot2 = Vector3.Dot((vecStartPos - camPos), forwardPos);

            // If the startPoint or endPoint is behind the camera,
            // then the bugged ID3DXLine:DrawTransform fails, so lets
            // prevent that.. shall we?
            if (dot1 < 0f || dot2 < 0f)
                return;

            if (_line == null)
                _line = new Line(Device);

            _line.Width = width;
            _line.Begin();
            _line.DrawTransformed(new[] { vecStartPos, vecEndPos }, Matrix.Identity * WoWCameraEx.View * WoWCameraEx.Projection, color);
            _line.End();
        }

        /// <summary> Draws a line. </summary>
        /// <param name="vecStartPos"> The vector start position. </param>
        /// <param name="vecEndPos">   The vector end position. </param>
        /// <param name="width">       The width. </param>
        /// <param name="color">       The color. </param>
        public static void DrawLineEx2(Vector3[] points, float width, Color color)
        {
            float halfWidth = width*0.5f;
            int segmentCount = points.Length/ 2;

            var positions = new Point3DCollection(segmentCount * 4);

            for (int i = 0; i < segmentCount; i++)
            {
                int startIndex = i * 2;
                Vector3 startPoint = points[startIndex];
                Vector3 endPoint = points[startIndex+1];
            }
        }

        /// <summary> Draw triangle. </summary>
        /// <remarks> Nesox, 2013-06-14. </remarks>
        /// <param name="pos">  my position. </param>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        /// <param name="color">  The color. </param>
        public static void DrawTriangle(Vector3 pos, float width, float height, Color color)
        {
            // Transformations.
            Device.SetTransform(TransformState.World, Matrix.Translation(pos.X, pos.Y, pos.Z));


            ColoredVertex[] triangleVerts =
                    {
                        new ColoredVertex(new Vector3(0f, 0f, height), color.ToArgb()),
                        new ColoredVertex(new Vector3(width, -width, 0f), color.ToArgb()),
                        new ColoredVertex(new Vector3(-width, width, 0f), color.ToArgb())
                    };

            var oldDecl = Device.VertexDeclaration;
            using (var newDecl = ColoredVertex.GetDecl(Device))
            {
                Device.VertexDeclaration = newDecl;
                Device.DrawUserPrimitives(PrimitiveType.TriangleList, 1, triangleVerts);
                Device.VertexDeclaration = oldDecl;
            }
        }

        /// <summary> Draw box. </summary>
        /// <remarks> Nesox, 2013-06-14. </remarks>
        /// <param name="center"> The min. </param>
        /// <param name="length"> The length. </param>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        /// <param name="color">  The color. </param>
        public static void DrawBox(Vector3 center, float length, float width, float height, Color color)
        {
            length /= 2f;
            width /= 2f;
            height /= 2f;
            var extents = new Vector3(width, length, height);

            Device.SetTransform(TransformState.World, Matrix.Translation(center.X, center.Y, center.Z));

            var vtx = new[]
            {
                new ColoredVertex(new Vector3(-extents.X, extents.Y, extents.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(extents.X, extents.Y, extents.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(extents.X, -extents.Y, extents.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(-extents.X, -extents.Y, extents.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(-extents.X, extents.Y, -extents.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(extents.X, extents.Y, -extents.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(extents.X, -extents.Y, -extents.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(-extents.X, -extents.Y, -extents.Z), color.ToArgb())
            };

            var ind = new short[]
            {
                // front
                0, 1, 2,    2, 3, 0,
                // right
                1, 5, 6,    6, 2, 1,
                // back
                5, 4, 7,    7, 6, 5,
                // left
                4, 0, 3,    3, 7, 4,
                // top
                4, 5, 1,    1, 0, 4,
                // bottom
                3, 2, 6,    6, 7, 3
            };

            var oldDecl = Device.VertexDeclaration;
            using (var newDecl = ColoredVertex.GetDecl(Device))
            {
                Device.VertexDeclaration = newDecl;
                Device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, 8, 12, ind, Format.Index16, vtx, 16);
                Device.VertexDeclaration = oldDecl;
            }
        }

        /// <summary> Draw outlined box.</summary>
        /// <remarks> Nesox, 2013-12-28.</remarks>
        /// <param name="center"> The min. </param>
        /// <param name="length"> The length. </param>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        /// <param name="color">  The color. </param>
        public static void DrawOutlinedBox(Vector3 center, float length, float width, float height, Color color)
        {
            float w = width / 2;
            float h = height / 2;
            float d = length / 2;

            Matrix mat = Matrix.Identity * Matrix.Scaling(w, d, h) * Matrix.Translation(center);
            Device.SetTransform(TransformState.World, mat);

            var min = new Vector3(-1, -1, 0);
            var max = new Vector3(1, 1, 1);

            var vtx = new[]
            {
                new ColoredVertex(new Vector3(min.X, max.Y, max.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(max.X, max.Y, max.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(min.X, min.Y, max.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(max.X, min.Y, max.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(min.X, min.Y, min.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(max.X, min.Y, min.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(min.X, max.Y, min.Z), color.ToArgb()),
                new ColoredVertex(new Vector3(max.X, max.Y, min.Z), color.ToArgb())
            };

            var ind = new short[]
            {
                //Top           [_]
                0, 1, 1, 3,
                3, 2, 2, 0,
                //Bottom        [_]
                6, 7, 7, 5,
                5, 4, 4, 6,
                // Back         | |
                0, 6, 1, 7,
                // Front        | |
                2, 4, 3, 5,
                // Left         | |
                0, 6, 2, 4,
                // Right        | |
                1, 7, 3, 5
            };

            var oldDecl = Device.VertexDeclaration;
            using (var newDecl = ColoredVertex.GetDecl(Device))
            {
                Device.VertexDeclaration = newDecl;
                Device.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, 8, 12, ind, Format.Index16, vtx, 16);
                Device.VertexDeclaration = oldDecl;
            }
        }

        /// <summary> Draws a circle.</summary>
        /// <remarks> Nesox, 2013-12-29.</remarks>
        /// <param name="center"> The min. </param>
        /// <param name="radius"> The radius. </param>
        /// <param name="slices"> The slices. </param>
        /// <param name="color">  The color. </param>
        public static void DrawCircle(Vector3 center, float radius, int slices, Color color)
        {
            Device.SetTransform(TransformState.World, Matrix.Translation(center.X, center.Y, center.Z));

            var min = new Vector3(0, 0, 0);
            var radsPerSlice = (float)(Math.PI * 2 / slices);

            var vertices = new ColoredVertex[slices + 1];
            vertices[0] = new ColoredVertex(min, color.ToArgb());
            float curRad = 0;
            for (int i = 1; i < slices + 1; i++)
            {
                var sine = (float)Math.Sin(curRad);
                var cosine = (float)Math.Cos(curRad);

                vertices[i] = new ColoredVertex(new Vector3(min.X + cosine * radius, min.Y + sine * radius, min.Z), color.ToArgb());
                curRad += radsPerSlice;
            }

            var indices = new int[slices * 3];
            for (int i = 0; i < slices; i++)
            {
                indices[i * 3 + 0] = i + 1;
                indices[i * 3 + 1] = i == slices - 1 ? 1 : i + 2;
                indices[i * 3 + 2] = 0;
            }

            Device.VertexFormat = ColoredVertex.Format;

            var oldDecl = Device.VertexDeclaration;
            using (var newDecl = ColoredVertex.GetDecl(Device))
            {
                Device.VertexDeclaration = newDecl;
                Device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, vertices.Length, indices.Length/3, indices, Format.Index32, vertices, ColoredVertex.Stride);
                Device.VertexDeclaration = oldDecl;
            }
        }

        public static void DrawSphere(Vector3 center, float radius, int stacks, int slices, Color color)
        {
            Device.SetTransform(TransformState.World, Matrix.Translation(center.X, center.Y, center.Z));
            var min = new Vector3(0, 0, 0);

            //calculates the resulting number of vertices and indices  
            int numVerts = (stacks + 1) * (slices + 1);
            int numInds = 3 * stacks * (slices + 1) * 2;
            var indices = new int[numInds];
            var vertices = new ColoredVertex[numVerts];

            var stackAngle = (float)(Math.PI / stacks);
            var sliceAngle = (float)((Math.PI * 2.0) / slices);

            int vertIndex = 0;
            //Generate the group of stacks for the sphere  
            int vertCount = 0;
            int indCount = 0;

            for (int stack = 0; stack < (stacks + 1); stack++)
            {
                var r = (float)Math.Sin(stack * stackAngle);
                var z = (float)Math.Cos(stack * stackAngle);

                //Generate the group of segments for the current Stack  
                for (int slice = 0; slice < (slices + 1); slice++)
                {
                    float x = r * (float)Math.Sin(slice * sliceAngle);
                    float y = r * (float)Math.Cos(slice * sliceAngle);
                    vertices[vertCount] = new ColoredVertex(min + new Vector3(x * radius, y * radius, z * radius), color.ToArgb());

                    vertCount++;
                    if (stack != (stacks - 1))
                    {
                        indices[indCount] = vertIndex + (slices + 1);
                        indCount++;
                        indices[indCount] = vertIndex + 1;
                        indCount++;
                        indices[indCount] = vertIndex;
                        indCount++;
                        indices[indCount] = vertIndex + (slices);
                        indCount++;
                        indices[indCount] = vertIndex + (slices + 1);
                        indCount++;
                        indices[indCount] = vertIndex;
                        indCount++;
                        vertIndex++;
                    }
                }
            }

            using (VertexBuffer vbSphere = new VertexBuffer(Device, vertices.Length*ColoredVertex.Stride, Usage.WriteOnly, ColoredVertex.Format, Pool.Default))
            using (IndexBuffer ibSphere = new IndexBuffer(Device, indices.Length*sizeof(int), Usage.WriteOnly, Pool.Default, false))
            {
                using (DataStream sphereIndexDs = ibSphere.Lock(0, indices.Length * sizeof(int), LockFlags.Discard | LockFlags.NoOverwrite))
                    sphereIndexDs.WriteRange(indices);
                using (DataStream sphereVertexDs = vbSphere.Lock(0, vertices.Length * ColoredVertex.Stride, LockFlags.Discard | LockFlags.NoOverwrite))
                    sphereVertexDs.WriteRange(vertices);

                vbSphere.Unlock();
                ibSphere.Unlock();

                var oldDecl = Device.VertexDeclaration;
                using (var newDecl = ColoredVertex.GetDecl(Device))
                {
                    Device.VertexDeclaration = newDecl;

                    Device.VertexFormat = ColoredVertex.Format;
                    Device.SetStreamSource(0, vbSphere, 0, ColoredVertex.Stride);
                    Device.Indices = ibSphere;
                    Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, vertices.Length/3);
                    //Device.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, vertices.Length, vertices.Length / 3, indices, Format.Index32, vertices, ColoredVertex.Stride);
                    
                    Device.VertexDeclaration = oldDecl;
                }
            }
        }

        /// <summary> Cleanups this object.</summary>
        /// <remarks> Nesox, 2013-12-27.</remarks>
        public static void Cleanup()
        {
            if (_line != null)
                _line.Dispose();
            if (_fontSprite != null)
                _fontSprite.Dispose();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ColoredVertex : IEquatable<ColoredVertex>
    {
        public static VertexDeclaration GetDecl(Device device)
        {
            return new VertexDeclaration(device, new[] {
                                new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0), 
                                new VertexElement(0, 12, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0), 
                                VertexElement.VertexDeclarationEnd
                });
        }

        /// <summary>
        /// Gets or sets the position of the vertex.
        /// </summary>
        public readonly Vector3 Position;

        /// <summary>
        /// Gets or sets the color of the vertex.
        /// </summary>
        public readonly int Color;

        public static int Stride { get { return Marshal.SizeOf(typeof(ColoredVertex)); }}
        public static VertexFormat Format { get { return VertexFormat.Position | VertexFormat.Diffuse; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColoredVertex"/> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        public ColoredVertex(Vector3 position, int color)
            : this()
        {
            Position = position;
            Color = color;
        }

        /// <summary>
        /// Implements operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ColoredVertex left, ColoredVertex right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements operator !=.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ColoredVertex left, ColoredVertex right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return Position.GetHashCode() + Color.GetHashCode();
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return Equals((ColoredVertex)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ColoredVertex other)
        {
            return (Position == other.Position && Color == other.Color);
        }
    }
}
