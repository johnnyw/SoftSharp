using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftSharp
{
    /// <summary>
    /// 2 dimensional floating point vector
    /// </summary>
    public struct Vector2
    {
        public float X, Y; // vector coordinates

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        // -- vector properties --

        public float Length
        {
            get
            {
                return (float)Math.Sqrt((X * X) + (Y * Y));
            }
        }

        public float LengthSq
        {
            get
            {
                return (X * X) + (Y * Y);
            }
        }

        public void Normalize()
        {
            // calculate the vector length here to avoid unnecessary function call
            float length = (float)Math.Sqrt((X * X) + (Y * Y));
            X = X / length;
            Y = Y / length;
        }

        public void Scale(float scale)
        {
            X *= scale;
            Y *= scale;
        }

        // -- static class methods --

        public static Vector2 Add(Vector2 vec1, Vector2 vec2)
        {
            Vector2 result;
            result.X = vec1.X + vec2.X;
            result.Y = vec1.Y + vec2.Y;
            return result;
        }

        public static float DotProduct(Vector2 vec1, Vector2 vec2)
        {
            return (vec1.X * vec2.X) + (vec1.Y * vec2.Y);
        }

        public static Vector2 Lerp(Vector2 vec1, Vector2 vec2, float value)
        {
            Vector2 result = vec1 + ((vec2 - vec1) * value);
            return result;
        }

        public static Vector2 Maximize(Vector2 vec1, Vector2 vec2)
        {
            Vector2 result;
            result.X = vec1.X > vec2.X ? vec1.X : vec2.X;
            result.Y = vec1.Y > vec2.Y ? vec1.Y : vec2.Y;
            return result;
        }

        public static Vector2 Minimize(Vector2 vec1, Vector2 vec2)
        {
            Vector2 result;
            result.X = vec1.X < vec2.X ? vec1.X : vec2.X;
            result.Y = vec1.Y < vec2.Y ? vec1.Y : vec2.Y;
            return result;
        }

        public static Vector2 Normalize(Vector2 input)
        {
            Vector2 result;
            // calculate the vector length here to avoid unnecessary function call
            float length = (float)Math.Sqrt((input.X * input.X) + (input.Y * input.Y));
            result.X = input.X / length;
            result.Y = input.Y / length;
            return result;
        }

        public static Vector2 Scale(Vector2 input, float scale)
        {
            Vector2 result;
            result.X = input.X * scale;
            result.Y = input.Y * scale;
            return result;
        }

        public static Vector2 Subtract(Vector2 vec1, Vector2 vec2)
        {
            Vector2 result;
            result.X = vec1.X - vec2.X;
            result.Y = vec1.Y - vec2.Y;
            return result;
        }


        public static Vector2 Transform(Vector2 v, Matrix m)
        {
            Vector2 result;
            result.X = (v.X * m.M11) + (v.Y * m.M21) + (1 * m.M31);
            result.Y = (v.X * m.M12) + (v.Y * m.M22) + (1 * m.M32);
            return result;
        }

        public static Vector2 operator *(Vector2 v, Matrix m)
        {
            Vector2 result;
            result.X = (v.X * m.M11) + (v.Y * m.M21) + (1 * m.M31);
            result.Y = (v.X * m.M12) + (v.Y * m.M22) + (1 * m.M32);
            return result;
        }

        public static Vector2 operator *(Vector2 vec, float scalar)
        {
            Vector2 result;
            result.X = vec.X * scalar;
            result.Y = vec.Y * scalar;
            return result;
        }

        public static Vector2 operator -(Vector2 vec1, Vector2 vec2)
        {
            Vector2 result;
            result.X = vec1.X - vec2.X;
            result.Y = vec1.Y - vec2.Y;
            return result;
        }

        public static Vector2 operator +(Vector2 vec1, Vector2 vec2)
        {
            Vector2 result;
            result.X = vec1.X + vec2.X;
            result.Y = vec1.Y + vec2.Y;
            return result;
        }
    }
}
