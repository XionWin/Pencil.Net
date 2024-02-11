using Common;
using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;
using System.Drawing;

namespace App.Objects
{
    internal class RenderObject : IRenderObject
    {
        public int VAO { get; set; }

        public int VBO { get; set; }

        public int EBO { get; set; }

        public Rectangle Rectangle { get; set; }
        public Vector4 Color { get; set; }
        public Matrix3 Matrix { get; set; } = Matrix3.Identity;

        protected Vertex2[]? _vertices = null;
        public Vertex2[] Vertices => this._vertices ?? throw new ArgumentException();

        protected uint[]? _indices = null;
        public uint[] Indices => this._indices ?? throw new ArgumentException();
        public Point Center => new Point(this.Rectangle.X + this.Rectangle.Width / 2, this.Rectangle.Y + this.Rectangle.Height / 2);

        public Texture Texture { get; init; }
        public RectangleF TexCoord { get; set; }

        private static RectangleF DEFAULT_TEXCOORD = new RectangleF(0, 0, 1, 1);

        public RenderObject(Rectangle rectangle, Texture texture, RectangleF? texCoord = null)
        {
            this.Rectangle = rectangle;
            this.Texture = texture;
            this.TexCoord = texCoord ?? DEFAULT_TEXCOORD;
        }

        public virtual void OnLoad(Shader shader)
        {
            this.VAO = GL.Oes.GenVertexArray();
            this.VBO = GL.GenBuffer();
            this.EBO = GL.GenBuffer();

            GL.Oes.BindVertexArray(this.VAO);

            SetVertexes(shader);

            // bind vbo and set data for vbo
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VBO);
            var vertices = this.Vertices.GetRaw();
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // bind ebo and set data for ebo
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

            SetParameters(shader);
        }
        
        public virtual void SetVertexes(Shader shader)
        {
            // Change vertices data
            _vertices =
            [
                new Vertex2(new Vector2(this.Rectangle.X, this.Rectangle.Y), new Vector2(this.TexCoord.Left, this.TexCoord.Top)),
                new Vertex2(new Vector2(this.Rectangle.X + this.Rectangle.Width, this.Rectangle.Y), new Vector2(this.TexCoord.Right, this.TexCoord.Top)),
                new Vertex2(new Vector2(this.Rectangle.X + this.Rectangle.Width, this.Rectangle.Y + this.Rectangle.Height), new Vector2(this.TexCoord.Right, this.TexCoord.Bottom)),
                new Vertex2(new Vector2(this.Rectangle.X, this.Rectangle.Y + this.Rectangle.Height), new Vector2(this.TexCoord.Left, this.TexCoord.Bottom)),
            ];

            _indices =
            [
                0, 1, 3,
                1, 2, 3
            ];
        }

        public virtual void SetParameters(Shader shader)
        {
            shader.EnableAttribs(Vertex2.AttribLocations);
        }

        public virtual void OnRenderFrame(Shader shader)
        {
            // Bind the VAO
            GL.Oes.BindVertexArray(this.VAO);

            this.SetFrameParameters(shader);

            // Enable Alpha
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, 0);

            shader.Uniform1("aMode", 1);
            GL.DrawElements(PrimitiveType.Points, this.Indices.Length, DrawElementsType.UnsignedInt, 0);
            shader.Uniform1("aMode", 0);
        }

        public virtual void SetFrameParameters(Shader shader)
        {
            //shader.UniformMatrix3("aTransform", this.Matrix);
            //shader.Uniform2("aCenter", new Vector2(this.Center.X, this.Center.Y));

            //shader.Uniform2("aTexOffset", new Vector2(0, 0));
            shader.Uniform1("aTexture", 0);
            shader.Uniform1("aPointSize", 10);
            GL.BindTexture(TextureTarget.Texture2D, this.Texture?.Id ?? 0);
        }

        public void Dispose()
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VBO);
            GL.Oes.BindVertexArray(this.VAO);

            // Delete all the resources.
            GL.DeleteBuffer(this.VBO);
            GL.Oes.DeleteVertexArray(this.VAO);
        }
    }
}