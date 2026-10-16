using System.Collections.Generic;
using System.Linq;
using Lab1_OZR1.Geometry;

namespace Lab1_OZR1.Movement
{
    /// <summary>
    /// Сцена, содержащая набор движущихся фигур.
    /// </summary>
    public class Scene
    {
        private readonly List<MovingShape> _shapes =
            new List<MovingShape>();

        public Rectangle2D Area { get; set; }

        public IReadOnlyList<MovingShape> Shapes
        {
            get { return _shapes; }
        }

        public Scene(Rectangle2D area)
        {
            Area = area;
        }

        public void AddShape(MovingShape shape)
        {
            if (_shapes.Count == 0)
            {
                shape.Order = 0;
            }
            else
            {
                shape.Order = _shapes.Max(s => s.Order) + 1;
            }

            _shapes.Add(shape);
        }

        public void RemoveShape(MovingShape shape)
        {
            _shapes.Remove(shape);
        }

        public void Update()
        {
            foreach (MovingShape shape in _shapes)
            {
                shape.Update(Area);
            }
        }

        public List<MovingShape> Ordered()
        {
            return _shapes.OrderBy(s => s.Order).ToList();
        }

        public MovingShape GetShapeAt(Point2D point)
        {
            foreach (MovingShape shape in _shapes.OrderByDescending(s => s.Order))
            {
                if (shape.BoundingRectangle.Contains(point))
                {
                    return shape;
                }
            }

            return null;
        }

        public void MoveOrderUp(MovingShape shape)
        {
            List<MovingShape> sorted = Ordered();

            int index = sorted.IndexOf(shape);

            if (index < 0 || index >= sorted.Count - 1)
            {
                return;
            }

            MovingShape next = sorted[index + 1];

            int tmp = shape.Order;
            shape.Order = next.Order;
            next.Order = tmp;
        }

        public void MoveOrderDown(MovingShape shape)
        {
            List<MovingShape> sorted = Ordered();

            int index = sorted.IndexOf(shape);

            if (index <= 0)
            {
                return;
            }

            MovingShape prev = sorted[index - 1];

            int tmp = shape.Order;
            shape.Order = prev.Order;
            prev.Order = tmp;
        }
    }
}