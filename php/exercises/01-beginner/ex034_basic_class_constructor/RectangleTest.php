<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex034BasicClassConstructor;

require_once __DIR__ . '/Rectangle.php';

use PHPUnit\Framework\TestCase;

final class RectangleTest extends TestCase
{
    public function testArea(): void
    {
        $rectangle = new Rectangle(4.0, 5.0);

        self::assertEqualsWithDelta(20.0, $rectangle->area(), 0.0001);
    }

    public function testPerimeter(): void
    {
        $rectangle = new Rectangle(4.0, 5.0);

        self::assertEqualsWithDelta(18.0, $rectangle->perimeter(), 0.0001);
    }

    public function testSquareIsSpecialCaseOfRectangle(): void
    {
        $square = new Rectangle(3.0, 3.0);

        self::assertEqualsWithDelta(9.0, $square->area(), 0.0001);
        self::assertEqualsWithDelta(12.0, $square->perimeter(), 0.0001);
    }

    public function testNegativeWidthThrows(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        new Rectangle(-1.0, 5.0);
    }

    public function testNegativeHeightThrows(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        new Rectangle(5.0, -1.0);
    }
}
