using System;
using Lab1_OZR1.Geometry;

namespace Lab1_OZR1.Movement
{
    public class MovingTrapezoid
    {
        private double _centerX;
        private double _centerY;
        private Vector2D _velocity;
        private double _rotationAngle;
        private double _scale;

        public double CenterX { get { return _centerX; } }
        public double CenterY { get { return _centerY; } }

        public double TopWidth { get; }
        public double BottomWidth { get; }
        public double Height { get; }

        public Vector2D Velocity { get { return _velocity; } }

        public double RotationAngle
        {
            get { return _rotationAngle; }
            set { _rotationAngle = value; }
        }

        public double Scale
        {
            get { return _scale; }
            set
            {
                if (value < 0.5) value = 0.5;
                if (value > 2.0) value = 2.0;
                _scale = value;
            }
        }

        public MovingTrapezoid(
            double centerX, double centerY,
            double topWidth, double bottomWidth, double height,
            Vector2D velocity)
        {
            if (topWidth <= 0) throw new ArgumentOutOfRangeException(nameof(topWidth));
            if (bottomWidth <= 0) throw new ArgumentOutOfRangeException(nameof(bottomWidth));
            if (bottomWidth > topWidth) throw new ArgumentException("BottomWidth не может быть больше TopWidth.");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _centerX = centerX;
            _centerY = centerY;
            TopWidth = topWidth;
            BottomWidth = bottomWidth;
            Height = height;
            _velocity = velocity;
            _rotationAngle = 0.0;
            _scale = 1.0;
        }

        private Point2D Transform(double x, double y)
        {
            double sx = _centerX + (x - _centerX) * _scale;
            double sy = _centerY + (y - _centerY) * _scale;

            double dx = sx - _centerX;
            double dy = sy - _centerY;
            double cos = Math.Cos(_rotationAngle);
            double sin = Math.Sin(_rotationAngle);

            return new Point2D(
                _centerX + dx * cos - dy * sin,
                _centerY + dx * sin + dy * cos);
        }

        public Point2D TopLeft
        {
            get { return Transform(_centerX - TopWidth / 2.0, _centerY - Height / 2.0); }
        }

        public Point2D TopRight
        {
            get { return Transform(_centerX + TopWidth / 2.0, _centerY - Height / 2.0); }
        }

        public Point2D BottomLeft
        {
            get { return Transform(_centerX - BottomWidth / 2.0, _centerY + Height / 2.0); }
        }

        public Point2D BottomRight
        {
            get { return Transform(_centerX + BottomWidth / 2.0, _centerY + Height / 2.0); }
        }

        public Triangle2D BoundingTriangle
        {
            get
            {
                double w = TopWidth * _scale;
                double h = Height * _scale;
                double a = Math.Max(w, 2.0 * h / Math.Sqrt(3.0));
                double hTri = a * Math.Sqrt(3.0) / 2.0;

                Point2D v1 = new Point2D(_centerX, _centerY - 2.0 * hTri / 3.0);
                Point2D v2 = new Point2D(_centerX - a / 2.0, _centerY + hTri / 3.0);
                Point2D v3 = new Point2D(_centerX + a / 2.0, _centerY + hTri / 3.0);

                return new Triangle2D(v1, v2, v3);
            }
        }

        public void SetVelocity(Vector2D velocity)
        {
            _velocity = velocity;
        }

        public void Move()
        {
            _centerX += _velocity.X;
            _centerY += _velocity.Y;
        }

        public void Move(int steps)
        {
            if (steps < 0) throw new ArgumentOutOfRangeException(nameof(steps));
            for (int i = 0; i < steps; i++) Move();
        }

        public bool IsInside(Rectangle2D area)
        {
            return BoundingTriangle.IsInside(area);
        }

        public bool IsOutside(Rectangle2D area)
        {
            return BoundingTriangle.IsOutside(area);
        }

        public bool CheckAndWrap(Rectangle2D area)
        {
            bool wrapped = false;

            double w = TopWidth * _scale;
            double h = Height * _scale;
            double a = Math.Max(w, 2.0 * h / Math.Sqrt(3.0));
            double hTri = a * Math.Sqrt(3.0) / 2.0;

            double halfW = a / 2.0;
            double upDist = 2.0 * hTri / 3.0;
            double dnDist = hTri / 3.0;

            Triangle2D tri = BoundingTriangle;

            if (tri.AllLeftOf(area.Left))
            {
                _centerX = area.Right + halfW;
                wrapped = true;
            }
            else if (tri.AllRightOf(area.Right))
            {
                _centerX = area.Left - halfW;
                wrapped = true;
            }

            tri = BoundingTriangle;

            if (tri.AllAbove(area.Top))
            {
                _centerY = area.Bottom + upDist;
                wrapped = true;
            }
            else if (tri.AllBelow(area.Bottom))
            {
                _centerY = area.Top - dnDist;
                wrapped = true;
            }

            return wrapped;
        }

        public bool Update(Rectangle2D area)
        {
            Move();
            return CheckAndWrap(area);
        }
    }
}