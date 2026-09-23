using System;

namespace Lab1_OZR1.Geometry
{
    /// <summary>
    /// Направляющий вектор движения.
    /// </summary>
    public struct Vector2D
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Длина вектора.
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        /// <summary>
        /// Проверка нулевого вектора.
        /// </summary>
        public bool IsZero
        {
            get
            {
                return X == 0.0 && Y == 0.0;
            }
        }

        public static Vector2D operator +(Vector2D first, Vector2D second)
        {
            return new Vector2D(
                first.X + second.X,
                first.Y + second.Y
            );
        }

        public static Vector2D operator *(Vector2D vector, double value)
        {
            return new Vector2D(
                vector.X * value,
                vector.Y * value
            );
        }

        public override string ToString()
        {
            return $"({X:F2}; {Y:F2})";
        }
    }
}