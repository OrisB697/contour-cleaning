namespace Lab1_OZR1.Geometry
{
    /// <summary>
    /// Прямоугольник в двумерном пространстве.
    /// </summary>
    public struct Rectangle2D
    {
        public double Left { get; set; }

        public double Top { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Right
        {
            get
            {
                return Left + Width;
            }
        }

        public double Bottom
        {
            get
            {
                return Top + Height;
            }
        }

        public Rectangle2D(
            double left,
            double top,
            double width,
            double height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Проверка принадлежности точки прямоугольнику.
        /// </summary>
        public bool Contains(Point2D point)
        {
            return point.X >= Left &&
                   point.X <= Right &&
                   point.Y >= Top &&
                   point.Y <= Bottom;
        }

        public override string ToString()
        {
            return
                $"Left={Left:F2}, " +
                $"Top={Top:F2}, " +
                $"Right={Right:F2}, " +
                $"Bottom={Bottom:F2}, " +
                $"Width={Width:F2}, " +
                $"Height={Height:F2}";
        }
    }
}