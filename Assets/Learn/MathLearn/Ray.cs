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

        // 3D中两条参数直线的相交性检测（直线不是射线，负方向也算）
        public bool Interact(Ray other, out Vector3 interactPos)
        {
            Vector3 p1 = m_Origin;
            Vector3 p2 = other.origin;
            Vector3 d1 = m_Direction;
            Vector3 d2 = other.direction;
            Vector3 cross = Vector3.Cross(d1, d2);
            float denominator = cross.x * cross.x + cross.y * cross.y + cross.z * cross.z;
            if (cross == Vector3.zero)
            {
                interactPos = Vector3.zero;
                return false;  // 平行或者重合
            }
            else
            {
                float t1 = Vector3.Cross(p2 - p1, d2) * cross / denominator;
                float t2 = Vector3.Cross(p2 - p1, d1) * cross / denominator;
                Vector3 nearestPos1 = this.GetPoint(t1);
                Vector3 nearestPos2 = other.GetPoint(t2);
                if (nearestPos1 == nearestPos2)
                {
                    interactPos = nearestPos1;
                    return true;
                }
                else
                {
                    interactPos = Vector3.zero;
                    return false;  // 不相交
                }
            }
        }
        
        public override string ToString()
        {
            return string.Format("(origin={0}, direction={1})", origin, direction);
        }
    }
}