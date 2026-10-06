using Lab1_OZR1.Geometry;
using Lab1_OZR1.Movement;
using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Windows.Forms;

namespace Lab1_OZR1
{
    public partial class Form1 : Form
    {

        private MovingTrapezoid _trapezoid;
        private Rectangle2D _area;

        private double _prevAngle;

        private double _prevCenterX;
        private double _prevCenterY;

        private Timer _timer;
        private double _angle;         // текущий угол поворота фона
        private double _paintedAngle;  // угол отрисованный
        private Bitmap _buffer;        // задний буфер
        private Graphics _bufferGraphics;

        private const int RINGS = 17;
        private const int PER_RING = 8;
        private const double PI = Math.PI;

        public Form1()
        {
            InitializeComponent();

            this.ClientSize = new Size(900, 700);
            this.Text = "ЛР1 — КВ2 — контурная очистка";

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
                velocity: new Vector2D(3, 2),
                angularVelocity: 0.03   // радиан за шаг
            );

            _prevCenterX = _trapezoid.CenterX;
            _prevCenterY = _trapezoid.CenterY;
            _prevAngle = _trapezoid.RotationAngle;

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
            _prevAngle = _trapezoid.RotationAngle;

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
                _prevAngle,
                erase: true
            );

            DrawBackground(_bufferGraphics, _paintedAngle, erase: true);
            DrawBackground(_bufferGraphics, _angle, erase: false);

            DrawTrapezoidShape(
                _bufferGraphics,
                _trapezoid.CenterX, _trapezoid.CenterY,
                _trapezoid.TopWidth, _trapezoid.BottomWidth, _trapezoid.Height,
                _trapezoid.RotationAngle,
                erase: false);

            // 5. Копируем задний буфер на экран
            e.Graphics.DrawImageUnscaled(_buffer, 0, 0);

            _paintedAngle = _angle;
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

        private void DrawBackground(Graphics g, double angle, bool erase)
        {
            Color fillColor = Color.White;
            Color penColor = erase ? Color.White : Color.Black;
            int penWidth = erase ? 4 : 2;   // Делаем толще чтобы точно все затереть

            using (Brush brush = new SolidBrush(fillColor))
            using (Pen pen = new Pen(penColor, penWidth))
            {
                int cx = ClientSize.Width / 2;
                int cy = ClientSize.Height / 2;

                int s = 50;

                for(int colRomb = 0; colRomb < RINGS; colRomb++)
                {
                    PointF[] romb = new PointF[4];
                    romb[0] = new PointF((float)(cx), (float)(cy - s));
                    romb[1] = new PointF((float)(cx + s), (float)(cy));
                    romb[2] = new PointF((float)(cx), (float)(cy + s));
                    romb[3] = new PointF((float)(cx - s), (float)(cy));

                    romb[0] = RotatePoint(romb[0].X, romb[0].Y, cx, cy, angle + colRomb * 3);
                    romb[1] = RotatePoint(romb[1].X, romb[1].Y, cx, cy, angle + colRomb * 3);
                    romb[2] = RotatePoint(romb[2].X, romb[2].Y, cx, cy, angle + colRomb * 3);
                    romb[3] = RotatePoint(romb[3].X, romb[3].Y, cx, cy, angle + colRomb * 3);

                    s += 20;

                    g.DrawPolygon(pen, romb);
                }
            }
        }

        private void DrawTrapezoidShape
(
    Graphics g,
    double centerX, double centerY,
    double topWidth, double bottomWidth, double height,
    double rotationAngle,
    bool erase
)
        {
            float cx = (float)centerX;
            float cy = (float)centerY;

            double left = centerX - topWidth / 2.0;
            double right = centerX + topWidth / 2.0;
            double top = centerY - height / 2.0;
            double bottom = centerY + height / 2.0;
            double blX = centerX - bottomWidth / 2.0;
            double brX = centerX + bottomWidth / 2.0;

            // Вершины трапеции
            PointF tl = RotatePoint((float)left, (float)top, cx, cy, rotationAngle);
            PointF tr = RotatePoint((float)right, (float)top, cx, cy, rotationAngle);
            PointF bl = RotatePoint((float)blX, (float)bottom, cx, cy, rotationAngle);
            PointF br = RotatePoint((float)brX, (float)bottom, cx, cy, rotationAngle);

            // Вспомогательные точки
            PointF blTop = RotatePoint((float)blX, (float)top, cx, cy, rotationAngle);
            PointF brTop = RotatePoint((float)brX, (float)top, cx, cy, rotationAngle);

            // Локальный габаритный прямоугольник
            PointF rectTl = RotatePoint((float)left, (float)top, cx, cy, rotationAngle);
            PointF rectTr = RotatePoint((float)right, (float)top, cx, cy, rotationAngle);
            PointF rectBr = RotatePoint((float)right, (float)bottom, cx, cy, rotationAngle);
            PointF rectBl = RotatePoint((float)left, (float)bottom, cx, cy, rotationAngle);

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
                PointF[] rectPts = { rectTl, rectTr, rectBr, rectBl };
                g.FillPolygon(bRect, rectPts);

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