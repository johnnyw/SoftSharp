using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftSharp
{
    /// <summary>
    /// Right-handed 4x4 transformation matrix
    /// </summary>
    public struct Matrix
    {
        // matrix instance data
        public float M11, M12, M13, M14,
                     M21, M22, M23, M24,
                     M31, M32, M33, M34,
                     M41, M42, M43, M44;

        // -- class transform methods --

        /// <summary>
        /// Generates a 4x4 identity matrix
        /// </summary>
        /// <returns>Generated identity matrix</returns>
        public static Matrix Identity()
        {
            Matrix result;
            result.M11 = 1.0f; result.M12 = 0.0f; result.M13 = 0.0f; result.M14 = 0.0f;
            result.M21 = 0.0f; result.M22 = 1.0f; result.M23 = 0.0f; result.M24 = 0.0f;
            result.M31 = 0.0f; result.M32 = 0.0f; result.M33 = 1.0f; result.M34 = 0.0f;
            result.M41 = 0.0f; result.M42 = 0.0f; result.M43 = 0.0f; result.M44 = 1.0f;
            return result;
        }

        public static Matrix Inverse(Matrix m)
        {
            Matrix result;
            // calculate determinant of input matrix
            float determinant = (m.M11 * m.M22 * m.M33 * m.M44) + (m.M11 * m.M23 * m.M34 * m.M42) + (m.M11 * m.M24 * m.M32 * m.M43) +
                                (m.M12 * m.M21 * m.M34 * m.M43) + (m.M12 * m.M23 * m.M31 * m.M44) + (m.M12 * m.M24 * m.M33 * m.M41) +
                                (m.M13 * m.M21 * m.M32 * m.M44) + (m.M13 * m.M22 * m.M34 * m.M41) + (m.M13 * m.M24 * m.M31 * m.M42) +
                                (m.M14 * m.M21 * m.M33 * m.M42) + (m.M14 * m.M22 * m.M31 * m.M43) + (m.M14 * m.M23 * m.M32 * m.M41) -
                                (m.M11 * m.M22 * m.M34 * m.M43) - (m.M11 * m.M23 * m.M32 * m.M44) - (m.M11 * m.M24 * m.M33 * m.M42) -
                                (m.M12 * m.M21 * m.M33 * m.M44) - (m.M12 * m.M23 * m.M34 * m.M41) - (m.M12 * m.M24 * m.M31 * m.M43) -
                                (m.M13 * m.M21 * m.M34 * m.M42) - (m.M13 * m.M22 * m.M31 * m.M44) - (m.M13 * m.M24 * m.M32 * m.M41) -
                                (m.M14 * m.M21 * m.M32 * m.M43) - (m.M14 * m.M22 * m.M33 * m.M41) - (m.M14 * m.M23 * m.M31 * m.M42);

            // calculate adjoint matrix
            result.M11 = (m.M22 * m.M33 * m.M44) + (m.M23 * m.M34 * m.M42) + (m.M24 * m.M32 * m.M43) - (m.M22 * m.M34 * m.M43) - (m.M23 * m.M32 * m.M44) - (m.M24 * m.M33 * m.M42);
            result.M12 = (m.M12 * m.M34 * m.M43) + (m.M13 * m.M32 * m.M44) + (m.M14 * m.M33 * m.M42) - (m.M12 * m.M33 * m.M44) - (m.M13 * m.M34 * m.M42) - (m.M14 * m.M32 * m.M43);
            result.M13 = (m.M12 * m.M23 * m.M44) + (m.M13 * m.M24 * m.M42) + (m.M14 * m.M22 * m.M43) - (m.M12 * m.M24 * m.M43) - (m.M13 * m.M22 * m.M44) - (m.M14 * m.M23 * m.M42);
            result.M14 = (m.M12 * m.M24 * m.M33) + (m.M13 * m.M22 * m.M34) + (m.M14 * m.M23 * m.M32) - (m.M12 * m.M23 * m.M34) - (m.M13 * m.M24 * m.M32) - (m.M14 * m.M22 * m.M33);
            result.M21 = (m.M21 * m.M34 * m.M43) + (m.M23 * m.M31 * m.M44) + (m.M24 * m.M33 * m.M41) - (m.M21 * m.M33 * m.M44) - (m.M23 * m.M34 * m.M41) - (m.M24 * m.M31 * m.M43);
            result.M22 = (m.M11 * m.M33 * m.M44) + (m.M13 * m.M34 * m.M41) + (m.M14 * m.M31 * m.M43) - (m.M11 * m.M34 * m.M43) - (m.M13 * m.M31 * m.M44) - (m.M14 * m.M33 * m.M41);
            result.M23 = (m.M11 * m.M24 * m.M43) + (m.M13 * m.M21 * m.M44) + (m.M14 * m.M23 * m.M41) - (m.M11 * m.M23 * m.M44) - (m.M13 * m.M24 * m.M41) - (m.M14 * m.M21 * m.M43);
            result.M24 = (m.M11 * m.M23 * m.M34) + (m.M13 * m.M24 * m.M31) + (m.M14 * m.M21 * m.M33) - (m.M11 * m.M24 * m.M33) - (m.M13 * m.M21 * m.M34) - (m.M14 * m.M23 * m.M31);
            result.M31 = (m.M21 * m.M32 * m.M44) + (m.M22 * m.M34 * m.M41) + (m.M24 * m.M31 * m.M42) - (m.M21 * m.M34 * m.M42) - (m.M22 * m.M31 * m.M44) - (m.M24 * m.M32 * m.M41);
            result.M32 = (m.M11 * m.M34 * m.M42) + (m.M12 * m.M31 * m.M44) + (m.M14 * m.M32 * m.M41) - (m.M11 * m.M32 * m.M44) - (m.M12 * m.M34 * m.M41) - (m.M14 * m.M31 * m.M42);
            result.M33 = (m.M11 * m.M22 * m.M44) + (m.M12 * m.M24 * m.M41) + (m.M14 * m.M21 * m.M42) - (m.M11 * m.M24 * m.M42) - (m.M12 * m.M21 * m.M44) - (m.M14 * m.M22 * m.M41);
            result.M34 = (m.M11 * m.M24 * m.M32) + (m.M12 * m.M21 * m.M34) + (m.M14 * m.M22 * m.M31) - (m.M11 * m.M22 * m.M34) - (m.M12 * m.M24 * m.M31) - (m.M14 * m.M21 * m.M32);
            result.M41 = (m.M21 * m.M33 * m.M42) + (m.M22 * m.M31 * m.M43) + (m.M23 * m.M32 * m.M41) - (m.M21 * m.M32 * m.M43) - (m.M22 * m.M33 * m.M41) - (m.M23 * m.M31 * m.M42);
            result.M42 = (m.M11 * m.M32 * m.M43) + (m.M12 * m.M33 * m.M41) + (m.M13 * m.M31 * m.M42) - (m.M11 * m.M33 * m.M42) - (m.M12 * m.M31 * m.M43) - (m.M13 * m.M32 * m.M41);
            result.M43 = (m.M11 * m.M32 * m.M42) + (m.M12 * m.M21 * m.M43) + (m.M13 * m.M22 * m.M41) - (m.M11 * m.M22 * m.M43) - (m.M12 * m.M23 * m.M41) - (m.M13 * m.M21 * m.M42);
            result.M44 = (m.M11 * m.M22 * m.M33) + (m.M12 * m.M23 * m.M31) + (m.M13 * m.M21 * m.M32) - (m.M11 * m.M23 * m.M32) - (m.M12 * m.M21 * m.M33) - (m.M13 * m.M22 * m.M31);

            // scale adjoint by (1 / determinant) (resulting in inverse)
            result = result * (1.0f / determinant);

            // return inverse
            return result;
        }

        public static Matrix Transpose(Matrix m)
        {
            Matrix result;
            result.M11 = m.M11; result.M12 = m.M21; result.M13 = m.M31; result.M14 = m.M41;
            result.M21 = m.M12; result.M22 = m.M22; result.M23 = m.M32; result.M24 = m.M42;
            result.M31 = m.M13; result.M32 = m.M23; result.M33 = m.M33; result.M34 = m.M43;
            result.M41 = m.M14; result.M42 = m.M24; result.M43 = m.M34; result.M44 = m.M44;
            return result;
        }

        /// <summary>
        /// Generate a translation matrix
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <returns>Generated translation matrix</returns>
        public static Matrix Translate(float x, float y, float z)
        {
            Matrix result;
            result.M11 = 1.0f; result.M12 = 0.0f; result.M13 = 0.0f; result.M14 = 0.0f;
            result.M21 = 0.0f; result.M22 = 1.0f; result.M23 = 0.0f; result.M24 = 0.0f;
            result.M31 = 0.0f; result.M32 = 0.0f; result.M33 = 1.0f; result.M34 = 0.0f;
            result.M41 = x; result.M42 = y; result.M43 = z; result.M44 = 1.0f;
            return result;
        }

        /// <summary>
        /// Generates a rotation matrix about the X axis
        /// </summary>
        /// <param name="angle">X angle</param>
        /// <returns>Generated rotation matrix</returns>
        public static Matrix RotateX(float angle)
        {
            Matrix result;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            result.M11 = 1.0f; result.M12 = 0.0f; result.M13 = 0.0f; result.M14 = 0.0f;
            result.M21 = 0.0f; result.M22 = cos; result.M23 = sin; result.M24 = 0.0f;
            result.M31 = 0.0f; result.M32 = -sin; result.M33 = cos; result.M34 = 0.0f;
            result.M41 = 0.0f; result.M42 = 0.0f; result.M43 = 0.0f; result.M44 = 1.0f;
            return result;
        }

        /// <summary>
        /// Generates a rotation matrix about the Y axis
        /// </summary>
        /// <param name="angle">Y angle</param>
        /// <returns>Generated rotation matrix</returns>
        public static Matrix RotateY(float angle)
        {
            Matrix result;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            result.M11 = cos; result.M12 = 0.0f; result.M13 = -sin; result.M14 = 0.0f;
            result.M21 = 0.0f; result.M22 = 1.0f; result.M23 = 0.0f; result.M24 = 0.0f;
            result.M31 = sin; result.M32 = 0.0f; result.M33 = cos; result.M34 = 0.0f;
            result.M41 = 0.0f; result.M42 = 0.0f; result.M43 = 0.0f; result.M44 = 1.0f;
            return result;
        }

        /// <summary>
        /// Generates a rotation matrix about the Z axis
        /// </summary>
        /// <param name="angle">Z angle</param>
        /// <returns>Generated rotation matrix</returns>
        public static Matrix RotateZ(float angle)
        {
            Matrix result;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            result.M11 = cos; result.M12 = sin; result.M13 = 0.0f; result.M14 = 0.0f;
            result.M21 = -sin; result.M22 = cos; result.M23 = 0.0f; result.M24 = 0.0f;
            result.M31 = 0.0f; result.M32 = 0.0f; result.M33 = 1.0f; result.M34 = 0.0f;
            result.M41 = 0.0f; result.M42 = 0.0f; result.M43 = 0.0f; result.M44 = 1.0f;
            return result;
        }

        public static Matrix Scale(float x, float y, float z)
        {
            Matrix result;
            result.M11 = x; result.M12 = 0.0f; result.M13 = 0.0f; result.M14 = 0.0f;
            result.M21 = 0.0f; result.M22 = y; result.M23 = 0.0f; result.M24 = 0.0f;
            result.M31 = 0.0f; result.M32 = 0.0f; result.M33 = z; result.M34 = 0.0f;
            result.M41 = 0.0f; result.M42 = 0.0f; result.M43 = 0.0f; result.M44 = 1.0f;
            return result;
        }

        /// <summary>
        /// Used to create a projection matrix
        /// </summary>
        /// <param name="fov">Field of view</param>
        /// <param name="aspect">Aspect ratio</param>
        /// <param name="near">Near clip value</param>
        /// <param name="far">Far clip value</param>
        /// <returns>A valid projection matrix</returns>
        public static Matrix PerspectiveFOV(float fov, float aspect, float near, float far)
        {
            Matrix result;
            float yScale = (1.0f / (float)Math.Tan(fov / 2.0f));
            float xScale = yScale / aspect;

            result.M11 = xScale; result.M12 = 0.0f; result.M13 = 0.0f; result.M14 = 0.0f;
            result.M21 = 0.0f; result.M22 = yScale; result.M23 = 0.0f; result.M24 = 0.0f;
            result.M31 = 0.0f; result.M32 = 0.0f; result.M33 = far / (near - far); result.M34 = -1.0f;
            result.M41 = 0.0f; result.M42 = 0.0f; result.M43 = near * far / (near - far); result.M44 = 0.0f;
            return result;
        }

        /// <summary>
        /// Generates a camera matrix specifying viewer position and target
        /// </summary>
        /// <param name="eye">Camera position</param>
        /// <param name="at">Camera target</param>
        /// <param name="up">Up vector</param>
        /// <returns>A view matrix with a viewer position and target</returns>
        public static Matrix LookAt(Vector3 eye, Vector3 at, Vector3 up)
        {
            Matrix result;
            Vector3 zaxis = eye - at; zaxis.Normalize();
            Vector3 xaxis = Vector3.CrossProduct(up, zaxis); xaxis.Normalize();
            Vector3 yaxis = Vector3.CrossProduct(zaxis, xaxis);

            result.M11 = xaxis.X; result.M12 = yaxis.X; result.M13 = zaxis.X; result.M14 = 0.0f;
            result.M21 = xaxis.Y; result.M22 = yaxis.Y; result.M23 = zaxis.Y; result.M24 = 0.0f;
            result.M31 = xaxis.Z; result.M32 = yaxis.Z; result.M33 = zaxis.Z; result.M34 = 0.0f;
            result.M41 = -Vector3.DotProduct(xaxis, eye); result.M42 = -Vector3.DotProduct(yaxis, eye); result.M43 = -Vector3.DotProduct(zaxis, eye); result.M44 = 1.0f;
            return result;
        }

        public static Matrix Multiply(Matrix left, Matrix right)
        {
            Matrix result;
            result.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
            result.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
            result.M13 = left.M11 * right.M13 + left.M12 * right.M32 + left.M13 * right.M33 + left.M14 * right.M43;
            result.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

            result.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
            result.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
            result.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
            result.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

            result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
            result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
            result.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
            result.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

            result.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
            result.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
            result.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
            result.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;
            return result;
        }

        /// <summary>
        /// Multiplies two matrices and returns the result
        /// </summary>
        /// <param name="left">1st operand in multiplication</param>
        /// <param name="right">2nd operand in multiplication</param>
        /// <returns>Product of two matrices</returns>
        public static Matrix operator *(Matrix left, Matrix right)
        {
            Matrix result;
            result.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
            result.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
            result.M13 = left.M11 * right.M13 + left.M12 * right.M32 + left.M13 * right.M33 + left.M14 * right.M43;
            result.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

            result.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
            result.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
            result.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
            result.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

            result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
            result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
            result.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
            result.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

            result.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
            result.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
            result.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
            result.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;
            return result;
        }

        public static Matrix operator *(Matrix m, float scalar)
        {
            Matrix result;
            result.M11 = m.M11 * scalar;
            result.M12 = m.M12 * scalar;
            result.M13 = m.M13 * scalar;
            result.M14 = m.M14 * scalar;
            result.M21 = m.M12 * scalar;
            result.M22 = m.M22 * scalar;
            result.M23 = m.M23 * scalar;
            result.M24 = m.M24 * scalar;
            result.M31 = m.M31 * scalar;
            result.M32 = m.M32 * scalar;
            result.M33 = m.M33 * scalar;
            result.M34 = m.M34 * scalar;
            result.M41 = m.M41 * scalar;
            result.M42 = m.M42 * scalar;
            result.M43 = m.M43 * scalar;
            result.M44 = m.M44 * scalar;
            return result;
        }
    }
}
