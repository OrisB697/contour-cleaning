using System;
using System.Drawing;
using System.Windows.Forms;
using Lab1_OZR1.Geometry;
using Lab1_OZR1.Movement;

namespace Lab1_OZR1
{
    public partial class Form1 : Form
    {
        private MovingTrapezoid _trapezoid;
        private Rectangle2D _area;

        private double _prevCenterX;
        private double _prevCenterY;

        private Timer _timer;
        private double _angle;         // текущий угол поворота фона
        private double _paintedAngle;  // угол отрисованный
        private Bitmap _buffer;        // задний буфер
        private Graphics _bufferGraphics;

        private const int RINGS = 12;
        private const int PER_RING = 8;
        private const double PI = Math.PI;

        public Form1()
        {
            InitializeComponent();

            this.ClientSize = new Size(900, 700);
            this.Text = "ЛР1 — КВ2 — контурная очистка (ДЗ.А1: треугольный габарит)";

            // Двойная буферизация
            this.DoubleBuffered = true;

            // Область движения
            _area = new Rectangle2D(0, 0, ClientSize.Width, ClientSize.Height);

            _trapezoid = new MovingTrapezoid(
                centerX: 450,
                centerY: 350,
                topWidth: 200,
                bottomWidth: 100,
                height: 120,
                velocity: new Vector2D(3, 2)
            );

            _prevCenterX = _trapezoid.CenterX;
            _prevCenterY = _trapezoid.CenterY;

            _buffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            _bufferGraphics = Graphics.FromImage(_buffer);
            ClearBuffer();

            _timer = new Timer();
            _timer.Interval = 16;
            _timer.Tick += Timer_Tick;
            _timer.Start();

            this.Resize += Form1_Resize;
        }

        private void ClearBuffer()
        {
            _bufferGraphics.Clear(Color.White);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            _area = new Rectangle2D(0, 0, ClientSize.Width, ClientSize.Height);

            _bufferGraphics?.Dispose();
            _buffer?.Dispose();

            _buffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            _bufferGraphics = Graphics.FromImage(_buffer);
            ClearBuffer();

            Invalidate();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _prevCenterX = _trapezoid.CenterX;
            _prevCenterY = _trapezoid.CenterY;

            _trapezoid.Update(_area);

            _angle += 0.02;

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawTrapezoidShape(
                _bufferGraphics,
                _prevCenterX, _prevCenterY,
                _trapezoid.TopWidth, _trapezoid.BottomWidth, _trapezoid.Height,
                erase: true);

            DrawBackground(_bufferGraphics, _paintedAngle, erase: true);

            DrawBackground(_bufferGraphics, _angle, erase: false);

            DrawTrapezoidShape(
                _bufferGraphics,
                _trapezoid.CenterX, _trapezoid.CenterY,
                _trapezoid.TopWidth, _trapezoid.BottomWidth, _trapezoid.Height,
                erase: false);

            e.Graphics.DrawImageUnscaled(_buffer, 0, 0);

            _paintedAngle = _angle;
        }

        private void DrawBackground(Graphics g, double angle, bool erase)
        {
            Color fillColor = Color.White;
            Color penColor = erase ? Color.White : Color.Black;
            int penWidth = erase ? 4 : 2;

            using (Brush brush = new SolidBrush(fillColor))
            using (Pen pen = new Pen(penColor, penWidth))
            {
                int cx = ClientSize.Width / 2;
                int cy = ClientSize.Height / 2;

                for (int ring = 0; ring < RINGS; ring++)
                {
                    for (int i = 0; i < PER_RING; i++)
                    {
                        double radius = (ring + 1) * 40.0;
                        double a = (2.0 * PI / PER_RING) * i
                                 + (ring + 1) * 0.15
                                 + angle;

                        double x = cx + radius * Math.Cos(a);
                        double y = cy + radius * Math.Sin(a);
                        double size = 10.0 + (ring + 1) * 1.5;

                        PointF[] pts = new PointF[4];
                        pts[0] = new PointF((float)x, (float)(y - size));
                        pts[1] = new PointF((float)(x + size), (float)y);
                        pts[2] = new PointF((float)x, (float)(y + size));
                        pts[3] = new PointF((float)(x - size), (float)y);

                        g.FillPolygon(brush, pts);
                        g.DrawPolygon(pen, pts);
                    }
                }
            }
        }

        private void DrawTrapezoidShape
        (
            Graphics g,
            double centerX, double centerY,
            double topWidth, double bottomWidth, double height,
            bool erase
        )
        {
            // ----- Вершины трапеции -----
            double tlX = centerX - topWidth / 2.0;
            double tlY = centerY - height / 2.0;
            double trX = centerX + topWidth / 2.0;
            double trY = centerY - height / 2.0;
            double blX = centerX - bottomWidth / 2.0;
            double blY = centerY + height / 2.0;
            double brX = centerX + bottomWidth / 2.0;
            double brY = centerY + height / 2.0;

            // ----- Вершины габаритного равностороннего треугольника (ДЗ.А1) -----
            // Сторона a подобрана так, чтобы треугольник полностью покрывал
            // габаритный прямоугольник трапеции W x H (W = topWidth).
            double w = topWidth;
            double h = height;
            double a = Math.Max(w, 2.0 * h / Math.Sqrt(3.0));
            double hTri = a * Math.Sqrt(3.0) / 2.0;

            PointF[] trianglePts =
            {
                new PointF((float)centerX,
                           (float)(centerY - 2.0 * hTri / 3.0)),
                new PointF((float)(centerX - a / 2.0),
                           (float)(centerY + hTri / 3.0)),
                new PointF((float)(centerX + a / 2.0),
                           (float)(centerY + hTri / 3.0))
            };

            // ----- Цвета и толщина пера -----
            Color fillRect = Color.White;
            Color leftColor = Color.White;
            Color midColor = Color.White;
            Color rightColor = Color.White;
            Color outline = erase ? Color.White : Color.Black;
            int penWidth = erase ? 4 : 2;

            if (!erase)
            {
                fillRect = Color.LightGray;
                leftColor = Color.LightBlue;
                midColor = Color.LightGreen;
                rightColor = Color.LightPink;
            }

            using (Brush bRect = new SolidBrush(fillRect))
            using (Brush bLeft = new SolidBrush(leftColor))
            using (Brush bMid = new SolidBrush(midColor))
            using (Brush bRight = new SolidBrush(rightColor))
            using (Pen pen = new Pen(outline, penWidth))
            {
                g.FillPolygon(bRect, trianglePts);
                g.DrawPolygon(pen, trianglePts);

                PointF[] leftPts =
                {
                    new PointF((float)tlX, (float)tlY),
                    new PointF((float)blX, (float)blY),
                    new PointF((float)blX, (float)tlY)
                };
                g.FillPolygon(bLeft, leftPts);

                PointF[] midPts =
                {
                    new PointF((float)blX, (float)tlY),
                    new PointF((float)brX, (float)tlY),
                    new PointF((float)brX, (float)blY),
                    new PointF((float)blX, (float)blY)
                };
                g.FillPolygon(bMid, midPts);

                PointF[] rightPts =
                {
                    new PointF((float)trX, (float)trY),
                    new PointF((float)brX, (float)brY),
                    new PointF((float)brX, (float)trY)
                };
                g.FillPolygon(bRight, rightPts);

                PointF[] trapezoidPts =
                {
                    new PointF((float)tlX, (float)tlY),
                    new PointF((float)trX, (float)trY),
                    new PointF((float)brX, (float)brY),
                    new PointF((float)blX, (float)blY)
                };
                g.DrawPolygon(pen, trapezoidPts);

                g.DrawLine(pen,
                    (float)blX, (float)blY,
                    (float)blX, (float)tlY);
                g.DrawLine(pen,
                    (float)brX, (float)brY,
                    (float)brX, (float)trY);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _timer?.Stop();
            _timer?.Dispose();
            _bufferGraphics?.Dispose();
            _buffer?.Dispose();

            base.OnFormClosed(e);
        }
    }
}