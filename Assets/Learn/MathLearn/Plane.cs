using System;

namespace MathLearn
{
    public struct Plane
    {
        private Vector3 m_Normal;
        private float m_Distance;

        // 法线+平面上一点，构造平面
        public Plane(Vector3 normal, Vector3 pos)
        {
            m_Normal = normal.normalized;
            m_Distance = m_Normal * pos;
        }

        // 法线+距离，构造平面
        public Plane(Vector3 normal, float distance)
        {
            m_Normal = normal.normalized;
            m_Distance = distance;
        }

        // 不共线的三个点，构造平面
        public Plane(Vector3 a, Vector3 b, Vector3 c)
        {
            m_Normal = Vector3.Cross(a - b, c - b).normalized;
            m_Distance = m_Normal * c;
        }
        
        // 不共线的N个点，构造平面
        public Plane(Vector3[] points)
        {
            throw new NotImplementedException("方法未实现");
        }

        // 获取法线
        public Vector3 normal
        {
            get { return m_Normal; }
            set { m_Normal = value; }
        }

        // 获取距离
        public float distance
        {
            get { return m_Distance; }
            set { m_Distance = value; }
        }

        // 重定义平面（用法线和平面上一点）
        public void SetNormalAndPos(Vector3 _normal, Vector3 pos)
        {
            m_Normal = _normal.normalized;
            m_Distance = m_Normal * pos;
        }

        // 重定义平面（用不共线的三个点）
        public void Set3Pos(Vector3 a, Vector3 b, Vector3 c)
        {
            m_Normal = Vector3.Cross(a - b, c - b).normalized;
            m_Distance = m_Normal * c;
        }

        // 翻转平面自己
        public void Flip()
        {
            m_Normal = -m_Normal;
            m_Distance = -m_Distance;
        }

        // 获取一个翻转后的新平面
        public Plane flipped => new Plane(-m_Normal, -m_Distance);

        // 获取平面上离某点最近的点
        public Vector3 GetNearestPos(Vector3 pos)
        {
            float offset = pos * m_Normal - m_Distance;
            Vector3 nearest = pos - offset * m_Normal;
            return nearest;
        }


        // 检测射线
        public bool Raycast(Ray ray, out float dis)
        {
            throw new NotImplementedException("方法未实现");
        }

        
        public override string ToString()
        {
            return string.Format("(normal={0}, distance={1})", m_Normal, m_Distance);
        }
    }
}