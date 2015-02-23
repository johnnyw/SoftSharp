using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftSharp
{
    /// <summary>
    /// Depth buffer test functions
    /// </summary>
    public enum DepthFunction
    {
        Never, // always fail depth test
        Less, // pass if the new pixel value is less than the current one
        Equal, // pass if pixel values are equal
        LessEqual, // pass if less than or equal
        Greater, // pass if pixel value is greater
        NotEqual, // pass if pixel values are not equal
        GreaterEqual, // pass if new pixel is greater or equal
        Always // always pass depth test
    }
}
