using System;
using Lab1_OZR1.Geometry;

namespace Lab1_OZR1.Movement
{
    /// <summary>
    /// Движущаяся равнобедренная перевёрнутая трапеция.
    ///
    /// Движение происходит с учётом силы тяготения.
    /// При столкновении с границами области выполняется
    /// отскок по закону отражения; потеря энергии при ударе
    /// описывается коэффициентом восстановления.
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
        /// Текущий вектор скорости.
        /// </summary>
        public Vector2D Velocity { get; private set; }

        /// <summary>
        /// Масса фигуры.
        /// </summary>
        public double Mass { get; }

        /// <summary>
        /// Ускорение свободного падения.
        /// Направлено вертикально вниз, поэтому положительно по Y.
        /// </summary>
        public double Gravity { get; }

        /// <summary>
        /// Коэффициент восстановления при отскоке (0..1).
        ///
        /// 1.0 — абсолютно упругий удар: энергия сохраняется,
        ///       гравитация полностью компенсирует импульс отскока.
        /// 0.0 — удар полностью гасится.
        /// </summary>
        public double Restitution { get; }

        /// <summary>
        /// Жёсткий предел модуля скорости.
        /// Служит страховкой от накопления численной ошибки.
        /// </summary>
        public double MaxSpeed { get; }

        public MovingTrapezoid(
            double centerX,
            double centerY,
            double topWidth,
            double bottomWidth,
            double height,
            Vector2D velocity,
            double mass = 1.0,
            double gravity = 0.2,
            double restitution = 1.0,
            double maxSpeed = 100.0)
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

            if (mass <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(mass));
            }

            if (restitution < 0.0 || restitution > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(restitution));
            }

            if (maxSpeed <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSpeed));
            }

            _centerX = centerX;
            _centerY = centerY;

            TopWidth = topWidth;
            BottomWidth = bottomWidth;
            Height = height;

            Velocity = velocity;

            Mass = mass;
            Gravity = gravity;
            Restitution = restitution;
            MaxSpeed = maxSpeed;
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
        /// Выполняет один шаг движения с учётом гравитации.
        ///
        /// v_y(k+1) = v_y(k) + g
        /// x(k+1)   = x(k) + v_x(k+1)
        /// y(k+1)   = y(k) + v_y(k+1)
        /// </summary>
        public void Move()
        {
            // Гравитация направлена вниз => увеличивает Y-скорость.
            Velocity = Velocity + new Vector2D(0, Gravity);

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
        // ОТСКОК ОТ ГРАНИЦ
        // ============================================================

        /// <summary>
        /// Обрабатывает столкновение с границами области.
        ///
        /// При выходе за границу:
        /// 1. фигура возвращается внутрь области;
        /// 2. нормальная составляющая скорости отражается
        ///    и умножается на коэффициент восстановления.
        ///
        /// Дополнительный импульс не прибавляется: импульс отскока
        /// компенсируется гравитацией на участке подъёма, поэтому
        /// скорость фигуры не растёт бесконечно.
        ///
        /// Возвращает true, если был хотя бы один отскок.
        /// </summary>
        private bool HandleBounce(Rectangle2D area)
        {
            bool bounced = false;

            Rectangle2D bounds = BoundingRectangle;

            // --------------------------------------------------------
            // Левая граница.
            // Нормаль направлена вправо: (1; 0).
            // --------------------------------------------------------
            if (bounds.Left < area.Left)
            {
                _centerX += area.Left - bounds.Left;

                Velocity = new Vector2D(
                    -Velocity.X * Restitution,
                    Velocity.Y
                );

                bounced = true;
            }
            // --------------------------------------------------------
            // Правая граница.
            // Нормаль направлена влево: (-1; 0).
            // --------------------------------------------------------
            else if (bounds.Right > area.Right)
            {
                _centerX += area.Right - bounds.Right;

                Velocity = new Vector2D(
                    -Velocity.X * Restitution,
                    Velocity.Y
                );

                bounced = true;
            }

            bounds = BoundingRectangle;

            // --------------------------------------------------------
            // Верхняя граница.
            // Нормаль направлена вниз: (0; 1).
            // --------------------------------------------------------
            if (bounds.Top < area.Top)
            {
                _centerY += area.Top - bounds.Top;

                Velocity = new Vector2D(
                    Velocity.X,
                    -Velocity.Y * Restitution
                );

                bounced = true;
            }
            // --------------------------------------------------------
            // Нижняя граница.
            // Нормаль направлена вверх: (0; -1).
            // --------------------------------------------------------
            else if (bounds.Bottom > area.Bottom)
            {
                _centerY += area.Bottom - bounds.Bottom;

                Velocity = new Vector2D(
                    Velocity.X,
                    -Velocity.Y * Restitution
                );

                bounced = true;
            }

            // --------------------------------------------------------
            // Страховка: жёсткий предел модуля скорости.
            // --------------------------------------------------------
            if (Velocity.Length > MaxSpeed)
            {
                double k = MaxSpeed / Velocity.Length;

                Velocity = new Vector2D(
                    Velocity.X * k,
                    Velocity.Y * k
                );
            }

            return bounced;
        }

        // ============================================================
        // ПОЛНЫЙ ШАГ
        // ============================================================

        /// <summary>
        /// Выполняет один полный шаг алгоритма:
        ///
        /// 1. применяет гравитацию и перемещает фигуру;
        /// 2. проверяет столкновения с границами;
        /// 3. при необходимости выполняет отскок.
        /// </summary>
        public bool Update(Rectangle2D area)
        {
            Move();

            return HandleBounce(area);
        }
    }
}