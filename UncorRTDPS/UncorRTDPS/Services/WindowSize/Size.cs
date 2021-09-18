namespace UncorRTDPS.Services.WindowSize
{
    class Size<T>
    {
        public T Width { get; set; }
        public T Height { get; set; }

        public Size() { }

        public Size(T width, T height)
        {
            Width = width;
            Height = height;
        }

        public bool IsEquals(Size<T> s1, Size<T> s2)
        {
            return s1.Width.Equals(s2.Width) && s1.Height.Equals(s2.Height);
        }
    }
}
