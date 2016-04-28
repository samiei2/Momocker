namespace MoMoker.src
{
    internal struct SpacePoint
    {
        private float x;
        private float y;
        private float z;

        public SpacePoint(float x, float y, float v)
        {
            this.x = x;
            this.y = y;
            this.z = v;
        }

        public float X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public float Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public float Z
        {
            get
            {
                return z;
            }

            set
            {
                z = value;
            }
        }
    }
}