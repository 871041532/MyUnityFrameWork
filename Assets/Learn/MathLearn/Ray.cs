namespace MathLearn
{
    public struct Ray
    {
        private Vector3 m_Origin;
        public Vector3 origin => m_Origin;
        
        private Vector3 m_Direction;
        public Vector3 direction => m_Direction;


        public Ray(Vector3 origin, Vector3 direction)
        {
            m_Origin = origin;
            m_Direction = direction.normalized;
        }
        
        public override int GetHashCode()
        {
            return m_Origin.GetHashCode() ^ m_Direction.GetHashCode() << 2;
        }
        
        // 获取自起点开始，一定距离后的点
        public Vector3 GetPoint(float distance)
        {
            return m_Origin + m_Direction * distance;
        }

        // 获取直线上距离点P的最近点 P0
        // q = f(t) = Porg + td = Porg + (d * (Q - Porg)) * d
        public Vector3 GetNearestPos(Vector3 targetPos)
        {
            float t = m_Direction * (targetPos - m_Origin);
            Vector3 nearestPos = m_Origin + t * m_Direction;
            return nearestPos;
        }
        
        public override string ToString()
        {
            return string.Format("(origin={0}, direction={1})", origin, direction);
        }
    }
}