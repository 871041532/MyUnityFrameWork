using System;

namespace MathLearn
{
    public class Quaternion
    {
        private float _w;
        private float _x;
        private float _y;
        private float _z;

        public float w => _w;

        public float x => _x;

        public float y => _y;

        public float z => _z;

        public Quaternion(float x, float y, float z, float w)
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this._w = w;
        }

        public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
        {
            float w1 = lhs.w;
            float w2 = rhs.w;
            float x1 = lhs.x;
            float x2 = rhs.x;
            float y1 = lhs.y;
            float y2 = rhs.y;
            float z1 = lhs.z;
            float z2 = rhs.z;

            float w = w1 * w2 - x1 * x2 - y1 * y2 - z1 * z2;
            float x = w1 * x2 + x1 * w2 + z1 * y2 - y1 * z2;
            float y = w1 * y2 + y1 * w2 + x1 * z2 - z1 * x2;
            float z = w1 * z2 + z1 * w2 + y1 * x2 - x1 * y2;
            return new Quaternion(x, y, z, w);
        }

        public static Vector3 operator *(Quaternion lhs, Vector3 p)
        {
            Quaternion pQua = new Quaternion(p.x, p.y, p.z, 0);
            Quaternion inverse = Quaternion.Inverse(lhs);
            Quaternion target = lhs * pQua * inverse;
            return  new Vector3(target.x, target.y, target.z);
        }
        
        public static Quaternion Inverse(Quaternion rotation)
        {
           return new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);;
        }
       
        
        public static Quaternion AngleAxis(float angle, Vector3 axis)
        {
            float halfAngle = angle * (float)Math.PI / 360f;
            float sin = (float)Math.Sin(halfAngle);
            float cos = (float)Math.Cos(halfAngle);
            return new Quaternion(axis.x * sin, axis.y * sin, axis.z * sin, cos);
        }

        public override string ToString()
        {
            return string.Format("(x:{0:F2}, y:{1:F2}, z:{2:F2}, w:{3:F2})", this.x,  this.y, this.z, this.w);
        }
    }
}