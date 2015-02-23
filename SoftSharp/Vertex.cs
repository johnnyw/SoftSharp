using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SoftSharp
{
    /// <summary>
    /// Vertex containing position, normal, diffuse, specular, and texture coordinate elements
    /// </summary>
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Color Diffuse;
        public Color Specular;
        public Vector2 TexCoord;
    }
}
