using Lab1_OZR1.Editor;
using Lab1_OZR1.Geometry;
using Lab1_OZR1.Movement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Lab1_OZR1
{
    public partial class Form1 : Form
    {
        private Scene _scene;

        private Timer _timer;

        private double _angle;
        private double _paintedAngle;

        private Bitmap _buffer;
        private Graphics _bufferGraphics;

        /// <summary>
        /// Признак необходимости полной перерисовки.
        /// Устанавливается при структурных изменениях сцены
        /// (создание, удаление, смена порядка, правка свойств).
        /// В остальных кадрах используется контурная очистка.
        /// </summary>
        private bool _fullRedraw = true;

        private const int RINGS = 17;

        public Form1()
        {
            InitializeComponent();

            this.ClientSize = new Size(1200, 800);
            this.Text = "ЛР1 — КВ2 — универсальная фигура + контурная очистка";

            _scene = new Scene(
                new Rectangle2D(0, 0, panelScene.Width, panelScene.Height));

            _buffer = new Bitmap(panelScene.Width, panelScene.Height);
            _bufferGraphics = Graphics.FromImage(_buffer);
            ClearBuffer();

            _timer = new Timer();
            _timer.Interval = 16;
            _timer.Tick += Timer_Tick;
            _timer.Start();

            panelScene.Paint += PanelScene_Paint;
            panelScene.MouseClick += PanelScene_MouseClick;
            panelScene.Resize += PanelScene_Resize;

            lstShapes.SelectedIndexChanged += LstShapes_SelectedIndexChanged;

            btnCreateShape.Click += BtnCreateShape_Click;
            btnDelete.Click += BtnDelete_Click;
            btnOrderUp.Click += BtnOrderUp_Click;
            btnOrderDown.Click += BtnOrderDown_Click;

            propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
        }

        // ============================================================
        // БУФЕР
        // ============================================================

        private void ClearBuffer()
        {
            _bufferGraphics.Clear(Color.White);
        }

        private void PanelScene_Resize(object sender, EventArgs e)
        {
            if (panelScene.Width <= 0 || panelScene.Height <= 0)
            {
                return;
            }

            _scene.Area = new Rectangle2D(
                0, 0, panelScene.Width, panelScene.Height);

            _bufferGraphics?.Dispose();
            _buffer?.Dispose();

            _buffer = new Bitmap(panelScene.Width, panelScene.Height);
            _bufferGraphics = Graphics.FromImage(_buffer);
            ClearBuffer();

            _fullRedraw = true;

            panelScene.Invalidate();
        }

        // ============================================================
        // ЦИКЛ
        // ============================================================

        private void Timer_Tick(object sender, EventArgs e)
        {
            _scene.Update();

            _angle += 0.02;

            panelScene.Invalidate();
        }

        // ============================================================
        // ОТРИСОВКА (КОНТУРНАЯ ОЧИСТКА)
        // ============================================================

        private void PanelScene_Paint(object sender, PaintEventArgs e)
        {
            if (_fullRedraw)
            {
                ClearBuffer();
            }
            else
            {
                // 1. Стереть все фигуры на предыдущих позициях.
                foreach (MovingShape shape in _scene.Ordered())
                {
                    shape.DrawAt(
                        _bufferGraphics,
                        shape.PrevCenterX,
                        shape.PrevCenterY,
                        erase: true);
                }

                // 2. Стереть фон при старом угле.
                DrawBackground(_bufferGraphics, _paintedAngle, erase: true);
            }

            // 3. Рисуем фон при новом угле.
            DrawBackground(_bufferGraphics, _angle, erase: false);

            // 4. Рисуем все фигуры в порядке Order.
            foreach (MovingShape shape in _scene.Ordered())
            {
                shape.DrawAt(
                    _bufferGraphics,
                    shape.CenterX,
                    shape.CenterY,
                    erase: false);
            }

            _paintedAngle = _angle;
            _fullRedraw = false;

            // 5. Копируем буфер на экран.
            e.Graphics.DrawImageUnscaled(_buffer, 0, 0);
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
            Color penColor = erase ? Color.White : Color.Black;
            int penWidth = erase ? 4 : 2;

            using (Pen pen = new Pen(penColor, penWidth))
            {
                int cx = panelScene.Width / 2;
                int cy = panelScene.Height / 2;

                int s = 50;

                for (int colRomb = 0; colRomb < RINGS; colRomb++)
                {
                    PointF[] romb = new PointF[4];
                    romb[0] = new PointF(cx, cy - s);
                    romb[1] = new PointF(cx + s, cy);
                    romb[2] = new PointF(cx, cy + s);
                    romb[3] = new PointF(cx - s, cy);

                    double a = angle + colRomb * 3;

                    for (int i = 0; i < 4; i++)
                    {
                        romb[i] = RotatePoint(romb[i].X, romb[i].Y, cx, cy, a);
                    }

                    s += 20;

                    g.DrawPolygon(pen, romb);
                }
            }
        }

        // ============================================================
        // ВЗАИМОДЕЙСТВИЕ
        // ============================================================

        private void PanelScene_MouseClick(object sender, MouseEventArgs e)
        {
            Point2D point = new Point2D(e.X, e.Y);

            MovingShape shape = _scene.GetShapeAt(point);

            if (shape != null)
            {
                lstShapes.SelectedItem = shape;
            }
        }

        private void LstShapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = lstShapes.SelectedItem as MovingShape;
        }

        private void PropertyGrid_PropertyValueChanged(
            object sender,
            PropertyValueChangedEventArgs e)
        {
            _fullRedraw = true;

            RefreshShapeList();

            panelScene.Invalidate();
        }

        private void BtnCreateShape_Click(object sender, EventArgs e)
        {
            using (PolygonEditorForm editor = new PolygonEditorForm())
            {
                if (editor.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                IReadOnlyList<Point2D> vertices = editor.Vertices;

                double cx = 0.0;
                double cy = 0.0;

                foreach (Point2D v in vertices)
                {
                    cx += v.X;
                    cy += v.Y;
                }

                cx /= vertices.Count;
                cy /= vertices.Count;

                MovingCustomPolygon shape = new MovingCustomPolygon(
                    cx, cy, vertices, new Vector2D(2, 2));

                // Помещаем фигуру в центр сцены, сохраняя форму.
                shape.CenterX = panelScene.Width / 2.0;
                shape.CenterY = panelScene.Height / 2.0;
                shape.PrevCenterX = shape.CenterX;
                shape.PrevCenterY = shape.CenterY;

                shape.Color = Color.LightSkyBlue;

                _scene.AddShape(shape);

                _fullRedraw = true;

                RefreshShapeList();
                lstShapes.SelectedItem = shape;
                panelScene.Invalidate();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            MovingShape shape = lstShapes.SelectedItem as MovingShape;

            if (shape == null)
            {
                return;
            }

            _scene.RemoveShape(shape);

            _fullRedraw = true;

            propertyGrid.SelectedObject = null;

            RefreshShapeList();
            panelScene.Invalidate();
        }

        private void BtnOrderUp_Click(object sender, EventArgs e)
        {
            MovingShape shape = lstShapes.SelectedItem as MovingShape;

            if (shape == null)
            {
                return;
            }

            _scene.MoveOrderUp(shape);

            _fullRedraw = true;

            RefreshShapeList();
            lstShapes.SelectedItem = shape;
            panelScene.Invalidate();
        }

        private void BtnOrderDown_Click(object sender, EventArgs e)
        {
            MovingShape shape = lstShapes.SelectedItem as MovingShape;

            if (shape == null)
            {
                return;
            }

            _scene.MoveOrderDown(shape);

            _fullRedraw = true;

            RefreshShapeList();
            lstShapes.SelectedItem = shape;
            panelScene.Invalidate();
        }

        private void RefreshShapeList()
        {
            object selected = lstShapes.SelectedItem;

            lstShapes.Items.Clear();

            foreach (MovingShape shape in _scene.Ordered())
            {
                lstShapes.Items.Add(shape);
            }

            if (selected != null && lstShapes.Items.Contains(selected))
            {
                lstShapes.SelectedItem = selected;
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