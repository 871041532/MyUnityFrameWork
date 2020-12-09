using UnityEngine.SocialPlatforms;

namespace MathLearn
{
    public struct Triangle
    {
        private Vector3 m_Pos1;
        public Vector3 pos1 => m_Pos1;
        private Vector3 m_Pos2;
        public Vector3 pos2 => m_Pos2;
        private Vector3 m_Pos3;
        public Vector3 pos3 => m_Pos3;
        
        // 用三个点构造三角形
        public Triangle(Vector3 pos1, Vector3 pos2, Vector3 pos3)
        {
            m_Pos1 = pos1;
            m_Pos2 = pos2;
            m_Pos3 = pos3;
        }

        // 获取三角形面积
        public float GetArea()
        {
            Vector3 n1 = m_Pos2 - m_Pos1;
            Vector3 n2 = m_Pos3 - m_Pos2;
            float area = Vector3.Cross(n1, n2).Magnitude() / 2;
            return area;
        }
        
        // 点是否在三角形内
        public bool InclusionPos(Vector3 pos)
        {
            Plane plane = new Plane(m_Pos1, m_Pos2, m_Pos3);
            if (plane.InclusionPos(pos))
            {
                Vector3 d1 = m_Pos1 - pos;
                Vector3 d2 = m_Pos2 - pos;
                Vector3 d3 = m_Pos3 - pos;
                float a1 = Vector3.Cross(d2, d3) * plane.normal;
                float a2 = Vector3.Cross(d3, d1) * plane.normal;
                float a3 = Vector3.Cross(d1, d2) * plane.normal;
                if ((a1 >=0 && a2 >=0 && a3 >= 0) || (a1 <= 0 && a2 <= 0 && a3 <= 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// 获取点在三角形平面上的重心空间坐标，不在平面返回false
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="gravityPos"></param>
        /// <returns></returns>
        public bool GetGravityPos(Vector3 pos, out Vector3 gravityPos)
        {
            Plane plane = new Plane(m_Pos1, m_Pos2, m_Pos3);
            if (plane.InclusionPos(pos))
            {
                Vector3 d1 = m_Pos1 - pos;
                Vector3 d2 = m_Pos2 - pos;
                Vector3 d3 = m_Pos3 - pos;
                float a1 = Vector3.Cross(d2, d3) * plane.normal;
                float a2 = Vector3.Cross(d3, d1) * plane.normal;
                float a3 = Vector3.Cross(d1, d2) * plane.normal;
                float A = Vector3.Cross(m_Pos3 - m_Pos2, m_Pos1 - m_Pos3) * plane.normal;
                gravityPos = Vector3.zero;
                gravityPos.x = a1 / A;
                gravityPos.y = a2 / A;
                gravityPos.z = a3 / A;
                return true;
            }
            else
            {
                gravityPos = Vector3.zero;
                return false;
            }
        }

        // 射线检测（三角形正反两面都检测，但是射线只检测正方向）
        public bool Raycast(Ray ray, out float dis)
        {
            Plane plane = new Plane(m_Pos1, m_Pos2, m_Pos3);
            var isTrigger = plane.Raycast(ray, out dis);
            if (isTrigger)
            {
                var hitPos = ray.GetPoint(dis);
                var isCast = this.InclusionPos(hitPos);
                return isCast;
            }
            return false;
        }
    }
}