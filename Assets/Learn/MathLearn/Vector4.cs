using System;
using System.Collections;
using System.Collections.Generic;

namespace MathLearn
{
    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        private static readonly Vector4 m_one = new Vector4(1, 1, 1, 1);
        public static Vector4 one => m_one;

        private static readonly Vector4 m_zero = new Vector4(0, 0, 0, 0);
        public static Vector4 zero => m_zero;

        private static readonly Vector4 m_up = new Vector4(0, 1f, 0, 0);
        public static Vector4 up => m_up;

        private static readonly Vector4 m_down = new Vector4(0, -1f, 0, 0);
        public static Vector4 down => m_down;

        private static readonly Vector4 m_left = new Vector4(-1f, 0, 0, 0);
        public static Vector4 left => m_left;

        private static readonly Vector4 m_right = new Vector4(1f, 0, 0, 0);
        public static Vector4 right => m_right;

        private static readonly Vector4 m_forward = new Vector4(0, 0, 1f, 0);
        public static Vector4 forward => m_forward;

        private static readonly Vector4 m_back = new Vector4(0, 0, -1f, 0);
        public static Vector4 back => m_back;

        public bool Equals(Vector4 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector4 && Equals((Vector4)obj);
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
        }

        public Vector4(float _x, float _y, float _z, float _w)
        {
            this.x = _x;
            this.y = _y;
            this.z = _z;
            this.w = _w;
        }

        public static bool operator ==(Vector4 lhs, Vector4 rhs)
        {
            float num1 = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            float num3 = lhs.z - rhs.z;
            float num4 = lhs.w - rhs.w;
            return (num1 * num1 + num2 * num2 + num3 * num3 + num4 * num4) < 9.99999943962493E-11;
        }

        public static bool operator !=(Vector4 lhs, Vector4 rhs)
        {
            return !(lhs == rhs);
        }

        // 两个分量都小于
        public static bool operator <(Vector4 lhs, Vector4 rhs)
        {
            return lhs != rhs && lhs.x < rhs.x && lhs.y < rhs.y && lhs.z < rhs.z && lhs.w < rhs.w;
        }

        public static bool operator <=(Vector4 lhs, Vector4 rhs)
        {
            return (lhs == rhs) || (lhs.x < rhs.x && lhs.y < rhs.y && lhs.z < rhs.z && lhs.w < rhs.w);
        }

        // 两个分量都大于
        public static bool operator >(Vector4 lhs, Vector4 rhs)
        {
            return lhs != rhs && lhs.x > rhs.x && lhs.y > rhs.y && lhs.z > rhs.z && lhs.w > rhs.w;
        }

        public static bool operator >=(Vector4 lhs, Vector4 rhs)
        {
            return (lhs == rhs) || (lhs.x > rhs.x && lhs.y > rhs.y && lhs.z > rhs.z && lhs.w > rhs.w);
        }

        public static Vector4 operator -(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
        }

        public static Vector4 operator -(Vector4 lhs)
        {
            return new Vector4(-lhs.x, -lhs.y, -lhs.z, -lhs.w);
        }

        public static Vector4 operator +(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
        }

        // 点乘
        public static float operator *(Vector4 lhs, Vector4 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z + lhs.w * rhs.w;
        }

        public static Vector4 operator *(Vector4 a, float d)
        {
            return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vector4 operator *(float d, Vector4 a)
        {
            return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vector4 operator /(Vector4 a, float d)
        {
            return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
        }

        public static Vector4 Min(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z), Math.Min(lhs.w, rhs.w));
        }

        public static Vector4 Max(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z), Math.Max(lhs.w, rhs.w));
        }


        // 标准化自身
        public void Normalize()
        {
            float magnitude = this.Magnitude();
            if ((double)magnitude > 9.99999974737875E-06)
                this = this / magnitude;
            else
                this = Vector4.zero;
        }

        // 返回标准化后的值，不改变自身的值
        public Vector4 normalized
        {
            get
            {
                Vector4 Vector4 = new Vector4(this.x, this.y, this.z, this.w);
                Vector4.Normalize();
                return Vector4;
            }
        }

        // 模长
        public static float Magnitude(Vector4 vector)
        {
            return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z + vector.w * vector.w);
        }

        public float Magnitude()
        {
            return Vector4.Magnitude(this);
        }


        // 距离
        public static float Distance(Vector4 lhs, Vector4 rhs)
        {
            float dx = lhs.x - rhs.x;
            float dy = lhs.y - rhs.y;
            float dz = lhs.z - rhs.z;
            float dw = lhs.w - rhs.w;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
        }

        public override string ToString()
        {
            return string.Format("(x:{0:F2}, y:{1:F2}, z:{2:F2}, w:{3:F2})", this.x, this.y, this.z, this.w);
        }
    }
}