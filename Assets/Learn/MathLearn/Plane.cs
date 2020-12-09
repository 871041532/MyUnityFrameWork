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

        // 法线+距离，构造平面，distance是原点到平面的有符号距离（Unity中距离是平面到原点有符号距离）
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
        
        // 不共线的N个点，构造平面（建议用顺时针顺序的点，可以求得出最佳平面）
        public Plane(Vector3[] points)
        {
            // normal
            m_Normal = Vector3.zero;
            // 从最后一个顶点开始，在循环中不做if判断
            Vector3 cur = points[points.Length - 1];
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 next = points[i];
                m_Normal.x += (cur.z + next.z) * (cur.y - next.y);
                m_Normal.y += (cur.x + next.x) * (cur.z - next.z);
                m_Normal.z += (cur.y + next.y) * (cur.x - next.x);
                cur = next;
            } 
            m_Normal.Normalize();

            m_Distance = 0;
            for (int i = 0; i < points.Length; i++)
            {
                m_Distance += m_Normal * points[i];
            }
            m_Distance = m_Distance / points.Length;
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
        
        // 点在平面上吗
        public bool InclusionPos(Vector3 pos)
        {
            var offset = pos * m_Normal - m_Distance;
            return offset < 9.99999974737875E-06 && offset > -9.99999974737875E-06;
        }


        // 检测射线（检测正反两面，并且射线起点不在平面上）
        public bool Raycast(Ray ray, out float dis)
        {
            float down = m_Normal * ray.direction;
            if ((double)down < -9.99999974737875E-06 || (double)down > 9.99999974737875E-06)  // 负数只检测平面正面, 此处正面背面都检测和unity保持一致
            {
                float upper = m_Distance - ray.origin * m_Normal;
                dis = upper / down;
                if (dis <= 0)
                {
                    return false;  // 此处如果返回true那么射线变直线
                }
                return true;
            }
            else
            {
                dis = 0;
                return false;
            }
        }
  
        public override string ToString()
        {
            return string.Format("(normal={0}, distance={1})", m_Normal, m_Distance);
        }
    }
}