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
        public Point Offset { get; set; }

        protected IVertex2[]? _vertices = null;
        public IVertex2[] Vertices => this._vertices ?? throw new ArgumentException();

        protected uint[]? _indices = null;
        public uint[] Indices => this._indices ?? throw new ArgumentException();
        public Point Center => new Point(this.Rectangle.X + this.Rectangle.Width / 2, this.Rectangle.Y + this.Rectangle.Height / 2);


        public RenderObject(Rectangle rectangle, Vector4 color, Point offset)
        {
            this.Rectangle = rectangle;
            this.Color = color;
            this.Offset = offset;
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
            var offsetX = this.Offset.X; 
            var offsetY = this.Offset.Y;
            // Change vertices data
            _vertices =
            [
                new ColorVertex2(new Vector2(this.Rectangle.X + offsetX, this.Rectangle.Y + offsetY), this.Color),
                new ColorVertex2(new Vector2(this.Rectangle.X + offsetX + this.Rectangle.Width, this.Rectangle.Y + offsetY), this.Color),
                new ColorVertex2(new Vector2(this.Rectangle.X + offsetX + this.Rectangle.Width, this.Rectangle.Y + offsetY + this.Rectangle.Height), this.Color),
                new ColorVertex2(new Vector2(this.Rectangle.X + offsetX, this.Rectangle.Y + offsetY + this.Rectangle.Height), this.Color),
            ];

            _indices =
            [
                0, 1, 3,
                1, 2, 3
            ];
        }

        public virtual void SetParameters(Shader shader)
        {
            shader.EnableAttribs(ColorVertex2.AttribLocations);
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

            // GL.DrawArrays(PrimitiveType.Triangles, 0, this.Indices.Length);
        }

        public virtual void SetFrameParameters(Shader shader)
        {
            //shader.UniformMatrix3("aTransform", this.Matrix);
            //shader.Uniform2("aCenter", new Vector2(this.Center.X, this.Center.Y));

            //shader.Uniform2("aTexOffset", new Vector2(0, 0));
            //shader.Uniform1("aMode", 0);
            shader.Uniform1("aMode", 0);
            shader.Uniform1("aPointSize", 10);
        }

        public void OnUnload()
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