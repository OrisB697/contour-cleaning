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

            TestHorizontalBounce();

            TestVerticalBounce();

            TestEnergyConservation();

            TestSpeedCap();

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
                    new Vector2D(5, -2),
                    mass: 1.0,
                    gravity: 0.2,
                    restitution: 1.0
                );

            trapezoid.Move();

            AssertEqual(
                trapezoid.CenterX,
                105,
                "Движение трапеции по X"
            );

            // Было vy = -2, после гравитации стало -1.8
            AssertEqual(
                trapezoid.CenterY,
                98.2,
                "Движение трапеции по Y с учётом гравитации"
            );

            Console.WriteLine("[OK] Движение трапеции с гравитацией");
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

            AssertEqual(bounds.Left, 80, "Левая граница");
            AssertEqual(bounds.Right, 120, "Правая граница");
            AssertEqual(bounds.Top, 85, "Верхняя граница");
            AssertEqual(bounds.Bottom, 115, "Нижняя граница");

            Console.WriteLine("[OK] Габаритный прямоугольник");
        }

        private static void TestHorizontalBounce()
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
                    new Vector2D(-20, 0),
                    mass: 1.0,
                    gravity: 0.0,
                    restitution: 1.0
                );

            trapezoid.Update(area);

            AssertEqual(
                trapezoid.Velocity.X,
                20,
                "Скорость X после упругого отскока"
            );

            if (trapezoid.BoundingRectangle.Left < area.Left)
            {
                throw new Exception(
                    "Фигура осталась за левой границей после отскока."
                );
            }

            Console.WriteLine("[OK] Отскок от вертикальной границы");
        }

        private static void TestVerticalBounce()
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
                    new Vector2D(0, -20),
                    mass: 1.0,
                    gravity: 0.0,
                    restitution: 1.0
                );

            trapezoid.Update(area);

            AssertEqual(
                trapezoid.Velocity.Y,
                20,
                "Скорость Y после упругого отскока"
            );

            if (trapezoid.BoundingRectangle.Top < area.Top)
            {
                throw new Exception(
                    "Фигура осталась за верхней границей после отскока."
                );
            }

            Console.WriteLine("[OK] Отскок от горизонтальной границы");
        }

        /// <summary>
        /// Проверяет, что при упругом отскоке и гравитации
        /// модуль скорости фигуры не растёт от удара к удару.
        /// </summary>
        private static void TestEnergyConservation()
        {
            Rectangle2D area =
                new Rectangle2D(0, 0, 200, 200);

            MovingTrapezoid trapezoid =
                new MovingTrapezoid(
                    100,
                    100,
                    40,
                    20,
                    20,
                    new Vector2D(3, 0),
                    mass: 1.0,
                    gravity: 0.2,
                    restitution: 1.0,
                    maxSpeed: 1000.0   // отключаем страховку для чистоты теста
                );

            double initialSpeed = trapezoid.Velocity.Length;
            double maxObservedSpeed = initialSpeed;

            // Прогоняем достаточно шагов, чтобы фигура
            // несколько раз ударилась о стены.
            for (int i = 0; i < 2000; i++)
            {
                trapezoid.Update(area);

                double speed = trapezoid.Velocity.Length;

                if (speed > maxObservedSpeed)
                {
                    maxObservedSpeed = speed;
                }
            }

            // Скорость может незначительно колебаться из-за
            // дискретности, но не должна вырасти, скажем, в 5 раз.
            if (maxObservedSpeed > initialSpeed * 5.0)
            {
                throw new Exception(
                    $"Скорость выросла с {initialSpeed} до {maxObservedSpeed}. " +
                    "Отскок не компенсируется гравитацией."
                );
            }

            Console.WriteLine(
                "[OK] Энергия сохраняется (скорость не растёт)"
            );
        }

        /// <summary>
        /// Проверяет, что жёсткий предел модуля скорости работает.
        /// </summary>
        private static void TestSpeedCap()
        {
            Rectangle2D area =
                new Rectangle2D(0, 0, 100, 100);

            MovingTrapezoid trapezoid =
                new MovingTrapezoid(
                    50,
                    50,
                    20,
                    10,
                    20,
                    new Vector2D(500, 500),
                    mass: 1.0,
                    gravity: 0.0,
                    restitution: 1.0,
                    maxSpeed: 10.0
                );

            trapezoid.Update(area);

            if (trapezoid.Velocity.Length > 10.0 + 1e-6)
            {
                throw new Exception(
                    "Жёсткий предел скорости не сработал."
                );
            }

            Console.WriteLine("[OK] Жёсткий предел скорости");
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