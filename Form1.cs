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
        private double _prevRotationAngle;
        private double _prevScale;

        private Timer _timer;
        private double _angle;
        private double _paintedAngle;
        private Bitmap _buffer;
        private Graphics _bufferGraphics;

        private const int RINGS = 12;
        private const int PER_RING = 8;
        private const double PI = Math.PI;

        public Form1()
        {
            InitializeComponent();

            this.ClientSize = new Size(900, 700);
            this.Text = "ЛР1 — КВ2 — контурная очистка";

            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

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
            _prevRotationAngle = _trapezoid.RotationAngle;
            _prevScale = _trapezoid.Scale;

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
            _prevRotationAngle = _trapezoid.RotationAngle;
            _prevScale = _trapezoid.Scale;

            _trapezoid.Update(_area);

            _angle += 0.02;

            Invalidate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            const double angleStep = PI / 36.0;
            const double scaleStep = 0.05;
            const double velStep = 1.0;

            if (e.KeyCode == Keys.Q)
                _trapezoid.RotationAngle -= angleStep;
            else if (e.KeyCode == Keys.E)
                _trapezoid.RotationAngle += angleStep;
            else if (e.KeyCode == Keys.A)
                _trapezoid.Scale -= scaleStep;
            else if (e.KeyCode == Keys.D)
                _trapezoid.Scale += scaleStep;
            else if (e.KeyCode == Keys.W)
                ApplyVelocity(_trapezoid.Velocity.X + velStep, _trapezoid.Velocity.Y);
            else if (e.KeyCode == Keys.S)
                ApplyVelocity(_trapezoid.Velocity.X - velStep, _trapezoid.Velocity.Y);
            else if (e.KeyCode == Keys.Up)
                ApplyVelocity(_trapezoid.Velocity.X, _trapezoid.Velocity.Y - velStep);
            else if (e.KeyCode == Keys.Down)
                ApplyVelocity(_trapezoid.Velocity.X, _trapezoid.Velocity.Y + velStep);

            Invalidate();
        }

        private void ApplyVelocity(double vx, double vy)
        {
            const double minAbs = 0.1;

            if (Math.Abs(vx) < minAbs) vx = vx >= 0 ? minAbs : -minAbs;
            if (Math.Abs(vy) < minAbs) vy = vy >= 0 ? minAbs : -minAbs;

            _trapezoid.SetVelocity(new Vector2D(vx, vy));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawTrapezoidShape(
                _bufferGraphics,
                _prevCenterX, _prevCenterY,
                _trapezoid.TopWidth, _trapezoid.BottomWidth, _trapezoid.Height,
                _prevRotationAngle, _prevScale,
                erase: true);

            DrawBackground(_bufferGraphics, _paintedAngle, erase: true);

            DrawBackground(_bufferGraphics, _angle, erase: false);

            DrawTrapezoidShape(
                _bufferGraphics,
                _trapezoid.CenterX, _trapezoid.CenterY,
                _trapezoid.TopWidth, _trapezoid.BottomWidth, _trapezoid.Height,
                _trapezoid.RotationAngle, _trapezoid.Scale,
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

        private PointF TransformPoint(
            double x, double y,
            double cx, double cy,
            double rotationAngle, double scale)
        {
            double sx = cx + (x - cx) * scale;
            double sy = cy + (y - cy) * scale;

            double dx = sx - cx;
            double dy = sy - cy;
            double cos = Math.Cos(rotationAngle);
            double sin = Math.Sin(rotationAngle);

            return new PointF(
                (float)(cx + dx * cos - dy * sin),
                (float)(cy + dx * sin + dy * cos));
        }

        private void DrawTrapezoidShape(
            Graphics g,
            double centerX, double centerY,
            double topWidth, double bottomWidth, double height,
            double rotationAngle, double scale,
            bool erase)
        {
            double tlX = centerX - topWidth / 2.0;
            double tlY = centerY - height / 2.0;
            double trX = centerX + topWidth / 2.0;
            double trY = centerY - height / 2.0;
            double blX = centerX - bottomWidth / 2.0;
            double blY = centerY + height / 2.0;
            double brX = centerX + bottomWidth / 2.0;
            double brY = centerY + height / 2.0;

            double w = topWidth * scale;
            double h = height * scale;
            double a = Math.Max(w, 2.0 * h / Math.Sqrt(3.0));
            double hTri = a * Math.Sqrt(3.0) / 2.0;

            PointF[] trianglePts =
            {
                new PointF((float)centerX, (float)(centerY - 2.0 * hTri / 3.0)),
                new PointF((float)(centerX - a / 2.0), (float)(centerY + hTri / 3.0)),
                new PointF((float)(centerX + a / 2.0), (float)(centerY + hTri / 3.0))
            };

            PointF tl = TransformPoint(tlX, tlY, centerX, centerY, rotationAngle, scale);
            PointF tr = TransformPoint(trX, trY, centerX, centerY, rotationAngle, scale);
            PointF bl = TransformPoint(blX, blY, centerX, centerY, rotationAngle, scale);
            PointF br = TransformPoint(brX, brY, centerX, centerY, rotationAngle, scale);

            PointF blTop = TransformPoint(blX, tlY, centerX, centerY, rotationAngle, scale);
            PointF brTop = TransformPoint(brX, trY, centerX, centerY, rotationAngle, scale);

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

                PointF[] leftPts = { tl, bl, blTop };
                g.FillPolygon(bLeft, leftPts);

                PointF[] midPts = { blTop, brTop, br, bl };
                g.FillPolygon(bMid, midPts);

                PointF[] rightPts = { tr, br, brTop };
                g.FillPolygon(bRight, rightPts);

                PointF[] trapezoidPts = { tl, tr, br, bl };
                g.DrawPolygon(pen, trapezoidPts);

                g.DrawLine(pen, bl, blTop);
                g.DrawLine(pen, br, brTop);
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