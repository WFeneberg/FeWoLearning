<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex089AbstractBaseContract;

require_once __DIR__ . '/Shape2.php';

use PHPUnit\Framework\TestCase;

final class Shape2Test extends TestCase
{
    public function testDescribeMatchesExpectedFormattingForEachShape(): void
    {
        $shapes = [
            new Square2(4.0),
            new Circle2(3.0),
        ];

        $expectedAreas = [16.0, M_PI * 9.0];
        $expectedPerimeters = [16.0, 2 * M_PI * 3.0];

        foreach ($shapes as $index => $shape) {
            $expected = sprintf('area=%.2f perimeter=%.2f', $expectedAreas[$index], $expectedPerimeters[$index]);

            self::assertSame($expected, $shape->describe());
        }
    }

    public function testSquareAreaAndPerimeter(): void
    {
        $square = new Square2(5.0);

        self::assertEqualsWithDelta(25.0, $square->area(), 0.0001);
        self::assertEqualsWithDelta(20.0, $square->perimeter(), 0.0001);
    }

    public function testCircleAreaAndPerimeter(): void
    {
        $circle = new Circle2(2.0);

        self::assertEqualsWithDelta(M_PI * 4.0, $circle->area(), 0.0001);
        self::assertEqualsWithDelta(2 * M_PI * 2.0, $circle->perimeter(), 0.0001);
    }

    public function testBothShapesAreInstancesOfShape2(): void
    {
        self::assertInstanceOf(Shape2::class, new Square2(1.0));
        self::assertInstanceOf(Shape2::class, new Circle2(1.0));
    }
}
