using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftSharp
{
    /// <summary>
    /// 3 dimensional floating point vector
    /// </summary>
    public struct Vector3
    {
        public float X, Y, Z; // vector coordinates

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // -- vector properties --

        public float Length
        {
            get
            {
                return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
            }
        }

        public float LengthSq
        {
            get
            {
                return (X * X) + (Y * Y) + (Z * Z);
            }
        }

        public void Normalize()
        {
            // calculate the vector length here to avoid unnecessary function call
            float length = (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
            X = X / length;
            Y = Y / length;
            Z = Z / length;
        }

        public void Scale(float scale)
        {
            X *= scale;
            Y *= scale;
            Z *= scale;
        }

        // -- static class methods --

        public static Vector3 Add(Vector3 vec1, Vector3 vec2)
        {
            Vector3 result;
            result.X = vec1.X + vec2.X;
            result.Y = vec1.Y + vec2.Y;
            result.Z = vec1.Z + vec2.Z;
            return result;
        }

        public static Vector3 CrossProduct(Vector3 vec1, Vector3 vec2)
        {
            Vector3 result;
            result.X = (vec1.Y * vec2.Z) - (vec1.Z * vec2.Y);
            result.Y = (vec1.Z * vec2.X) - (vec1.X * vec2.Z);
            result.Z = (vec1.X * vec2.Y) - (vec1.Y * vec2.X);
            return result;
        }

        public static float DotProduct(Vector3 vec1, Vector3 vec2)
        {
            return (vec1.X * vec2.X) + (vec1.Y * vec2.Y) + (vec1.Z * vec2.Z);
        }

        public static Vector3 Lerp(Vector3 vec1, Vector3 vec2, float value)
        {
            Vector3 result = vec1 + ((vec2 - vec1) * value);
            return result;
        }

        public static Vector3 Maximize(Vector3 vec1, Vector3 vec2)
        {
            Vector3 result;
            result.X = vec1.X > vec2.X ? vec1.X : vec2.X;
            result.Y = vec1.Y > vec2.Y ? vec1.Y : vec2.Y;
            result.Z = vec1.Z > vec2.Z ? vec1.Z : vec2.Z;
            return result;
        }

        public static Vector3 Minimize(Vector3 vec1, Vector3 vec2)
        {
            Vector3 result;
            result.X = vec1.X < vec2.X ? vec1.X : vec2.X;
            result.Y = vec1.Y < vec2.Y ? vec1.Y : vec2.Y;
            result.Z = vec1.Z < vec2.Z ? vec1.Z : vec2.Z;
            return result;
        }

        public static Vector3 Normalize(Vector3 input)
        {
            Vector3 result;
            // calculate the vector length here to avoid unnecessary function call
            float length = (float)Math.Sqrt((input.X * input.X) + (input.Y * input.Y) +
                (input.Z * input.Z));
            result.X = input.X / length;
            result.Y = input.Y / length;
            result.Z = input.Z / length;
            return result;
        }

        public static Vector3 Scale(Vector3 input, float scale)
        {
            Vector3 result;
            result.X = input.X * scale;
            result.Y = input.Y * scale;
            result.Z = input.Z * scale;
            return result;
        }

        public static Vector3 Subtract(Vector3 vec1, Vector3 vec2)
        {
            Vector3 result;
            result.X = vec1.X - vec2.X;
            result.Y = vec1.Y - vec2.Y;
            result.Z = vec1.Z - vec2.Z;
            return result;
        }


        public static Vector3 Transform(Vector3 v, Matrix m)
        {
            Vector3 result;
            result.X = (v.X * m.M11) + (v.Y * m.M21) + (v.Z * m.M31) + (1 * m.M41);
            result.Y = (v.X * m.M12) + (v.Y * m.M22) + (v.Z * m.M32) + (1 * m.M42);
            result.Z = (v.X * m.M13) + (v.Y * m.M23) + (v.Z * m.M33) + (1 * m.M43);
            return result;
        }

        public static Vector3 operator *(Vector3 v, Matrix m)
        {
            Vector3 result;
            result.X = (v.X * m.M11) + (v.Y * m.M21) + (v.Z * m.M31) + (1 * m.M41);
            result.Y = (v.X * m.M12) + (v.Y * m.M22) + (v.Z * m.M32) + (1 * m.M42);
            result.Z = (v.X * m.M13) + (v.Y * m.M23) + (v.Z * m.M33) + (1 * m.M43);
            return result;
        }

        public static Vector3 operator *(Vector3 vec, float scalar)
        {
            Vector3 result;
            result.X = vec.X * scalar;
            result.Y = vec.Y * scalar;
            result.Z = vec.Z * scalar;
            return result;
        }

        public static Vector3 operator -(Vector3 vec1, Vector3 vec2)
        {
            Vector3 result;
            result.X = vec1.X - vec2.X;
            result.Y = vec1.Y - vec2.Y;
            result.Z = vec1.Z - vec2.Z;
            return result;
        }

        public static Vector3 operator +(Vector3 vec1, Vector3 vec2)
        {
            Vector3 result;
            result.X = vec1.X + vec2.X;
            result.Y = vec1.Y + vec2.Y;
            result.Z = vec1.Z + vec2.Z;
            return result;
        }
    }
}
