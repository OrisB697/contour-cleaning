using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Lab1_OZR1.Geometry;

namespace Lab1_OZR1.Editor
{
    /// <summary>
    /// Модальное окно для рисования произвольной замкнутой фигуры
    /// из отрезков с помощью мыши.
    ///
    /// ЛКМ — добавить вершину.
    /// Backspace — удалить последнюю вершину.
    /// Enter или ПКМ — замкнуть фигуру.
    /// Esc — отмена.
    /// </summary>
    public class PolygonEditorForm : Form
    {
        private readonly List<PointF> _points = new List<PointF>();
        private PointF _cursor;
        private bool _hasCursor;

        public IReadOnlyList<Point2D> Vertices
        {
            get
            {
                List<Point2D> result = new List<Point2D>(_points.Count);

                foreach (PointF p in _points)
                {
                    result.Add(new Point2D(p.X, p.Y));
                }

                return result;
            }
        }

        public PolygonEditorForm()
        {
            this.Text = "Рисование фигуры: ЛКМ — вершина, ПКМ/Enter — замкнуть, Esc — отмена";
            this.ClientSize = new Size(700, 600);
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.KeyPreview = true;

            this.MouseClick += Editor_MouseClick;
            this.MouseMove += Editor_MouseMove;
            this.KeyDown += Editor_KeyDown;
            this.Paint += Editor_Paint;
        }

        private void Editor_MouseMove(object sender, MouseEventArgs e)
        {
            _cursor = new PointF(e.X, e.Y);
            _hasCursor = true;
            Invalidate();
        }

        private void Editor_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _points.Add(new PointF(e.X, e.Y));
                Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                Finish();
            }
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Finish();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else if (e.KeyCode == Keys.Back && _points.Count > 0)
            {
                _points.RemoveAt(_points.Count - 1);
                Invalidate();
            }
        }

        private void Finish()
        {
            if (_points.Count < 3)
            {
                MessageBox.Show(
                    "Нужно минимум 3 вершины.",
                    "Фигура не задана",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Editor_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen gridPen = new Pen(Color.Gainsboro))
            {
                for (int x = 0; x < ClientSize.Width; x += 50)
                    g.DrawLine(gridPen, x, 0, x, ClientSize.Height);

                for (int y = 0; y < ClientSize.Height; y += 50)
                    g.DrawLine(gridPen, 0, y, ClientSize.Width, y);
            }

            if (_points.Count == 0)
            {
                return;
            }

            using (Pen pen = new Pen(Color.Black, 2))
            {
                for (int i = 0; i < _points.Count - 1; i++)
                {
                    g.DrawLine(pen, _points[i], _points[i + 1]);
                }

                if (_hasCursor)
                {
                    g.DrawLine(pen, _points[_points.Count - 1], _cursor);
                }

                if (_points.Count >= 3 && _hasCursor)
                {
                    using (Pen closePen = new Pen(Color.Gray, 1))
                    {
                        closePen.DashStyle = DashStyle.Dash;
                        g.DrawLine(closePen, _cursor, _points[0]);
                    }
                }
            }

            using (Brush brush = new SolidBrush(Color.Red))
            {
                foreach (PointF p in _points)
                {
                    g.FillEllipse(brush, p.X - 3, p.Y - 3, 6, 6);
                }
            }
        }
    }
}