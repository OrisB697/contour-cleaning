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

        private double _rotationAngle;

        public double RotationAngle
        {
            get { return _rotationAngle; }
        }

        public double AngularVelocity { get; private set; }

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
            Vector2D velocity,
            double angularVelocity = 0.0)
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
            AngularVelocity = angularVelocity;
            _rotationAngle = 0.0;
        }

        // ============================================================
        // ВЕРШИНЫ ФИГУРЫ
        // ============================================================

        private Point2D RotateAroundCenter(Point2D point)
        {
            return point.Rotate(
                new Point2D(_centerX, _centerY),
                _rotationAngle
            );
        }

        public Point2D TopLeft
        {
            get
            {
                Point2D basePoint = new Point2D(
                    _centerX - TopWidth / 2.0,
                    _centerY - Height / 2.0
                );
                return RotateAroundCenter(basePoint);
            }
        }

        public Point2D TopRight
        {
            get
            {
                Point2D basePoint = new Point2D(
                    _centerX + TopWidth / 2.0,
                    _centerY - Height / 2.0
                );
                return RotateAroundCenter(basePoint);
            }
        }

        public Point2D BottomLeft
        {
            get
            {
                Point2D basePoint = new Point2D(
                    _centerX - BottomWidth / 2.0,
                    _centerY + Height / 2.0
                );
                return RotateAroundCenter(basePoint);
            }
        }

        public Point2D BottomRight
        {
            get
            {
                Point2D basePoint = new Point2D(
                    _centerX + BottomWidth / 2.0,
                    _centerY + Height / 2.0
                );
                return RotateAroundCenter(basePoint);
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
                Point2D[] vertices =
                {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft
        };

                double minX = vertices[0].X;
                double maxX = vertices[0].X;
                double minY = vertices[0].Y;
                double maxY = vertices[0].Y;

                for (int i = 1; i < vertices.Length; i++)
                {
                    if (vertices[i].X < minX) minX = vertices[i].X;
                    if (vertices[i].X > maxX) maxX = vertices[i].X;
                    if (vertices[i].Y < minY) minY = vertices[i].Y;
                    if (vertices[i].Y > maxY) maxY = vertices[i].Y;
                }

                return new Rectangle2D(
                    minX,
                    minY,
                    maxX - minX,
                    maxY - minY
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

            Rotate(AngularVelocity);
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

            if (bounds.Right < area.Left)
            {
                _centerX = area.Right - (bounds.Left - _centerX);
                wrapped = true;
            }
            else if (bounds.Left > area.Right)
            {
                _centerX = area.Left - (bounds.Right - _centerX);
                wrapped = true;
            }

            bounds = BoundingRectangle;

            if (bounds.Bottom < area.Top)
            {
                _centerY = area.Bottom - (bounds.Top - _centerY);
                wrapped = true;
            }
            else if (bounds.Top > area.Bottom)
            {
                _centerY = area.Top - (bounds.Bottom - _centerY);
                wrapped = true;
            }

            return wrapped;
        }

        public void SetAngularVelocity(double angularVelocity)
        {
            AngularVelocity = angularVelocity;
        }

        public void Rotate(double angleRadians)
        {
            _rotationAngle += angleRadians;

            while (_rotationAngle > Math.PI)
            {
                _rotationAngle -= 2.0 * Math.PI;
            }

            while (_rotationAngle < -Math.PI)
            {
                _rotationAngle += 2.0 * Math.PI;
            }
        }

        public void RotateDegrees(double angleDegrees)
        {
            Rotate(angleDegrees * Math.PI / 180.0);
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