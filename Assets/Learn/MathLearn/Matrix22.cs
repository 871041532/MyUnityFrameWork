using System;
using UnityEngine.SocialPlatforms;

namespace MathLearn
{
    public struct Matrix22
    {
        private float m00;
        private float m01;
        private float m10;
        private float m11;

        public Matrix22(float m00, float m01, float m10, float m11)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m10 = m10;
            this.m11 = m11;
        }

        public Matrix22(Vector3 line1, Vector3 line2)
        {
            this.m00 = line1.x;
            this.m01 = line1.y;
            this.m10 = line2.x;
            this.m11 = line2.y;
        }
        
        // 下标运算符
        public float this[int r, int c]
        {
            get
            {
                if (r == 0 && c == 0)
                    return m00;
                else if (r == 0 && c == 1)
                    return m01;
                else if (r == 1 && c == 0)
                    return m10;
                else if (r == 1 && c == 1)
                    return m11;
                return 0;
            }
            set {                
                if (r == 0 && c == 0)
                    m00 = value;
                else if (r == 0 && c == 1)
                    m01 = value;
                else if (r == 1 && c == 0)
                    m10 = value;
                else if (r == 1 && c == 1)
                    m11 = value;
            }
        }
        
        public float Determinant()
        {
            return m00 * m11 - m01 * m10;
        }

        public Matrix22 Inverse()
        {
            float det = this.Determinant();
            float t11 = m11 / det;
            float t12 = -m01 / det;
            float t21 = -m10 / det;
            float t22 = m00 / det;
            return new Matrix22(t11, t12, t21, t22);
        }

        public static Matrix22 operator *(Matrix22 lhs, Matrix22 rhs)
        {
            float t11 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10;
            float t12 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11;
            float t21 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10;
            float t22 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11;
            return new Matrix22(t11, t12, t21, t22);
        }

        public static Vector3 operator *(Vector3 p, Matrix22 m)
        {
            float x = p.x * m.m00 + p.y * m.m10;
            float y = p.x * m.m01 + p.y * m.m11;
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