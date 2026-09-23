using System;
using Lab1_OZR1.Geometry;

namespace Lab1_OZR1.Movement
{
    /// <summary>
    /// Движущаяся равнобедренная перевёрнутая трапеция.
    ///
    /// Класс содержит математическую и программную логику
    /// перемещения фигуры и обработки выхода за границы
    /// области движения.
    /// </summary>
    public class MovingTrapezoid
    {
        private double _centerX;
        private double _centerY;

        public double CenterX
        {
            get
            {
                return _centerX;
            }
        }

        public double CenterY
        {
            get
            {
                return _centerY;
            }
        }

        /// <summary>
        /// Ширина верхнего основания.
        /// </summary>
        public double TopWidth { get; }

        /// <summary>
        /// Ширина нижнего основания.
        /// </summary>
        public double BottomWidth { get; }

        /// <summary>
        /// Высота трапеции.
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// Направляющий вектор движения.
        /// </summary>
        public Vector2D Velocity { get; private set; }

        public MovingTrapezoid(
            double centerX,
            double centerY,
            double topWidth,
            double bottomWidth,
            double height,
            Vector2D velocity)
        {
            if (topWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(topWidth));
            }

            if (bottomWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bottomWidth));
            }

            if (bottomWidth > topWidth)
            {
                throw new ArgumentException(
                    "BottomWidth не может быть больше TopWidth.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            _centerX = centerX;
            _centerY = centerY;

            TopWidth = topWidth;
            BottomWidth = bottomWidth;
            Height = height;

            Velocity = velocity;
        }

        // ============================================================
        // ВЕРШИНЫ ФИГУРЫ
        // ============================================================

        public Point2D TopLeft
        {
            get
            {
                return new Point2D(
                    _centerX - TopWidth / 2.0,
                    _centerY - Height / 2.0
                );
            }
        }

        public Point2D TopRight
        {
            get
            {
                return new Point2D(
                    _centerX + TopWidth / 2.0,
                    _centerY - Height / 2.0
                );
            }
        }

        public Point2D BottomLeft
        {
            get
            {
                return new Point2D(
                    _centerX - BottomWidth / 2.0,
                    _centerY + Height / 2.0
                );
            }
        }

        public Point2D BottomRight
        {
            get
            {
                return new Point2D(
                    _centerX + BottomWidth / 2.0,
                    _centerY + Height / 2.0
                );
            }
        }

        // ============================================================
        // ГАБАРИТНЫЙ ПРЯМОУГОЛЬНИК
        // ============================================================

        /// <summary>
        /// Ортогональный прямоугольник, описывающий габариты фигуры.
        /// </summary>
        public Rectangle2D BoundingRectangle
        {
            get
            {
                return new Rectangle2D(
                    _centerX - TopWidth / 2.0,
                    _centerY - Height / 2.0,
                    TopWidth,
                    Height
                );
            }
        }

        // ============================================================
        // СКОРОСТЬ
        // ============================================================

        public void SetVelocity(Vector2D velocity)
        {
            Velocity = velocity;
        }

        // ============================================================
        // ПЕРЕМЕЩЕНИЕ
        // ============================================================

        /// <summary>
        /// Выполняет один шаг прямолинейного движения.
        ///
        /// x(k+1) = x(k) + vx
        /// y(k+1) = y(k) + vy
        /// </summary>
        public void Move()
        {
            _centerX += Velocity.X;
            _centerY += Velocity.Y;
        }

        /// <summary>
        /// Выполняет несколько шагов движения.
        /// </summary>
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

        // ============================================================
        // ПРОВЕРКА ГРАНИЦ
        // ============================================================

        /// <summary>
        /// Проверяет, полностью ли фигура находится внутри
        /// области движения.
        /// </summary>
        public bool IsInside(Rectangle2D area)
        {
            Rectangle2D bounds = BoundingRectangle;

            return bounds.Left >= area.Left &&
                   bounds.Right <= area.Right &&
                   bounds.Top >= area.Top &&
                   bounds.Bottom <= area.Bottom;
        }

        /// <summary>
        /// Проверяет, находится ли фигура полностью за пределами
        /// области движения.
        /// </summary>
        public bool IsOutside(Rectangle2D area)
        {
            Rectangle2D bounds = BoundingRectangle;

            return bounds.Right < area.Left ||
                   bounds.Left > area.Right ||
                   bounds.Bottom < area.Top ||
                   bounds.Top > area.Bottom;
        }

        // ============================================================
        // ПЕРЕНОС ЧЕРЕЗ ГРАНИЦУ
        // ============================================================

        /// <summary>
        /// Переносит фигуру на противоположную сторону области,
        /// если её габаритный прямоугольник полностью вышел
        /// за соответствующую границу.
        ///
        /// Возвращает true, если был выполнен перенос.
        /// </summary>
        public bool CheckAndWrap(Rectangle2D area)
        {
            bool wrapped = false;

            Rectangle2D bounds = BoundingRectangle;

            // --------------------------------------------------------
            // Выход через левую границу.
            // Фигура появляется справа.
            // --------------------------------------------------------

            if (bounds.Right < area.Left)
            {
                _centerX = area.Right + TopWidth / 2.0;

                wrapped = true;
            }

            // --------------------------------------------------------
            // Выход через правую границу.
            // Фигура появляется слева.
            // --------------------------------------------------------

            else if (bounds.Left > area.Right)
            {
                _centerX = area.Left - TopWidth / 2.0;

                wrapped = true;
            }

            bounds = BoundingRectangle;

            // --------------------------------------------------------
            // Выход через верхнюю границу.
            // Фигура появляется снизу.
            // --------------------------------------------------------

            if (bounds.Bottom < area.Top)
            {
                _centerY = area.Bottom + Height / 2.0;

                wrapped = true;
            }

            // --------------------------------------------------------
            // Выход через нижнюю границу.
            // Фигура появляется сверху.
            // --------------------------------------------------------

            else if (bounds.Top > area.Bottom)
            {
                _centerY = area.Top - Height / 2.0;

                wrapped = true;
            }

            return wrapped;
        }

        // ============================================================
        // ПОЛНЫЙ ШАГ
        // ============================================================

        /// <summary>
        /// Выполняет один полный шаг алгоритма:
        ///
        /// 1. перемещение;
        /// 2. получение габаритов;
        /// 3. проверка выхода;
        /// 4. перенос на противоположную сторону.
        /// </summary>
        public bool Update(Rectangle2D area)
        {
            Move();

            return CheckAndWrap(area);
        }
    }
}