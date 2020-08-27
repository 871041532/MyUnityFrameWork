using System;
using UnityEngine.SocialPlatforms;

namespace MathLearn
{
    public struct Matrix22
    {
        private float m11;
        private float m12;
        private float m21;
        private float m22;

        public Matrix22(float m11, float m12, float m21, float m22)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m21 = m21;
            this.m22 = m22;
        }

        public Matrix22(Vector3 line1, Vector3 line2)
        {
            this.m11 = line1.x;
            this.m12 = line1.y;
            this.m21 = line2.x;
            this.m22 = line2.y;
        }

        public float Determinant()
        {
            return m11 * m22 - m12 * m21;
        }

        public Matrix22 Inverse()
        {
            float det = this.Determinant();
            float t11 = m22 / det;
            float t12 = -m12 / det;
            float t21 = -m21 / det;
            float t22 = m11 / det;
            return new Matrix22(t11, t12, t21, t22);
        }

        public static Matrix22 operator *(Matrix22 lhs, Matrix22 rhs)
        {
            float t11 = lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21;
            float t12 = lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22;
            float t21 = lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21;
            float t22 = lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22;
            return new Matrix22(t11, t12, t21, t22);
        }

        public static Vector3 operator *(Vector3 p, Matrix22 m)
        {
            float x = p.x * m.m11 + p.y * m.m21;
            float y = p.x * m.m12 + p.y * m.m22;
            return new Vector3(x, y, p.z);
        }

        public static Matrix22 Rotation(float angle)
        {
            float rad = angle * (float)Math.PI / 180;
            float cosValue = (float)Math.Cos(rad);
            float sinValue = (float)Math.Sin(rad);
            return new Matrix22(cosValue, sinValue, -sinValue, cosValue);
        }

        public static Matrix22 Scale(Vector3 s)
        {
            return new Matrix22(s.x, 0, 0, s.y);
        }
    }
}