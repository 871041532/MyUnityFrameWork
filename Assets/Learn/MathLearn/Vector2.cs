using System;
using System.Collections;
using System.Collections.Generic;

namespace MathLearn
{
    public struct Vector2
    {
        public float x;
        public float y;

        private static readonly Vector2 m_zero = new Vector2(0, 0);
        public static Vector2 zero => m_zero;

        private static readonly Vector2 m_up = new Vector2(0, 1f);
        public static Vector2 up => m_up;

        private static readonly Vector2 m_down = new Vector2(0, -1f);
        public static Vector2 down => m_down;

        private static readonly Vector2 m_left = new Vector2(-1f, 0);
        public static Vector2 left => m_left;

        private static readonly Vector2 m_right = new Vector2(1f, 0);
        public static Vector2 right => m_right;

        public bool Equals(Vector2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector2 && Equals((Vector2) obj);
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
        }
        
        public Vector2(float _x, float _y)
        {
            this.x = _x;
            this.y = _y;
        }

        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            float num1 = lhs.x - rhs.x;
            float num2 = lhs.y - rhs.y;
            return (num1 * num1 + num2 * num2) < 9.99999943962493E-11;
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return !(lhs == rhs);
        }

        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static Vector2 operator -(Vector2 lhs)
        {
            return new Vector2(-lhs.x, -lhs.y);
        }

        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        // 点乘
        public static float operator *(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        public static Vector2 operator *(Vector2 a, float d)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator *(float d, Vector2 a)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator /(Vector2 a, float d)
        {
            return new Vector2(a.x / d, a.y / d);
        }

        // 标准化自身
        public void Normalize()
        {
            float magnitude = this.Magnitude();
            if ((double) magnitude > 9.99999974737875E-06)
                this = this / magnitude;
            else
                this = Vector2.zero;
        }

        // 返回标准化后的值，不改变自身的值
        public Vector2 normalized
        {
            get
            {
                Vector2 vector2 = new Vector2(this.x, this.y);
                vector2.Normalize();
                return vector2;
            }
        }

        // 模长
        public static float Magnitude(Vector2 vector)
        {
            return (float) Math.Sqrt(vector.x * vector.x + vector.y * vector.y);
        }

        public float Magnitude()
        {
            return Vector2.Magnitude(this);
        }

        // 叉乘
        public static float Cross(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.y - lhs.y * rhs.x;
        }

        // 距离
        public static float Distance(Vector2 lhs, Vector2 rhs)
        {
            float dx = lhs.x - rhs.x;
            float dy = lhs.y - rhs.y;
            return (float) Math.Sqrt(dx * dx + dy * dy);
        }
        
        public override string ToString()
        {
            return string.Format("(x:{0:F2}, y:{1:F2})", this.x,  this.y);
        }
    }
}