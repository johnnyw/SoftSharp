using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftSharp
{
    /// <summary>
    /// Specifies which polygons to cull from rendering
    /// </summary>
    public enum CullMode
    {
        None,
        Front,
        Back,
        FrontAndBack
    }
}
