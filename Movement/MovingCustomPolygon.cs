using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Lab1_OZR1.Geometry;

namespace Lab1_OZR1.Movement
{
    /// <summary>
    /// Универсальная движущаяся фигура —
    /// замкнутый многоугольник с произвольным набором вершин.
    /// Вершины хранятся в локальных координатах относительно центра.
    /// </summary>
    public class MovingCustomPolygon : MovingShape
    {
        private readonly List<Point2D> _localVertices =
            new List<Point2D>();

        [Browsable(false)]
        public IReadOnlyList<Point2D> LocalVertices
        {
            get { return _localVertices; }
        }

        public MovingCustomPolygon(
            double centerX,
            double centerY,
            IEnumerable<Point2D> absoluteVertices,
            Vector2D velocity)
            : base(centerX, centerY, velocity)
        {
            if (absoluteVertices == null)
            {
                throw new ArgumentNullException(nameof(absoluteVertices));
            }

            List<Point2D> list = new List<Point2D>(absoluteVertices);

            if (list.Count < 3)
            {
                throw new ArgumentException(
                    "Многоугольник должен содержать минимум 3 вершины.",
                    nameof(absoluteVertices));
            }

            foreach (Point2D p in list)
            {
                _localVertices.Add(
                    new Point2D(p.X - centerX, p.Y - centerY));
            }
        }

        public override Rectangle2D BoundingRectangle
        {
            get
            {
                double minX = double.MaxValue;
                double minY = double.MaxValue;
                double maxX = double.MinValue;
                double maxY = double.MinValue;

                foreach (Point2D v in _localVertices)
                {
                    if (v.X < minX) minX = v.X;
                    if (v.Y < minY) minY = v.Y;
                    if (v.X > maxX) maxX = v.X;
                    if (v.Y > maxY) maxY = v.Y;
                }

                return new Rectangle2D(
                    CenterX + minX,
                    CenterY + minY,
                    maxX - minX,
                    maxY - minY
                );
            }
        }

        public override void DrawAt(
            Graphics g,
            double centerX,
            double centerY,
            bool erase)
        {
            Color fillColor = erase ? Color.White : Color;
            Color outlineColor = erase ? Color.White : Color.Black;
            int penWidth = erase ? 4 : 2;

            PointF[] points = new PointF[_localVertices.Count];

            for (int i = 0; i < _localVertices.Count; i++)
            {
                points[i] = new PointF(
                    (float)(centerX + _localVertices[i].X),
                    (float)(centerY + _localVertices[i].Y)
                );
            }

            using (Brush brush = new SolidBrush(fillColor))
            using (Pen pen = new Pen(outlineColor, penWidth))
            {
                g.FillPolygon(brush, points);
                g.DrawPolygon(pen, points);
            }
        }
    }
}