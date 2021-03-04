using System;

namespace MathLearn
{
    public struct OBB2D
    {
        private Vector2 m_Center;
        private Vector2 m_Extent;
       
        private Vector2 m_right;  // x坐标轴
        private Vector2 m_forward;  // y坐标轴

        public Vector2 center
        {
            get { return m_Center; }
            set { m_Center = value; }
        }

        public Vector2 size
        {
            get { return m_Extent * 2f; }
            set { m_Extent = value * 0.5f; }
        }

        // 使用坐标轴构建OBB
        public OBB2D(Vector2 center, Vector2 size, Vector2 _right, Vector2 _forward)
        {
            m_Center = center;
            m_Extent = size * 0.5f;
            m_right = _right;
            m_forward = _forward;
        }

        // 使用物体->惯性旋转矩阵构建OBB
        public OBB2D(Vector2 center, Vector2 size, Matrix22 object2Intertial)
        {
            m_Center = center;
            m_Extent = size * 0.5f;
            m_right = new Vector2(object2Intertial[0, 0], object2Intertial[0, 1]);
            m_forward = new Vector2(object2Intertial[1, 0], object2Intertial[1, 1]);
        }

        public override int GetHashCode()
        {
            return m_Center.GetHashCode() ^ m_Extent.GetHashCode() << 2 ^ m_right.GetHashCode() >> 2 ^ m_forward.GetHashCode() >> 1;
        }

        // 两个OBB2D相交性判断
        public bool Intersects(OBB2D bounds)
        {
            throw new NotImplementedException();
        }

        // 判断OBB2D是否和射线相交
        public bool IntersectRay(Ray2D ray)
        {
            throw new NotImplementedException();
        }

        // 判断并求出OBB2D和射线的交点
        public bool IntersectRay(Ray2D ray, out float distance)
        {
            throw new NotImplementedException();
        }

        // 获取OBB2D上距离一个点最近的点，如果点在OBB2D中直接返回该点
        public Vector2 GetNearestPos(Vector2 pos)
        {
            throw new NotImplementedException();
        }

        // 将一个点&向量转换到局部空间
        // 将局部空间一个点&向量转换到世界空间
        // 获取局部空间下的AABB

        public override string ToString()
        {
            return string.Format("(center={0}, extent={1}, right={2}, forward={3})", m_Center, m_Extent, m_right, m_forward);
        }
    }
}