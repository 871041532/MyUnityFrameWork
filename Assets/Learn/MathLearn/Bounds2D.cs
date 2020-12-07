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

        public void SetMinMax(Vector2 min, Vector2 max)
        {
            m_Extent = (max - min) * 0.5f;
            m_Center = (max + min) * 0.5f;
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
    }
}





















