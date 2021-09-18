namespace UncorRTDPS.Services
{
    class Point<T>
    {
        public T X { get; set; }
        public T Y { get; set; }

        public Point() { }

        public Point(T X, T Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public bool IsEquals(Point<T> p1, Point<T> p2)
        {
            return p1.X.Equals(p2.X) && p1.Y.Equals(p2.Y);
        }
    }
}
