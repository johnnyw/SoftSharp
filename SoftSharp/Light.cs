using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SoftSharp
{
    /// <summary>
    /// Light type enumeration
    /// </summary>
    public enum LightType
    {
        Point, // point light
        Spot, // spot light
        Directional // directional light
    }

    /// <summary>
    /// Holds an individual lights properties
    /// </summary>
    public struct Light
    {
        /// <summary>
        /// Flag if light is enabled or not (used by renderer to determine how many lights are active)
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// Light type (point, spot, directional)
        /// </summary>
        public LightType Type;

        /// <summary>
        /// Diffuse light color
        /// </summary>
        public Color Diffuse;

        /// <summary>
        /// Specular light color
        /// </summary>
        public Color Specular;

        /// <summary>
        /// Ambient light color
        /// </summary>
        public Color Ambient;

        /// <summary>
        /// Light position in 3D space
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Light direction vector
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Light range in world space
        /// </summary>
        public float Range;

        /// <summary>
        /// Spotlight falloff value
        /// </summary>
        public float Falloff;

        /// <summary>
        /// Specifies how light intensity changes over distance. Not used for directional lights.
        /// </summary>
        public float Attenuation0;

        /// <summary>
        /// Specifies how light intensity changes over distance. Not used for directional lights.
        /// </summary>
        public float Attenuation1;

        /// <summary>
        /// Specifies how light intensity changes over distance. Not used for directional lights.
        /// </summary>
        public float Attenuation2;

        /// <summary>
        /// Angle in radians of spotlight's inner cone
        /// </summary>
        public float Theta;

        /// <summary>
        /// Angle in radians of the outer edge of the spotlight's cone
        /// </summary>
        public float Phi;
    }
}
