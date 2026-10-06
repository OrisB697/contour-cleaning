using System;

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

        public Point2D Rotate(Point2D center, double angleRadians)
        {
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);

            double dx = X - center.X;
            double dy = Y - center.Y;

            return new Point2D(
                center.X + dx * cos - dy * sin,
                center.Y + dx * sin + dy * cos
            );
        }

        /// <summary>
        /// Поворот точки вокруг заданного центра на угол в градусах.
        /// </summary>
        public Point2D RotateDegrees(Point2D center, double angleDegrees)
        {
            return Rotate(center, angleDegrees * Math.PI / 180.0);
        }
    }
}