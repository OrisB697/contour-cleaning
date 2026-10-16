using System;
using System.ComponentModel;
using System.Drawing;
using Lab1_OZR1.Geometry;

namespace Lab1_OZR1.Movement
{
    /// <summary>
    /// Базовый класс движущейся фигуры.
    /// Поддерживает контурную очистку: хранит предыдущую позицию,
    /// в которой фигура была отрисована.
    /// </summary>
    public abstract class MovingShape
    {
        private static int _nextId = 1;

        [Browsable(false)]
        public int Id { get; } = _nextId++;

        [Category("Положение")]
        public double CenterX { get; set; }

        [Category("Положение")]
        public double CenterY { get; set; }

        [Browsable(false)]
        public double PrevCenterX { get; set; }

        [Browsable(false)]
        public double PrevCenterY { get; set; }

        [Category("Движение")]
        public Vector2D Velocity { get; set; }

        [Category("Внешний вид")]
        public Color Color { get; set; } = Color.LightSkyBlue;

        [Category("Отрисовка")]
        public int Order { get; set; }

        protected MovingShape(double centerX, double centerY, Vector2D velocity)
        {
            CenterX = centerX;
            CenterY = centerY;
            PrevCenterX = centerX;
            PrevCenterY = centerY;
            Velocity = velocity;
        }

        [Browsable(false)]
        public abstract Rectangle2D BoundingRectangle { get; }

        public virtual void Move()
        {
            PrevCenterX = CenterX;
            PrevCenterY = CenterY;

            CenterX += Velocity.X;
            CenterY += Velocity.Y;
        }

        public void Move(int steps)
        {
            if (steps < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(steps));
            }

            for (int i = 0; i < steps; i++)
            {
                Move();
            }
        }

        public bool IsInside(Rectangle2D area)
        {
            Rectangle2D bounds = BoundingRectangle;

            return bounds.Left >= area.Left &&
                   bounds.Right <= area.Right &&
                   bounds.Top >= area.Top &&
                   bounds.Bottom <= area.Bottom;
        }

        public bool IsOutside(Rectangle2D area)
        {
            Rectangle2D bounds = BoundingRectangle;

            return bounds.Right < area.Left ||
                   bounds.Left > area.Right ||
                   bounds.Bottom < area.Top ||
                   bounds.Top > area.Bottom;
        }

        /// <summary>
        /// Переносит фигуру на противоположную сторону области,
        /// если её габаритный прямоугольник полностью вышел
        /// за соответствующую границу.
        ///
        /// Отступы от центра до краёв габарита вычисляются
        /// индивидуально — это корректно работает для любых
        /// несимметричных многоугольников.
        /// </summary>
        public virtual bool CheckAndWrap(Rectangle2D area)
        {
            bool wrapped = false;

            Rectangle2D bounds = BoundingRectangle;

            double leftOffset = bounds.Left - CenterX;
            double rightOffset = bounds.Right - CenterX;
            double topOffset = bounds.Top - CenterY;
            double bottomOffset = bounds.Bottom - CenterY;

            // --------------------------------------------------------
            // Выход через левую границу — появляется справа,
            // левый край габарита совпадает с area.Right.
            // --------------------------------------------------------

            if (bounds.Right < area.Left)
            {
                CenterX = area.Right - leftOffset;
                wrapped = true;
            }

            // --------------------------------------------------------
            // Выход через правую границу — появляется слева,
            // правый край габарита совпадает с area.Left.
            // --------------------------------------------------------

            else if (bounds.Left > area.Right)
            {
                CenterX = area.Left - rightOffset;
                wrapped = true;
            }

            bounds = BoundingRectangle;

            // --------------------------------------------------------
            // Выход через верхнюю границу — появляется снизу.
            // --------------------------------------------------------

            if (bounds.Bottom < area.Top)
            {
                CenterY = area.Bottom - topOffset;
                wrapped = true;
            }

            // --------------------------------------------------------
            // Выход через нижнюю границу — появляется сверху.
            // --------------------------------------------------------

            else if (bounds.Top > area.Bottom)
            {
                CenterY = area.Top - bottomOffset;
                wrapped = true;
            }

            return wrapped;
        }

        public bool Update(Rectangle2D area)
        {
            Move();
            return CheckAndWrap(area);
        }

        /// <summary>
        /// Отрисовка фигуры в заданной точке.
        /// erase=true — стирание (белым), erase=false — обычная отрисовка.
        /// </summary>
        public abstract void DrawAt(Graphics g, double centerX, double centerY, bool erase);

        public void Draw(Graphics g, bool erase = false)
        {
            DrawAt(g, CenterX, CenterY, erase);
        }

        public override string ToString()
        {
            return $"{GetType().Name} #{Id} [Order={Order}]";
        }
    }
}