using System;

namespace MathLearn
{
    public struct Circle2D
    {
        private Vector2 m_Center;
        public Vector2 center
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

        public Circle2D(Vector2 _center, float _radius)
        {
            m_Center = _center;
            m_Radius = _radius;
        }

        /// <summary>
        /// 获取圆弧离目标点的最近点
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2 GetNearestPos(Vector2 pos)
        {
            if (pos == m_Center)
            {
                throw new Exception("目标点与圆心重合，圆上所有点都是最近点！");
            }
            // n为pos指向center的向量的单位向量, d为pos到center的距离
            Vector2 n = m_Center - pos;
            float d = n.Magnitude();
            n.Normalize();
            // 最近点pos1 =  pos + 单位向量 * 两点距离 = pos + n * (d - radius)
            Vector2 pos1 = pos + n * (d - m_Radius);
            return pos1;
        }

        /// <summary>
        /// 某点是否在圆中
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool InclusionPos(Vector2 pos)
        {
            float m = (m_Center - pos).Magnitude();
            return m <= m_Radius;
        }

        /// <summary>
        /// 获取圆内均匀分布的随机一个点（极坐标法）
        /// </summary>
        /// <returns></returns>
        public Vector2 GetInclusionRandomPos()
        {
            Random random = new Random();
            // 生成0到2pi范围内的随机角度
            float rad = (float) (random.NextDouble() * Math.PI * 2);
            // 生成开平方根的极坐标随机长度k, 根据平方根的性质抵消圆心稠密边缘稀疏的情况，使分布均匀。
            float k = (float) Math.Sqrt(random.NextDouble());
            // 计算平均分布的随机极坐标
            float x = k * (float) Math.Cos(rad);
            float y = k * (float) Math.Sin(rad);
            // 计算极坐标
            Vector2 pos = m_Center;
            pos.x += m_Radius * x;
            pos.y += m_Radius * y;
            return pos;
        }
    }
}