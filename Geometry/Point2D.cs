namespace Lab1_OZR1.Geometry
{
    /// <summary>
    /// Точка двумерного пространства.
    /// </summary>
    public struct Point2D
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Перемещение точки на заданный вектор.
        /// </summary>
        public static Point2D operator +(Point2D point, Vector2D vector)
        {
            return new Point2D(
                point.X + vector.X,
                point.Y + vector.Y
            );
        }

        public override string ToString()
        {
            return $"({X:F2}; {Y:F2})";
        }
    }
}