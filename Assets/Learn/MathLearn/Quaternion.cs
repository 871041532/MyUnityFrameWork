using System;

namespace MathLearn
{
    public struct Quaternion
    {
        private float _w;
        private float _x;
        private float _y;
        private float _z;
        private static Quaternion _identity = new Quaternion(0, 0, 0, 1);

        public float w => _w;

        public float x => _x;

        public float y => _y;

        public float z => _z;

        public static Quaternion identity => Quaternion._identity;
        
        public Quaternion(float x, float y, float z, float w)
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this._w = w;
        }
    
        // 插值
        public static Quaternion Lerp(Quaternion lhs, Quaternion rhs, float t)
        {
            t = t < 0 ? 0 : t;
            t = t > 1 ? 1 : t;
            
            float w0 = lhs.w;
            float x0 = lhs.x;
            float y0 = lhs.y;
            float z0 = lhs.z;

            float w1 = rhs.w;
            float x1 = rhs.x;
            float y1 = rhs.y;
            float z1 = rhs.z;

            float w, x, y, z;
            // 点乘计算两四元数夹角的cos值
            float cosValue = w0 * w1 + x0 * x1 + y0 * y1 + z0 * z1;
            // 点乘为负，反转一个四元数取得较短弧面
            if (cosValue < 0)
            {
                w1 = -w1;
                x1 = -x1;
                y1 = -y1;
                z1 = -z1;
                cosValue = -cosValue;
            }

            float k0, k1;
            // 检查是否过于接近避免除以0
            if (cosValue > 0.9999f)
            {
                // 非常接近时直接线性插值即可
                k0 = 1.0f - t;
                k1 = t;
            }
            else
            {
                // 用三角公式求出sin值
                float sinValue = (float)Math.Sqrt(1.0f - cosValue * cosValue);
                // 通过sin和cos计算角度
                float rad = (float)Math.Atan2(sinValue, cosValue);
                float reverseCrossValue = 1.0f / sinValue;
                k0 = (float)Math.Sin((1.0f - t) * rad) * reverseCrossValue;
                k1 = (float)Math.Sin(t * rad) * reverseCrossValue;
            }

            // 插值
            w = w0 * k0 + w1 * k1;
            x = x0 * k0 + x1 * k1;
            y = y0 * k0 + y1 * k1;
            z = z0 * k0 + z1 * k1;
            return new Quaternion(x, y, z, w);
        }
        
        // 相等性判断
        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return Math.Abs(lhs.x - rhs.x) < 9.99999943962493E-11 && Math.Abs(lhs.y - rhs.y) < 9.99999943962493E-11 && Math.Abs(lhs.z - rhs.z) < 9.99999943962493E-11 && Math.Abs(lhs.w - rhs.w) < 9.99999943962493E-11;
        }

        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            return !(lhs == rhs);
        }
        
        // 四元数叉乘四元数
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

        // 四元数变换点
        public static Vector3 operator *(Quaternion lhs, Vector3 p)
        {
            Quaternion pQua = new Quaternion(p.x, p.y, p.z, 0);
            Quaternion inverse = Quaternion.Inverse(lhs);
            Quaternion target = lhs * pQua * inverse;
            return  new Vector3(target.x, target.y, target.z);
        }
        
        // 单位四元数求逆
        public static Quaternion Inverse(Quaternion rotation)
        {
           return new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);;
        }
       
        // 用旋转轴和角度构造四元数
        public static Quaternion AngleAxis(float angle, Vector3 axis)
        {
            float halfAngle = angle * (float)Math.PI / 360f;
            float sin = (float)Math.Sin(halfAngle);
            float cos = (float)Math.Cos(halfAngle);
            return new Quaternion(-axis.x * sin, -axis.y * sin, -axis.z * sin, cos);
        }

        public override string ToString()
        {
            return string.Format("(x:{0:F2}, y:{1:F2}, z:{2:F2}, w:{3:F2})", this.x,  this.y, this.z, this.w);
        }
    }
}