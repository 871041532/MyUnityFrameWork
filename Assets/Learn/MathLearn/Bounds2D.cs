namespace MathLearn
{
    public struct Bounds2D
    {
        private Vector2 m_Center;
        private Vector2 m_Extent;

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

        public Vector2 min
        {
            get { return m_Center - m_Extent; }
            set { SetMinMax(value, max);}
        }

        public Vector2 max
        {
            get { return m_Center + m_Extent; }
            set { SetMinMax(min, value);}
        }                                                                       

        public Bounds2D(Vector2 center, Vector2 size)
        {
            this.m_Center = center;
            this.m_Extent = size * 0.5f;
        }

        public void SetMinMax(Vector2 _min, Vector2 _max)
        {
            m_Extent = (_max - _min) * 0.5f;
            m_Center = (_max + _min) * 0.5f;
        }

        public override int GetHashCode()
        {
            return m_Center.GetHashCode() ^ m_Extent.GetHashCode() << 2;
        }
        
        // 扩张AABB使之包围住点
        public void Encapsulate(Vector2 pos)
        {
           SetMinMax(Vector2.Min(min, pos), Vector2.Max(max, pos)); 
        }
        
        // 两个AABB相交性判断
        public bool Intersects(Bounds2D bounds)
        {
            return min.x <= bounds.max.x && max.x >= bounds.min.x && min.y <=  bounds.max.y && max.y >= bounds.min.y;
        }
        
        // 判断AABB是否和射线相交
        public bool IntersectRay(Ray2D ray)
        {
            float temp = 0;
            return IntersectRay(ray, out temp);
        }

        public bool IntersectRay(Ray2D ray, out float distance)
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
                float t = (min.x - ray.origin.x) / ray.direction.x;
                Vector2 hitPos = ray.origin + ray.direction * t;
                if (hitPos.y >= min.y && hitPos.y <= max.y)
                {
                    distance = t;
                    return true;
                }
            }
            // maxX
            if (ray.origin.x > max.x && ray.direction.x < 0)
            {
                float t = (max.x - ray.origin.x) / ray.direction.x;
                Vector2 hitPos = ray.origin + ray.direction * t;
                if (hitPos.y >= min.y && hitPos.y <= max.y)
                {
                    distance = t;
                    return true;
                }
            }
            // minY
            if (ray.origin.y < min.y && ray.direction.y > 0)
            {
                float t = (min.y - ray.origin.y) / ray.direction.y;
                Vector2 hitPos = ray.origin + ray.direction * t;
                if (hitPos.x >= min.x && hitPos.x <= max.x)
                {
                    distance = t;
                    return true;
                }
            }
            // maxY
            if (ray.origin.y > max.y && ray.direction.y < 0)
            {
                float t = (max.y - ray.origin.y) / ray.direction.y;
                Vector2 hitPos = ray.origin + ray.direction * t;
                if (hitPos.x >= min.x && hitPos.x <= max.x)
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
        public Vector2 GetNearestPos(Vector2 pos)
        {
            Vector2 _min = min;
            Vector2 _max = max;
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
            return pos;
        }
        
        public override string ToString()
        {
            return string.Format("(min={0}, max={1})", min, max);
        }
        
    }
}





















