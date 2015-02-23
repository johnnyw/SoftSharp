using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftSharp
{
    /// <summary>
    /// 4 dimensional floating point vector
    /// </summary>
    public struct Vector4
    {
        public float X, Y, Z, W; // vector coordinates

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        // -- vector properties --

        public float Length
        {
            get
            {
                return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
            }
        }

        public float LengthSq
        {
            get
            {
                return (X * X) + (Y * Y) + (Z * Z) + (W * W);
            }
        }

        // -- instance methods --

        public void Normalize()
        {
            // calculate the vector length here to avoid unnecessary function call
            float length = (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
            X = X / length;
            Y = Y / length;
            Z = Z / length;
            W = W / length;
        }

        public void Scale(float scale)
        {
            X *= scale;
            Y *= scale;
            Z *= scale;
            W *= scale;
        }

        // -- static class methods --

        public static Vector4 Add(Vector4 vec1, Vector4 vec2)
        {
            Vector4 result;
            result.X = vec1.X + vec2.X;
            result.Y = vec1.Y + vec2.Y;
            result.Z = vec1.Z + vec2.Z;
            result.W = vec1.W + vec2.W;
            return result;
        }

        public static float DotProduct(Vector4 vec1, Vector4 vec2)
        {
            return (vec1.X * vec2.X) + (vec1.Y * vec2.Y) + (vec1.Z * vec2.Z) + (vec1.W * vec2.W);
        }

        public static Vector4 Lerp(Vector4 vec1, Vector4 vec2, float value)
        {
            Vector4 result = vec1 + ((vec2 - vec1) * value);
            return result;
        }

        public static Vector4 Maximize(Vector4 vec1, Vector4 vec2)
        {
            Vector4 result;
            result.X = vec1.X > vec2.X ? vec1.X : vec2.X;
            result.Y = vec1.Y > vec2.Y ? vec1.Y : vec2.Y;
            result.Z = vec1.Z > vec2.Z ? vec1.Z : vec2.Z;
            result.W = vec1.W > vec2.W ? vec1.W : vec2.W;
            return result;
        }

        public static Vector4 Minimize(Vector4 vec1, Vector4 vec2)
        {
            Vector4 result;
            result.X = vec1.X < vec2.X ? vec1.X : vec2.X;
            result.Y = vec1.Y < vec2.Y ? vec1.Y : vec2.Y;
            result.Z = vec1.Z < vec2.Z ? vec1.Z : vec2.Z;
            result.W = vec1.W < vec2.W ? vec1.W : vec2.W;
            return result;
        }

        public static Vector4 Normalize(Vector4 input)
        {
            Vector4 result;
            // calculate the vector length here to avoid unnecessary function call
            float length = (float)Math.Sqrt((input.X * input.X) + (input.Y * input.Y) +
                (input.Z * input.Z) + (input.W * input.W));
            result.X = input.X / length;
            result.Y = input.Y / length;
            result.Z = input.Z / length;
            result.W = input.W / length;
            return result;
        }

        public static Vector4 Scale(Vector4 input, float scale)
        {
            Vector4 result;
            result.X = input.X * scale;
            result.Y = input.Y * scale;
            result.Z = input.Z * scale;
            result.W = input.W * scale;
            return result;
        }

        public static Vector4 Subtract(Vector4 vec1, Vector4 vec2)
        {
            Vector4 result;
            result.X = vec1.X - vec2.X;
            result.Y = vec1.Y - vec2.Y;
            result.Z = vec1.Z - vec2.Z;
            result.W = vec1.W * vec2.W;
            return result;
        }


        public static Vector4 Transform(Vector4 v, Matrix m)
        {
            Vector4 result;
            result.X = (v.X * m.M11) + (v.Y * m.M21) + (v.Z * m.M31) + (v.W * m.M41);
            result.Y = (v.X * m.M12) + (v.Y * m.M22) + (v.Z * m.M32) + (v.W * m.M42);
            result.Z = (v.X * m.M13) + (v.Y * m.M23) + (v.Z * m.M33) + (v.W * m.M43);
            result.W = (v.X * m.M14) + (v.Y * m.M24) + (v.Z * m.M34) + (v.W * m.M44);
            return result;
        }

        public static Vector4 operator *(Vector4 v, Matrix m)
        {
            Vector4 result;
            result.X = (v.X * m.M11) + (v.Y * m.M21) + (v.Z * m.M31) + (v.W * m.M41);
            result.Y = (v.X * m.M12) + (v.Y * m.M22) + (v.Z * m.M32) + (v.W * m.M42);
            result.Z = (v.X * m.M13) + (v.Y * m.M23) + (v.Z * m.M33) + (v.W * m.M43);
            result.W = (v.X * m.M14) + (v.Y * m.M24) + (v.Z * m.M34) + (v.W * m.M44);
            return result;
        }

        public static Vector4 operator *(Vector4 vec, float scalar)
        {
            Vector4 result;
            result.X = vec.X * scalar;
            result.Y = vec.Y * scalar;
            result.Z = vec.Z * scalar;
            result.W = vec.W * scalar;
            return result;
        }

        public static Vector4 operator -(Vector4 vec1, Vector4 vec2)
        {
            Vector4 result;
            result.X = vec1.X - vec2.X;
            result.Y = vec1.Y - vec2.Y;
            result.Z = vec1.Z - vec2.Z;
            result.W = vec1.W - vec2.W;
            return result;
        }

        public static Vector4 operator +(Vector4 vec1, Vector4 vec2)
        {
            Vector4 result;
            result.X = vec1.X + vec2.X;
            result.Y = vec1.Y + vec2.Y;
            result.Z = vec1.Z + vec2.Z;
            result.W = vec1.W + vec2.W;
            return result;
        }
    }
}
