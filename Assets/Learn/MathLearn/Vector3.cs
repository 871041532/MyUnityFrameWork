using System;
using System.Collections;
using System.Collections.Generic;

namespace MathLearn
{
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        private static readonly Vector3 m_zero = new Vector3(0, 0, 0);
        public static Vector3 zero => m_zero;

        private static readonly Vector3 m_up = new Vector3(0, 1f, 0);
        public static Vector3 up => m_up;

        private static readonly Vector3 m_down = new Vector3(0, -1f, 0);
        public static Vector3 down => m_down;

        private static readonly Vector3 m_left = new Vector3(-1f, 0, 0);
        public static Vector3 left => m_left;

        private static readonly Vector3 m_right = new Vector3(1f, 0, 0);
        public static Vector3 right => m_right;

        private static readonly Vector3 m_forward = new Vector3(0, 0, 1f);
        public static Vector3 forward => m_forward;

        private static readonly Vector3 m_back = new Vector3(0, 0, -1f);
        public static Vector3 back => m_back;

        public bool Equals(Vector3 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector3 && Equals((Vector3) obj);
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
        }
        
        public Vector3(float _x, float _y, float _z=0)
        {
            this.x = _x;
            this.y = _y;
            this.z = _z;
        }

        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            float num1 = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            float num3 = lhs.z - rhs.z;
            return (num1 * num1 + num2 * num2 + num3 * num3) < 9.99999943962493E-11;
        }

        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !(lhs == rhs);
        }

        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static Vector3 operator -(Vector3 lhs)
        {
            return new Vector3(-lhs.x, -lhs.y, -lhs.z);
        }

        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }

        // 点乘
        public static float operator *(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        public static Vector3 operator *(Vector3 a, float d)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator *(float d, Vector3 a)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        // 标准化自身
        public void Normalize()
        {
            float magnitude = this.Magnitude();
            if ((double) magnitude > 9.99999974737875E-06)
                this = this / magnitude;
            else
                this = Vector3.zero;
        }

        // 返回标准化后的值，不改变自身的值
        public Vector3 normalized
        {
            get
            {
                Vector3 vector3 = new Vector3(this.x, this.y);
                vector3.Normalize();
                return vector3;
            }
        }

        // 模长
        public static float Magnitude(Vector3 vector)
        {
            return (float) Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        public float Magnitude()
        {
            return Vector3.Magnitude(this);
        }

        // 叉乘
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(
                lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x
            );
        }

        // 距离
        public static float Distance(Vector3 lhs, Vector3 rhs)
        {
            float dx = lhs.x - rhs.x;
            float dy = lhs.y - rhs.y;
            float dz = lhs.z - rhs.z;
            return (float) Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        
        public override string ToString()
        {
            return string.Format("(x:{0:F2}, y:{1:F2}, z:{2:F2})", this.x,  this.y, this.z);
        }
    }
}