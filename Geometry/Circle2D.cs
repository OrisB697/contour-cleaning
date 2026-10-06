using System;

namespace Lab1_OZR1.Geometry
{
    /// <summary>
    /// Окружность в двумерном пространстве.
    /// Используется как габарит движущейся фигуры.
    /// </summary>
    public struct Circle2D
    {
        public double CenterX { get; set; }

        public double CenterY { get; set; }

        public double Radius { get; set; }

        public double Left
        {
            get
            {
                return CenterX - Radius;
            }
        }

        public double Right
        {
            get
            {
                return CenterX + Radius;
            }
        }

        public double Top
        {
            get
            {
                return CenterY - Radius;
            }
        }

        public double Bottom
        {
            get
            {
                return CenterY + Radius;
            }
        }

        public Circle2D(
            double centerX,
            double centerY,
            double radius)
        {
            if (radius < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius));
            }

            CenterX = centerX;
            CenterY = centerY;
            Radius = radius;
        }

        /// <summary>
        /// Проверка принадлежности точки окружности.
        /// </summary>
        public bool Contains(Point2D point)
        {
            double dx = point.X - CenterX;
            double dy = point.Y - CenterY;

            return dx * dx + dy * dy <= Radius * Radius;
        }

        public override string ToString()
        {
            return
                $"Center=({CenterX:F2}; {CenterY:F2}), " +
                $"Radius={Radius:F2}";
        }
    }
}