using System;
using Lab1_OZR1.Geometry;

namespace Lab1_OZR1.Movement
{
    /// <summary>
    /// Движущаяся равнобедренная перевёрнутая трапеция.
    ///
    /// Габаритом фигуры является описанная окружность.
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
        // ГАБАРИТНАЯ ОКРУЖНОСТЬ
        // ============================================================

        /// <summary>
        /// Радиус окружности, описанной вокруг трапеции.
        ///
        /// Самая удалённая вершина — верхняя (TopWidth >= BottomWidth):
        ///   R = sqrt( (TopWidth/2)^2 + (Height/2)^2 )
        /// </summary>
        public double Radius
        {
            get
            {
                double halfTop = TopWidth / 2.0;
                double halfHeight = Height / 2.0;

                return Math.Sqrt(
                    halfTop * halfTop +
                    halfHeight * halfHeight);
            }
        }

        /// <summary>
        /// Описанная окружность как габарит фигуры.
        /// </summary>
        public Circle2D CircumscribedCircle
        {
            get
            {
                return new Circle2D(_centerX, _centerY, Radius);
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
        /// Проверяет, полностью ли описанная окружность
        /// находится внутри области движения.
        /// </summary>
        public bool IsInside(Rectangle2D area)
        {
            Circle2D circle = CircumscribedCircle;

            return circle.Left >= area.Left &&
                   circle.Right <= area.Right &&
                   circle.Top >= area.Top &&
                   circle.Bottom <= area.Bottom;
        }

        /// <summary>
        /// Проверяет, полностью ли описанная окружность
        /// находится за пределами области движения.
        /// </summary>
        public bool IsOutside(Rectangle2D area)
        {
            Circle2D circle = CircumscribedCircle;

            return circle.Right < area.Left ||
                   circle.Left > area.Right ||
                   circle.Bottom < area.Top ||
                   circle.Top > area.Bottom;
        }

        // ============================================================
        // ПЕРЕНОС ЧЕРЕЗ ГРАНИЦУ
        // ============================================================

        /// <summary>
        /// Переносит фигуру на противоположную сторону области,
        /// если её описанная окружность полностью вышла
        /// за соответствующую границу.
        ///
        /// Возвращает true, если был выполнен перенос.
        /// </summary>
        public bool CheckAndWrap(Rectangle2D area)
        {
            bool wrapped = false;

            double r = Radius;
            Circle2D circle = CircumscribedCircle;

            // --------------------------------------------------------
            // Выход через левую границу.
            // Фигура появляется справа.
            // --------------------------------------------------------

            if (circle.Right < area.Left)
            {
                _centerX = area.Right + r;
                wrapped = true;
            }

            // --------------------------------------------------------
            // Выход через правую границу.
            // Фигура появляется слева.
            // --------------------------------------------------------

            else if (circle.Left > area.Right)
            {
                _centerX = area.Left - r;
                wrapped = true;
            }

            // Пересчитываем окружность после возможного сдвига по X
            circle = CircumscribedCircle;

            // --------------------------------------------------------
            // Выход через верхнюю границу.
            // Фигура появляется снизу.
            // --------------------------------------------------------

            if (circle.Bottom < area.Top)
            {
                _centerY = area.Bottom + r;
                wrapped = true;
            }

            // --------------------------------------------------------
            // Выход через нижнюю границу.
            // Фигура появляется сверху.
            // --------------------------------------------------------

            else if (circle.Top > area.Bottom)
            {
                _centerY = area.Top - r;
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
        /// 2. получение габаритов (описанной окружности);
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