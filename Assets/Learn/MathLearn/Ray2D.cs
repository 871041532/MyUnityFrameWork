using System;

namespace MathLearn
{
    public struct Ray2D
    {
        private Vector2 m_Origin;
        public Vector2 Origin => m_Origin;
        
        private Vector2 m_Direction;
        public Vector2 Direction => m_Direction;


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

        // 转换到向量式 P * N = D
        public void ComputeVectorStyle(out Vector2 normal, out float distance)
        {
            normal = new Vector2(m_Direction.y, -m_Direction.x);
            distance = m_Origin * normal;
        }

        // 获取平面上的最近点 P0
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
    }
}