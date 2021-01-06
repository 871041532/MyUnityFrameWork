namespace MathLearn
{
    public struct Bounds
    {
        private Vector3 m_Center;
        private Vector3 m_Extent;

        public Vector3 center
        {
            get { return m_Center; }
            set { m_Center = value; }
        }

        public Vector3 size
        {
            get { return m_Extent * 2f; }
            set { m_Extent = value * 0.5f; }
        }

        public Vector3 min
        {
            get { return m_Center - m_Extent; }
            set { SetMinMax(value, max);}
        }

        public Vector3 max
        {
            get { return m_Center + m_Extent; }
            set { SetMinMax(min, value);}
        }                                                                       

        public Bounds(Vector3 center, Vector3 size)
        {
            this.m_Center = center;
            this.m_Extent = size * 0.5f;
        }

        public void SetMinMax(Vector3 _min, Vector3 _max)
        {
            m_Extent = (_max - _min) * 0.5f;
            m_Center = (_max + _min) * 0.5f;
        }

        public override int GetHashCode()
        {
            return m_Center.GetHashCode() ^ m_Extent.GetHashCode() << 2;
        }
        
        // 扩张AABB使之包围住点
        public void Encapsulate(Vector3 pos)
        {
           SetMinMax(Vector3.Min(min, pos), Vector3.Max(max, pos)); 
        }
        
        // 两个AABB相交性判断
        public bool Intersects(Bounds bounds)
        {
            return min.x <= bounds.max.x && max.x >= bounds.min.x && min.y <=  bounds.max.y && max.y >= bounds.min.y && min.z <= bounds.max.z && max.z >= bounds.min.z;
        }
        
        // AABB和平面的相交性检测
        public bool IntersectPlane(Plane plane)
        {
            Vector3 minPos = Vector3.zero;
            Vector3 maxPos = Vector3.zero;
            Vector3 thisMin = min;
            Vector3 thisMax = max;
            
            if (plane.normal.x > 0)
            {   // 如果nx>0, 点积最小的顶点x=xmin，点积最大的顶点是x=xmax
                minPos.x = thisMin.x;
                maxPos.x = thisMax.x;
            }
            else
            {   // 如果nx < 0，点积最小的顶点x=xmax，点积最大的顶点是x=xmin
                minPos.x = thisMax.x;
                maxPos.x = thisMin.x;
            }

            if (plane.normal.y > 0)
            {   // 如果ny > 0，点积最小的顶点y=ymin，点积最大的顶点是y=ymax
                minPos.y = thisMin.y;
                maxPos.y = thisMax.y;
            }
            else
            {
                minPos.y = thisMax.y;
                maxPos.y = thisMin.y;
            }

            if (plane.normal.z > 0)
            {
                // 如果nz > 0，点积最小的顶点z=zmin，点积最大的顶点是z=zmax
                minPos.z = thisMin.z;
                maxPos.z = thisMax.z;
            }
            else
            {
                minPos.z = thisMax.z;
                maxPos.z = thisMin.z;
            }

            float distance1 = minPos * plane.normal;
            float distance2 = maxPos * plane.normal;
            return distance1 < plane.distance && plane.distance < distance2;
        }
        
        // 判断AABB是否和射线相交
        public bool IntersectRay(Ray ray)
        {
            float temp = 0;
            return IntersectRay(ray, out temp);
        }

        // AABB和射线的相交性检测
        public bool IntersectRay(Ray ray, out float distance)
        {
            // 点在内部
            if (ray.origin >= min && ray.origin <= max)
            {
                distance = 0;
                return true;
            } 
            // minX
            if (ray.origin.x < min.x && ray.direction.x > 0)
            {
                float t = (min.x - ray.origin.x) / ray.direction.x + (float)9.99999974737875E-07;
                Vector3 hitPos = ray.origin + ray.direction * t;
                if (hitPos.y >= min.y && hitPos.y <= max.y && hitPos.z >= min.z && hitPos.z <= max.z)
                {
                    distance = t;
                    return true;
                }
            }
            // maxX
            if (ray.origin.x > max.x && ray.direction.x < 0)
            {
                float t = (max.x - ray.origin.x) / ray.direction.x + (float)9.99999974737875E-07;
                Vector3 hitPos = ray.origin + ray.direction * t;
                if (hitPos.y >= min.y && hitPos.y <= max.y && hitPos.z >= min.z && hitPos.z <= max.z)
                {
                    distance = t;
                    return true;
                }
            }
            // minY
            if (ray.origin.y < min.y && ray.direction.y > 0)
            {
                float t = (min.y - ray.origin.y) / ray.direction.y + (float)9.99999974737875E-07;
                Vector3 hitPos = ray.origin + ray.direction * t;
                if (hitPos.x >= min.x && hitPos.x <= max.x && hitPos.z >= min.z && hitPos.z <= max.z)
                {
                    distance = t;
                    return true;
                }
            }
            // maxY
            if (ray.origin.y > max.y && ray.direction.y < 0)
            {
                float t = (max.y - ray.origin.y) / ray.direction.y + (float)9.99999974737875E-07;
                Vector3 hitPos = ray.origin + ray.direction * t;
                if (hitPos.x >= min.x && hitPos.x <= max.x && hitPos.z >= min.z && hitPos.z <= max.z)
                {
                    distance = t;
                    return true;
                }
            }
            // minZ
            if (ray.origin.z < min.z && ray.direction.z > 0)
            {
                float t = (min.z - ray.origin.z) / ray.direction.z + (float)9.99999974737875E-07;
                Vector3 hitPos = ray.origin + ray.direction * t ;
                if (hitPos.x > min.x && hitPos.x < max.x && hitPos.y > min.y && hitPos.y < max.y)
                {
                    distance = t;
                    return true;
                }
            }
            // maxZ
            if (ray.origin.z > max.z && ray.direction.z < 0)
            {
                float t = (max.z - ray.origin.z) / ray.direction.z + (float)9.99999974737875E-07;
                Vector3 hitPos = ray.origin + ray.direction * t;
                if (hitPos.x > min.x && hitPos.x < max.x && hitPos.y > min.y && hitPos.y < max.y)
                {
                    distance = t;
                    return true;
                }
            }
            distance = 0;
            return false;
        }
        
        /// <summary>
        /// 获取Bounds上离某点最近的点，如果在内部返回自身
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector3 GetNearestPos(Vector3 pos)
        {
            Vector3 _min = min;
            Vector3 _max = max;
            // 按一定顺序沿着每条轴将pos推向Bounds
            if (pos.x < _min.x)
            {
                pos.x = _min.x;
            }
            else if (pos.x > _max.x)
            {
                pos.x = _max.x;
            }

            if (pos.y < _min.y)
            {
                pos.y = _min.y;
            }
            else if (pos.y > _max.y)
            {
                pos.y = _max.y;
            }

            if (pos.z < _min.z)
            {
                pos.z = _min.z;
            }
            else if (pos.z > _max.z)
            {
                pos.z = _max.z;
            }
            return pos;
        }
        
        public override string ToString()
        {
            return string.Format("(min={0}, max={1})", min, max);
        }
    }
}





















