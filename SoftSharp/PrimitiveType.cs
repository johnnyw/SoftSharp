using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftSharp
{
    /// <summary>
    /// Different ways of specifying geometric topology for rendering.
    /// </summary>
    public enum PrimitiveType
    {
        TriangleList, // every three vertices is a triangle
        TriangleStrip, // list of connected triangles
        TriangleFan // triangles all joined at one vertex
    }
}
