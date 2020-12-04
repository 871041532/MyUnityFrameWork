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
        
        public bool IntersectRay(Ray2D ray, out float distance)
        {
            // 点在包围盒里
            if (ray.origin > min && ray.origin < max)
            {
                distance = 0;
                return true;
            } 
            // minx
            
            return Bounds.IntersectRayAABB(ray, this, out distance);
        }
    }
}