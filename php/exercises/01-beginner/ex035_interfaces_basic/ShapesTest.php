<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex035InterfacesBasic;

require_once __DIR__ . '/Shapes.php';

use PHPUnit\Framework\TestCase;

final class ShapesTest extends TestCase
{
    public function testCircleArea(): void
    {
        $circle = new Circle(2.0);

        self::assertEqualsWithDelta(M_PI * 4.0, $circle->area(), 0.0001);
    }

    public function testSquareArea(): void
    {
        $square = new Square(3.0);

        self::assertEqualsWithDelta(9.0, $square->area(), 0.0001);
    }

    public function testTotalAreaSumsPolymorphically(): void
    {
        $shapes = [
            new Circle(2.0),
            new Square(3.0),
        ];

        $expected = (M_PI * 4.0) + 9.0;

        self::assertEqualsWithDelta($expected, ShapeMath::totalArea($shapes), 0.0001);
    }

    public function testTotalAreaOfEmptyListIsZero(): void
    {
        self::assertEqualsWithDelta(0.0, ShapeMath::totalArea([]), 0.0001);
    }

    public function testTotalAreaOfMultipleCircles(): void
    {
        $shapes = [new Circle(1.0), new Circle(1.0), new Circle(1.0)];

        self::assertEqualsWithDelta(3.0 * M_PI, ShapeMath::totalArea($shapes), 0.0001);
    }
}
