using Common;
using OpenTK.Mathematics;
using System.Drawing;

namespace App.Objects
{
    internal class TextureObject : RenderObject
    {
        public Texture? Texture { get; set; }
        public RectangleF TexCoord { get; set; }

        public TextureObject(Rectangle rectangle, Vector4 color, RectangleF texCoord, Point offset): base(rectangle, color, offset)
        {
            this.TexCoord = texCoord;
        }
        
        public override void SetVertexes(Shader shader)
        {
            var offsetX = this.Offset.X; 
            var offsetY = this.Offset.Y;
            // Change vertices data
            _vertices =
            [
                new ColorTextureVertex2(new Vector2(this.Rectangle.X + offsetX, this.Rectangle.Y + offsetY), this.Color, new Vector2(this.TexCoord.Left, this.TexCoord.Top)),
                new ColorTextureVertex2(new Vector2(this.Rectangle.X + offsetX + this.Rectangle.Width, this.Rectangle.Y + offsetY), this.Color, new Vector2(this.TexCoord.Right, this.TexCoord.Top)),
                new ColorTextureVertex2(new Vector2(this.Rectangle.X + offsetX + this.Rectangle.Width, this.Rectangle.Y + offsetY + this.Rectangle.Height), this.Color, new Vector2(this.TexCoord.Right, this.TexCoord.Bottom)),
                new ColorTextureVertex2(new Vector2(this.Rectangle.X + offsetX, this.Rectangle.Y + offsetY + this.Rectangle.Height), this.Color, new Vector2(this.TexCoord.Left, this.TexCoord.Bottom)),
            ];

            _indices =
            [
                0, 1, 3,
                1, 2, 3
            ];
        }

        public override void SetParameters(Shader shader)
        {
            base.SetParameters(shader);
            shader.EnableAttribs(ColorTextureVertex2.AttribLocations);
        }

        public override void SetFrameParameters(Shader shader)
        {
            base.SetFrameParameters(shader);
            // Active texture
            shader.Uniform1("aMode", 1);
            shader.Uniform1("aTexture", this.Texture?.Id ?? 0);
        }
    }
}