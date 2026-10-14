using Lab1_OZR1.Geometry;
using Lab1_OZR1.Movement;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Lab1_OZR1
{
    public partial class Form1 : Form
    {
        private MovingTrapezoid _trapezoid;
        private Rectangle2D _area;

        private double _prevCenterX;
        private double _prevCenterY;

        private Timer _timer;
        private double _angle;
        private double _paintedAngle;
        private Bitmap _buffer;
        private Graphics _bufferGraphics;

        private Bitmap _backgroundImage;
        private Bitmap _stretchedBackground;

        private const int RINGS = 17;
        private const int PER_RING = 8;
        private const double PI = Math.PI;

        private const int ERASE_PEN_EXTRA = 2;

        private const string BackgroundFileName =
            "the-center-of-milky-way-galaxy-and-space-dust-in-the-universe-night-starry-sky-with-stars-photo.jpg";

        public Form1()
        {
            InitializeComponent();

            this.ClientSize = new Size(900, 700);
            this.Text = "ЛР1 — КВ2 — контурная очистка";

            this.DoubleBuffered = true;

            _backgroundImage = LoadBackgroundImage();

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

            ResetBuffer();

            _timer = new Timer();
            _timer.Interval = 16;
            _timer.Tick += Timer_Tick;
            _timer.Start();

            this.Resize += Form1_Resize;
        }

        private Bitmap LoadBackgroundImage()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(baseDir, "assets", BackgroundFileName);

                if (File.Exists(path))
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        return new Bitmap(fs);
                }

                string altPath = Path.Combine("assets", BackgroundFileName);
                if (File.Exists(altPath))
                {
                    using (var fs = new FileStream(altPath, FileMode.Open, FileAccess.Read))
                        return new Bitmap(fs);
                }
            }
            catch
            {
            }

            return null;
        }

        private void ResetBuffer()
        {
            _bufferGraphics?.Dispose();
            _buffer?.Dispose();
            _stretchedBackground?.Dispose();

            _buffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            _bufferGraphics = Graphics.FromImage(_buffer);

            if (_backgroundImage != null)
            {
                _stretchedBackground = new Bitmap(ClientSize.Width, ClientSize.Height);
                using (var g = Graphics.FromImage(_stretchedBackground))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(
                        _backgroundImage,
                        0, 0,
                        _stretchedBackground.Width,
                        _stretchedBackground.Height);
                }

                _bufferGraphics.DrawImageUnscaled(_stretchedBackground, 0, 0);
            }
            else
            {
                _stretchedBackground = null;
                _bufferGraphics.Clear(Color.White);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            _area = new Rectangle2D(0, 0, ClientSize.Width, ClientSize.Height);

            ResetBuffer();

            Invalidate();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _prevCenterX = _trapezoid.CenterX;
            _prevCenterY = _trapezoid.CenterY;

            _trapezoid.Update(_area);

            _angle = (_angle + 0.02) % (2 * PI);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            EraseGrid(_bufferGraphics, _paintedAngle);

            EraseTrapezoid(
                _bufferGraphics,
                _prevCenterX, _prevCenterY,
                _trapezoid.TopWidth, _trapezoid.BottomWidth, _trapezoid.Height);

            DrawGrid(_bufferGraphics, _angle);

            DrawTrapezoidShape(
                _bufferGraphics,
                _trapezoid.CenterX, _trapezoid.CenterY,
                _trapezoid.TopWidth, _trapezoid.BottomWidth, _trapezoid.Height);

            e.Graphics.DrawImageUnscaled(_buffer, 0, 0);

            _paintedAngle = _angle;
        }

        private void DrawRombs(Graphics g, Pen pen, double angle)
        {
            int cx = ClientSize.Width / 2;
            int cy = ClientSize.Height / 2;

            int s = 50;

            for (int colRomb = 0; colRomb < RINGS; colRomb++)
            {
                double a = angle + colRomb * 3;

                PointF[] romb = new PointF[4];
                romb[0] = RotatePoint(cx, cy - s, cx, cy, a);
                romb[1] = RotatePoint(cx + s, cy, cx, cy, a);
                romb[2] = RotatePoint(cx, cy + s, cx, cy, a);
                romb[3] = RotatePoint(cx - s, cy, cx, cy, a);

                s += 20;

                g.DrawPolygon(pen, romb);
            }
        }

        private void EraseGrid(Graphics g, double angle)
        {
            if (_stretchedBackground != null)
            {
                using (var brush = new TextureBrush(_stretchedBackground, WrapMode.Tile))
                using (var pen = new Pen(brush, 2 + ERASE_PEN_EXTRA))
                {
                    DrawRombs(g, pen, angle);
                }
            }
            else
            {
                using (var pen = new Pen(Color.White, 2 + ERASE_PEN_EXTRA))
                {
                    DrawRombs(g, pen, angle);
                }
            }
        }

        private void DrawGrid(Graphics g, double angle)
        {
            using (var pen = new Pen(Color.Black, 2))
            {
                DrawRombs(g, pen, angle);
            }
        }

        private void DrawTrapezoidCore(
            Graphics g,
            double centerX, double centerY,
            double topWidth, double bottomWidth, double height,
            Brush bRect, Brush bLeft, Brush bMid, Brush bRight, Pen pen)
        {
            double tlX = centerX - topWidth / 2.0;
            double tlY = centerY - height / 2.0;
            double trX = centerX + topWidth / 2.0;
            double trY = centerY - height / 2.0;
            double blX = centerX - bottomWidth / 2.0;
            double blY = centerY + height / 2.0;
            double brX = centerX + bottomWidth / 2.0;
            double brY = centerY + height / 2.0;

            double left = centerX - topWidth / 2.0;
            double top = centerY - height / 2.0;

            g.FillRectangle(bRect,
                (float)left, (float)top,
                (float)topWidth, (float)height);

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

            g.DrawLine(pen, (float)blX, (float)blY, (float)blX, (float)tlY);
            g.DrawLine(pen, (float)brX, (float)brY, (float)brX, (float)trY);
        }

        private void DrawTrapezoidShape(
            Graphics g,
            double centerX, double centerY,
            double topWidth, double bottomWidth, double height)
        {
            Color fillRect = Color.LightGray;
            Color leftColor = Color.LightBlue;
            Color midColor = Color.LightGreen;
            Color rightColor = Color.LightPink;

            using (var bRect = new SolidBrush(fillRect))
            using (var bLeft = new SolidBrush(leftColor))
            using (var bMid = new SolidBrush(midColor))
            using (var bRight = new SolidBrush(rightColor))
            using (var pen = new Pen(Color.Black, 2))
            {
                DrawTrapezoidCore(
                    g, centerX, centerY, topWidth, bottomWidth, height,
                    bRect, bLeft, bMid, bRight, pen);
            }
        }

        private void EraseTrapezoid(
            Graphics g,
            double centerX, double centerY,
            double topWidth, double bottomWidth, double height)
        {
            if (_stretchedBackground != null)
            {
                using (var brushFill = new TextureBrush(_stretchedBackground, WrapMode.Tile))
                using (var brushPen = new TextureBrush(_stretchedBackground, WrapMode.Tile))
                using (var pen = new Pen(brushPen, 2 + ERASE_PEN_EXTRA))
                {
                    DrawTrapezoidCore(
                        g, centerX, centerY, topWidth, bottomWidth, height,
                        brushFill, brushFill, brushFill, brushFill, pen);
                }
            }
            else
            {
                using (var brushFill = new SolidBrush(Color.White))
                using (var pen = new Pen(Color.White, 2 + ERASE_PEN_EXTRA))
                {
                    DrawTrapezoidCore(
                        g, centerX, centerY, topWidth, bottomWidth, height,
                        brushFill, brushFill, brushFill, brushFill, pen);
                }
            }
        }

        private PointF RotatePoint(float x, float y, float cx, float cy, double angle)
        {
            double cosA = Math.Cos(angle);
            double sinA = Math.Sin(angle);
            float dx = x - cx;
            float dy = y - cy;
            float rotatedX = (float)(dx * cosA - dy * sinA + cx);
            float rotatedY = (float)(dx * sinA + dy * cosA + cy);
            return new PointF(rotatedX, rotatedY);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _timer?.Stop();
            _timer?.Dispose();
            _bufferGraphics?.Dispose();
            _buffer?.Dispose();
            _stretchedBackground?.Dispose();
            _backgroundImage?.Dispose();

            base.OnFormClosed(e);
        }
    }
}