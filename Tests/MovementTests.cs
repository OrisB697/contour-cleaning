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

            TestCircumscribedCircle();

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

        private static void TestCircumscribedCircle()
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

            Circle2D circle = trapezoid.CircumscribedCircle;

            // R = sqrt(20^2 + 15^2) = 25
            AssertEqual(
                circle.CenterX,
                100,
                "Центр окружности X"
            );

            AssertEqual(
                circle.CenterY,
                100,
                "Центр окружности Y"
            );

            AssertEqual(
                circle.Radius,
                25,
                "Радиус описанной окружности"
            );

            AssertEqual(
                circle.Left,
                75,
                "Левая точка окружности"
            );

            AssertEqual(
                circle.Right,
                125,
                "Правая точка окружности"
            );

            AssertEqual(
                circle.Top,
                75,
                "Верхняя точка окружности"
            );

            AssertEqual(
                circle.Bottom,
                125,
                "Нижняя точка окружности"
            );

            Console.WriteLine(
                "[OK] Описанная окружность"
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

            Circle2D circle = trapezoid.CircumscribedCircle;

            if (circle.Right < area.Left)
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

            Circle2D circle = trapezoid.CircumscribedCircle;

            if (circle.Bottom < area.Top)
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