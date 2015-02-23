using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SoftSharp
{
    /// <summary>
    /// Surface material properties
    /// </summary>
    public struct Material
    {
        /// <summary>
        /// Diffuse material color
        /// </summary>
        public Color Diffuse;

        /// <summary>
        /// Ambient color
        /// </summary>
        public Color Ambient;

        /// <summary>
        /// Specular color
        /// </summary>
        public Color Specular;

        /// <summary>
        /// Emissive color
        /// </summary>
        public Color Emissive;

        /// <summary>
        /// Specular exponent power
        /// </summary>
        public float Power;
    }
}
