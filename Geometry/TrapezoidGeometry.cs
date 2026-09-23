using System;

namespace Lab1_OZR1.Geometry
{
    /// <summary>
    /// Геометрическое описание равнобедренной перевёрнутой трапеции.
    /// </summary>
    public class TrapezoidGeometry
    {
        public double CenterX { get; }

        public double CenterY { get; }

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

        public TrapezoidGeometry(
            double centerX,
            double centerY,
            double topWidth,
            double bottomWidth,
            double height)
        {
            if (topWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(topWidth));
            }

            if (bottomWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bottomWidth));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if (bottomWidth > topWidth)
            {
                throw new ArgumentException(
                    "Для перевёрнутой трапеции нижнее основание " +
                    "не должно быть больше верхнего.");
            }

            CenterX = centerX;
            CenterY = centerY;

            TopWidth = topWidth;
            BottomWidth = bottomWidth;
            Height = height;
        }

        public Point2D TopLeft
        {
            get
            {
                return new Point2D(
                    CenterX - TopWidth / 2.0,
                    CenterY - Height / 2.0
                );
            }
        }

        public Point2D TopRight
        {
            get
            {
                return new Point2D(
                    CenterX + TopWidth / 2.0,
                    CenterY - Height / 2.0
                );
            }
        }

        public Point2D BottomLeft
        {
            get
            {
                return new Point2D(
                    CenterX - BottomWidth / 2.0,
                    CenterY + Height / 2.0
                );
            }
        }

        public Point2D BottomRight
        {
            get
            {
                return new Point2D(
                    CenterX + BottomWidth / 2.0,
                    CenterY + Height / 2.0
                );
            }
        }

        /// <summary>
        /// Ортогональный габаритный прямоугольник.
        /// </summary>
        public Rectangle2D BoundingRectangle
        {
            get
            {
                return new Rectangle2D(
                    CenterX - TopWidth / 2.0,
                    CenterY - Height / 2.0,
                    TopWidth,
                    Height
                );
            }
        }

        public Triangle2D BoundingTriangle
        {
            get
            {
                double w = TopWidth;
                double h = Height;

                double a = Math.Max(w, 2.0 * h / Math.Sqrt(3.0));

                double hTri = a * Math.Sqrt(3.0) / 2.0;

                Point2D v1 = new Point2D(CenterX, CenterY - 2.0 * hTri / 3.0);
                Point2D v2 = new Point2D(CenterX - a / 2, CenterY + hTri / 3.0);
                Point2D v3 = new Point2D(CenterX + a / 2, CenterY + hTri / 3.0);

                return new Triangle2D(v1, v2, v3);
            }
        }
    }
}