﻿using Common;

namespace App.Objects
{
    internal interface IRenderObject
    {
        public int VAO { get; }

        public int VBO { get; }

        public int EBO { get; }
        public void OnLoad(Shader shader);
        public void OnRenderFrame(Shader shader);
        public void OnUnload();
    }
}