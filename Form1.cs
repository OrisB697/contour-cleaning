using System;
using System.Drawing;
using System.Windows.Forms;
using Lab1_OZR1.Geometry;
using Lab1_OZR1.Movement;

namespace Lab1_OZR1
{
    public partial class Form1 : Form
    {
        // ------------------------------------------------------------
        // Логика движения (часть ОЗ.Р1 — взято из C#-проекта)
        // ------------------------------------------------------------
        private MovingTrapezoid _trapezoid;
        private Rectangle2D _area;

        // Предыдущие координаты центра — нужны для контурной очистки.
        private double _prevCenterX;
        private double _prevCenterY;

        // ------------------------------------------------------------
        // Отрисовка (часть ОЗ.Р2)
        // ------------------------------------------------------------
        private Timer _timer;
        private double _angle;         // текущий угол поворота фона
        private double _paintedAngle;  // угол, уже отрисованный на холсте
        private Bitmap _buffer;        // задний буфер (двойная буферизация)
        private Graphics _bufferGraphics;

        private const int RINGS = 12;
        private const int PER_RING = 8;
        private const double PI = Math.PI;

        public Form1()
        {
            InitializeComponent();

            this.ClientSize = new Size(900, 700);
            this.Text = "ЛР1 — КВ2 — контурная очистка";

            // Двойная буферизация самого окна, чтобы не мерцало
            this.DoubleBuffered = true;

            // Область движения
            _area = new Rectangle2D(0, 0, ClientSize.Width, ClientSize.Height);

            // Трапеция по КВ2: перевёрнутая, верхнее основание шире нижнего
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

            // Буфер
            _buffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            _bufferGraphics = Graphics.FromImage(_buffer);
            ClearBuffer();

            // Таймер ~60 FPS
            _timer = new Timer();
            _timer.Interval = 16;
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // При изменении размера — пересоздаём буфер и область
            this.Resize += Form1_Resize;
        }

        // ------------------------------------------------------------
        // Служебное
        // ------------------------------------------------------------

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

        // ------------------------------------------------------------
        // Шаг анимации
        // ------------------------------------------------------------

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Запоминаем старую позицию ДО движения
            _prevCenterX = _trapezoid.CenterX;
            _prevCenterY = _trapezoid.CenterY;

            // Перемещение + перенос через границы (логика ОЗ.Р1)
            _trapezoid.Update(_area);

            // Поворот фона
            _angle += 0.02;

            Invalidate();
        }

        // ------------------------------------------------------------
        // Контурная очистка
        // ------------------------------------------------------------
        //
        // Принцип: фон целиком НЕ перерисовываем. Стираем только
        // движущиеся элементы (ромбы фона + трапецию) в СТАРЫХ позициях
        // белым по контуру, затем рисуем их в НОВЫХ.
        //
        // Порядок строго такой:
        //   1. стереть СТАРУЮ трапецию
        //   2. стереть СТАРЫЙ фон
        //   3. нарисовать НОВЫЙ фон
        //   4. нарисовать НОВУЮ трапецию
        //
        // Если стереть трапецию после того, как нарисован новый фон,
        // белый прямоугольник затрёт часть ромбов — будут артефакты.

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 1. Стираем СТАРУЮ трапецию — по СТАРЫМ координатам
            DrawTrapezoidShape(
                _bufferGraphics,
                _prevCenterX, _prevCenterY,
                _trapezoid.TopWidth, _trapezoid.BottomWidth, _trapezoid.Height,
                erase: true);

            // 2. Стираем СТАРЫЙ фон — по старому углу
            DrawBackground(_bufferGraphics, _paintedAngle, erase: true);

            // 3. Рисуем НОВЫЙ фон
            DrawBackground(_bufferGraphics, _angle, erase: false);

            // 4. Рисуем НОВУЮ трапецию — по новым координатам
            DrawTrapezoidShape(
                _bufferGraphics,
                _trapezoid.CenterX, _trapezoid.CenterY,
                _trapezoid.TopWidth, _trapezoid.BottomWidth, _trapezoid.Height,
                erase: false);

            // 5. Копируем задний буфер на экран
            e.Graphics.DrawImageUnscaled(_buffer, 0, 0);

            _paintedAngle = _angle;
        }

        // ------------------------------------------------------------
        // Отрисовка фона: ромбы, радиальный ломаный узор
        // ------------------------------------------------------------

        private void DrawBackground(Graphics g, double angle, bool erase)
        {
            Color fillColor = Color.White;
            Color penColor = erase ? Color.White : Color.Black;
            int penWidth = erase ? 4 : 2;   // при стирании перо толще,
                                            // чтобы перекрыть старый контур

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
                        g.DrawPolygon(pen, pts);   // <-- теперь всегда, и при erase тоже
                    }
                }
            }
        }

        // ------------------------------------------------------------
        // Отрисовка трапеции по КВ2
        // ------------------------------------------------------------
        //
        // Фигура:
        //   - закрашенный прямоугольник вокруг трапеции (без контура);
        //   - три цветные части: левая, средняя, правая;
        //   - контур: стороны трапеции + две высоты (вертикали из нижних
        //     углов к верхнему основанию).
        //
        // Координаты центра передаются явно, чтобы можно было стереть
        // фигуру в старой позиции (её уже нет в объекте _trapezoid).

        private void DrawTrapezoidShape(
    Graphics g,
    double centerX, double centerY,
    double topWidth, double bottomWidth, double height,
    bool erase)
        {
            // Вершины
            double tlX = centerX - topWidth / 2.0;
            double tlY = centerY - height / 2.0;
            double trX = centerX + topWidth / 2.0;
            double trY = centerY - height / 2.0;
            double blX = centerX - bottomWidth / 2.0;
            double blY = centerY + height / 2.0;
            double brX = centerX + bottomWidth / 2.0;
            double brY = centerY + height / 2.0;

            // Цвета и толщина пера
            Color fillRect = Color.White;
            Color leftColor = Color.White;
            Color midColor = Color.White;
            Color rightColor = Color.White;
            Color outline = erase ? Color.White : Color.Black;
            int penWidth = erase ? 4 : 2;   // при стирании перо толще,
                                            // чтобы перекрыть старый контур

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
                // 1. Габаритный прямоугольник вокруг трапеции
                double left = centerX - topWidth / 2.0;
                double top = centerY - height / 2.0;
                double width = topWidth;
                double rectH = height;

                g.FillRectangle(bRect,
                    (float)left, (float)top,
                    (float)width, (float)rectH);

                // 2. Левая часть
                PointF[] leftPts =
                {
            new PointF((float)tlX, (float)tlY),
            new PointF((float)blX, (float)blY),
            new PointF((float)blX, (float)tlY)
        };
                g.FillPolygon(bLeft, leftPts);

                // 3. Средняя часть
                PointF[] midPts =
                {
            new PointF((float)blX, (float)tlY),
            new PointF((float)brX, (float)tlY),
            new PointF((float)brX, (float)blY),
            new PointF((float)blX, (float)blY)
        };
                g.FillPolygon(bMid, midPts);

                // 4. Правая часть
                PointF[] rightPts =
                {
            new PointF((float)trX, (float)trY),
            new PointF((float)brX, (float)brY),
            new PointF((float)brX, (float)trY)
        };
                g.FillPolygon(bRight, rightPts);

                // 5. Контур трапеции — ТЕПЕРЬ ВСЕГДА, и при erase тоже
                PointF[] trapezoidPts =
                {
            new PointF((float)tlX, (float)tlY),
            new PointF((float)trX, (float)trY),
            new PointF((float)brX, (float)brY),
            new PointF((float)blX, (float)blY)
        };
                g.DrawPolygon(pen, trapezoidPts);

                // 6. Две высоты — тоже ВСЕГДА
                g.DrawLine(pen, (float)blX, (float)blY, (float)blX, (float)tlY);
                g.DrawLine(pen, (float)brX, (float)brY, (float)brX, (float)trY);
            }
        }

        // ------------------------------------------------------------
        // Освобождение ресурсов
        // ------------------------------------------------------------

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