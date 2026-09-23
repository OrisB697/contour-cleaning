using System;
using Lab1_OZR1.Geometry;
using Lab1_OZR1.Movement;

namespace Lab1_OZR1.Tests
{
    public static class MovementTests
    {
        public static void RunAll()
        {
            TestPointMovement();

            TestTrapezoidMovement();

            TestBoundingRectangle();

            TestHorizontalWrap();

            TestVerticalWrap();

            TestVelocityPreservation();

            Console.WriteLine();
            Console.WriteLine("Все тесты успешно пройдены.");
        }

        private static void TestPointMovement()
        {
            MovingPoint point = new MovingPoint(
                new Point2D(10, 20),
                new Vector2D(3, 4)
            );

            point.Move();

            AssertEqual(
                point.Position.X,
                13,
                "Движение точки по X"
            );

            AssertEqual(
                point.Position.Y,
                24,
                "Движение точки по Y"
            );

            Console.WriteLine("[OK] Движение точки");
        }

        private static void TestTrapezoidMovement()
        {
            MovingTrapezoid trapezoid =
                new MovingTrapezoid(
                    100,
                    100,
                    40,
                    20,
                    30,
                    new Vector2D(5, -2)
                );

            trapezoid.Move();

            AssertEqual(
                trapezoid.CenterX,
                105,
                "Движение трапеции по X"
            );

            AssertEqual(
                trapezoid.CenterY,
                98,
                "Движение трапеции по Y"
            );

            Console.WriteLine("[OK] Движение трапеции");
        }

        private static void TestBoundingRectangle()
        {
            MovingTrapezoid trapezoid =
                new MovingTrapezoid(
                    100,
                    100,
                    40,
                    20,
                    30,
                    new Vector2D(0, 0)
                );

            Rectangle2D bounds =
                trapezoid.BoundingRectangle;

            AssertEqual(
                bounds.Left,
                80,
                "Левая граница"
            );

            AssertEqual(
                bounds.Right,
                120,
                "Правая граница"
            );

            AssertEqual(
                bounds.Top,
                85,
                "Верхняя граница"
            );

            AssertEqual(
                bounds.Bottom,
                115,
                "Нижняя граница"
            );

            Console.WriteLine(
                "[OK] Габаритный прямоугольник"
            );
        }

        private static void TestHorizontalWrap()
        {
            Rectangle2D area =
                new Rectangle2D(0, 0, 100, 100);

            MovingTrapezoid trapezoid =
                new MovingTrapezoid(
                    10,
                    50,
                    20,
                    10,
                    20,
                    new Vector2D(-20, 0)
                );

            trapezoid.Update(area);

            Rectangle2D bounds =
                trapezoid.BoundingRectangle;

            if (bounds.Right < area.Left)
            {
                throw new Exception(
                    "Фигура не была перенесена через левую границу."
                );
            }

            Console.WriteLine(
                "[OK] Перенос через горизонтальную границу"
            );
        }

        private static void TestVerticalWrap()
        {
            Rectangle2D area =
                new Rectangle2D(0, 0, 100, 100);

            MovingTrapezoid trapezoid =
                new MovingTrapezoid(
                    50,
                    10,
                    20,
                    10,
                    20,
                    new Vector2D(0, -20)
                );

            trapezoid.Update(area);

            Rectangle2D bounds =
                trapezoid.BoundingRectangle;

            if (bounds.Bottom < area.Top)
            {
                throw new Exception(
                    "Фигура не была перенесена через верхнюю границу."
                );
            }

            Console.WriteLine(
                "[OK] Перенос через вертикальную границу"
            );
        }

        private static void TestVelocityPreservation()
        {
            Vector2D velocity =
                new Vector2D(-5, 3);

            Rectangle2D area =
                new Rectangle2D(0, 0, 100, 100);

            MovingTrapezoid trapezoid =
                new MovingTrapezoid(
                    10,
                    50,
                    20,
                    10,
                    20,
                    velocity
                );

            trapezoid.Update(area);

            AssertEqual(
                trapezoid.Velocity.X,
                velocity.X,
                "Сохранение скорости X"
            );

            AssertEqual(
                trapezoid.Velocity.Y,
                velocity.Y,
                "Сохранение скорости Y"
            );

            Console.WriteLine(
                "[OK] Сохранение направления и скорости"
            );
        }

        private static void AssertEqual(
            double actual,
            double expected,
            string message)
        {
            const double epsilon = 0.000001;

            if (Math.Abs(actual - expected) > epsilon)
            {
                throw new Exception(
                    $"{message}: ожидалось {expected}, " +
                    $"получено {actual}"
                );
            }
        }
    }
}