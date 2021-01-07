using System;

namespace MathLearn
{
    public struct Sphere
    {
        private Vector3 m_Center;
        public Vector3 center
        {
            get { return m_Center; }
            set { m_Center = value; }
        }

        private float m_Radius;
        public float radius
        {
            get { return m_Radius; }
            set { radius = value; }
        }

        public Sphere(Vector3 _center, float _radius)
        {
            m_Center = _center;
            m_Radius = _radius;
        }

        /// <summary>
        /// 获取球面离目标点的最近点
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector3 GetNearestPos(Vector3 pos)
        {
            if (pos == m_Center)
            {
                throw new Exception("目标点与圆心重合，圆上所有点都是最近点！");
            }
            // n为pos指向center的向量的单位向量, d为pos到center的距离
            Vector3 n = m_Center - pos;
            float d = n.Magnitude();
            n.Normalize();
            // 最近点pos1 =  pos + 单位向量 * 两点距离 = pos + n * (d - radius)
            Vector3 pos1 = pos + n * (d - m_Radius);
            return pos1;
        }

        /// <summary>
        /// 某点是否在圆中
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool InclusionPos(Vector3 pos)
        {
            float m = (m_Center - pos).Magnitude();
            return m <= m_Radius;
        }

        /// <summary>
        /// 获取圆内均匀分布的随机一个点（近似）
        /// </summary>
        /// <returns></returns>
        public Vector3 GetInclusionRandomPos()
        {
            Random random = new Random();
            float x=   (float) random.NextDouble() * 2.0f - 1.0f;
            float y =  (float) random.NextDouble() * 2.0f - 1.0f;
            float z =  (float) random.NextDouble() * 2.0f - 1.0f;
            if ((x * x + y * y + z * z) > 1)
            {
                Vector3 n = new Vector3(x, y, z);
                n.Normalize();
                float k = (float) Math.Pow(random.NextDouble(), 1.0 / 3.0);
                x = n.x * k;
                y = n.y * k;
                z = n.z * k;
                // // 生成0到pi之间的随机角度φ
                // float φ = (float) (random.NextDouble() * Math.PI);
                // // 生成0到2pi之间的随机角度θ
                // float θ = (float) (random.NextDouble() * Math.PI * 2);
                // // 生成开三次方平方根的极坐标随机长度k, 根据平方根的性质抵消球心稠密边缘稀疏的情况，使分布均匀。
                // float k = (float) Math.Pow(random.NextDouble(), 1.0 / 3.0);
                // // 计算平均分布的随机球坐标
                //  x = k * (float) (Math.Sin(φ) * Math.Cos(θ));
                //  y = k * (float) (Math.Sin(φ) * Math.Sin(θ));
                //  z = k * (float) Math.Cos(φ);
            }
            Vector3 target = m_Center;
            target.x += x * m_Radius;
            target.y += y * m_Radius;
            target.z += z * m_Radius;
            return target;
        }
        
        // 射线和圆的相交性检测
        public bool IntersectRay(Ray ray)
        {
            float dis1 = 0;
            float dis2 = 0;
            bool result = IntersectRay(ray, out dis1, out dis2);
            return result;
        }
        
        // 射线和圆的相交性检测
        public bool IntersectRay(Ray ray, out float distance1, out float distance2)
        {
            Vector3 e = m_Center - ray.origin;
            float a = e * ray.direction;
            float f2 = m_Radius * m_Radius + a * a - e * e;
            if (f2 < 0)
            {
                distance1 = 0;
                distance2 = 0;
                return false;
            }
            else
            {
                float fOne = (float)Math.Sqrt(f2);
                float fTwo = - (float)Math.Sqrt(f2);
                distance1 = a - fOne;
                distance2 = a - fTwo;
                return true;
            }
        }
        
    }
}