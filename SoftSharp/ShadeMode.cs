using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftSharp
{
    /// <summary>
    /// Specifies flat shading (first vertex color goes across whole triangle) or smooth shading (linear interpolation
    /// of all three vertex colors)
    /// </summary>
    public enum ShadeMode
    {
        Flat,
        Smooth
    }
}
