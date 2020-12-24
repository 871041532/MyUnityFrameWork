using System;
using System.Security.Cryptography.X509Certificates;

namespace MathLearn
{
    public struct Ray2D
    {
        private Vector2 m_Origin;
        public Vector2 origin => m_Origin;
        
        private Vector2 m_Direction;
        public Vector2 direction => m_Direction;

        // 默认用参数式
        public Ray2D(Vector2 origin, Vector2 direction)
        {
            m_Origin = origin;
            m_Direction = direction.normalized;
        }
        
        // 获取自起点开始，一定距离后的点
        public Vector2 GetPoint(float distance)
        {
            return m_Origin + m_Direction * distance;
        }

        // 转换到向量式&隐式 P * N = D
        public void ComputeVectorStyle(out Vector2 normal, out float distance)
        {
            normal = new Vector2(m_Direction.y, -m_Direction.x);
            distance = m_Origin * normal;
        }

        // 获取直线上距离点P的最近点 P0
        // P0 + offset * normal = P;  offset = P * normal - d
        // P0 = P - offset * normal = P - (P * normal - d) * normal
        public Vector2 GetNearestPos(Vector2 targetPos)
        {
            Vector2 normal;
            float distance;
            this.ComputeVectorStyle(out normal, out distance);
            Vector2 nearestPos = targetPos - (targetPos * normal - distance) * normal;
            return nearestPos;
        }
        
        // 2D中两条隐式直线的相交性检测（直线不是射线，负方向也算）
        // x = (b2d1 - b1d2) / (a1b2 - a2b1)
        // y = (a1d2 - a2d1) / (a1b2 - a2b1)
        public bool Interact(Ray2D other, out Vector2 interactPos)
        {
            Vector2 mN;
            float mD;
            Vector2 tN;
            float tD;
            this.ComputeVectorStyle(out mN, out mD);
            other.ComputeVectorStyle(out tN, out tD);
            float denominator = mN.x * tN.y - mN.y * tN.x;
            if (denominator > 9.99999974737875E-06 || denominator < -9.99999974737875E-06)
            {
                float x = (tN.y * mD - mN.y * tD) / denominator;
                float y = (mN.x * tD - tN.x * mD) / denominator;
                interactPos = new Vector2(x, y);
                return true;
            }
            else
            {
                interactPos = Vector2.zero;
                return false;
            }
        }
        
        
        public override int GetHashCode()
        {
            return m_Origin.GetHashCode() ^ m_Direction.GetHashCode() << 2;
        }
        
        public override string ToString()
        {
            return string.Format("(origin={0}, direction={1})", origin, direction);
        }
    }
}