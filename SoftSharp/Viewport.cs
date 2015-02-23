using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftSharp
{
    /// <summary>
    /// Specifies screen space volume for rendering and clipping
    /// </summary>
    public struct Viewport
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public float Near, Far;
    }
}
