using Lab1_OZR1.Geometry;

namespace Lab1_OZR1.Movement
{
    /// <summary>
    /// Движущаяся точка.
    /// </summary>
    public class MovingPoint
    {
        public Point2D Position { get; private set; }

        public Vector2D Velocity { get; private set; }

        public MovingPoint(
            Point2D position,
            Vector2D velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        public void SetVelocity(Vector2D velocity)
        {
            Velocity = velocity;
        }

        /// <summary>
        /// Выполнение одного шага движения.
        /// </summary>
        public void Move()
        {
            Position = Position + Velocity;
        }

        /// <summary>
        /// Выполнение нескольких шагов движения.
        /// </summary>
        public void Move(int steps)
        {
            if (steps < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(steps));
            }

            for (int i = 0; i < steps; i++)
            {
                Move();
            }
        }

        public bool IsInside(Rectangle2D area)
        {
            return area.Contains(Position);
        }
    }
}