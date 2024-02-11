using Common;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Objects
{
    internal class Character : TextureObject
    {
        public Character(Rectangle rect, Vector4 color, RectangleF texCoord, Point offset) : base(rect, color, texCoord, offset)
        {
        }

        public override void OnLoad(Shader shader)
        {
            base.OnLoad(shader);
        }
    }
}
