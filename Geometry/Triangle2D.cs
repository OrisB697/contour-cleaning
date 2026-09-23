using System;

namespace Lab1_OZR1.Geometry
{
    /// <summary>
    /// Равносторонний треугольник, используемый как габарит
    /// движущейся фигуры (ДЗ.А1).
    /// Вершина V1 направлена вверх.
    /// </summary>
    public struct Triangle2D
    {
        public Point2D V1 { get; set; }  // верхняя вершина
        public Point2D V2 { get; set; }  // левая нижняя
        public Point2D V3 { get; set; }  // правая нижняя

        public Triangle2D(Point2D v1, Point2D v2, Point2D v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }

        /// <summary>
        /// Все три вершины левее заданной координаты X.
        /// </summary>
        public bool AllLeftOf(double x)
        {
            return V1.X < x && V2.X < x && V3.X < x;
        }

        /// <summary>
        /// Все три вершины правее заданной координаты X.
        /// </summary>
        public bool AllRightOf(double x)
        {
            return V1.X > x && V2.X > x && V3.X > x;
        }

        /// <summary>
        /// Все три вершины выше заданной координаты Y.
        /// </summary>
        public bool AllAbove(double y)
        {
            return V1.Y < y && V2.Y < y && V3.Y < y;
        }

        /// <summary>
        /// Все три вершины ниже заданной координаты Y.
        /// </summary>
        public bool AllBelow(double y)
        {
            return V1.Y > y && V2.Y > y && V3.Y > y;
        }

        /// <summary>
        /// Полностью ли треугольник за пределами прямоугольной области.
        /// </summary>
        public bool IsOutside(Rectangle2D area)
        {
            return AllLeftOf(area.Left)
                || AllRightOf(area.Right)
                || AllAbove(area.Top)
                || AllBelow(area.Bottom);
        }

        /// <summary>
        /// Полностью ли треугольник внутри прямоугольной области.
        /// </summary>
        public bool IsInside(Rectangle2D area)
        {
            return !AllLeftOf(area.Left)
                && !AllRightOf(area.Right)
                && !AllAbove(area.Top)
                && !AllBelow(area.Bottom);
        }

        public override string ToString()
        {
            return $"V1={V1}, V2={V2}, V3={V3}";
        }
    }
}