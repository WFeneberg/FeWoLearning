<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex044ReadonlyProperties;

require_once __DIR__ . '/Point.php';

use PHPUnit\Framework\TestCase;

final class PointTest extends TestCase
{
    public function testWithXReturnsNewInstanceWithUpdatedX(): void
    {
        $point = new Point(1.0, 2.0);
        $moved = $point->withX(9.0);

        self::assertNotSame($point, $moved);
        self::assertSame(9.0, $moved->x);
        self::assertSame(2.0, $moved->y);
    }

    public function testWithYReturnsNewInstanceWithUpdatedY(): void
    {
        $point = new Point(1.0, 2.0);
        $moved = $point->withY(9.0);

        self::assertNotSame($point, $moved);
        self::assertSame(1.0, $moved->x);
        self::assertSame(9.0, $moved->y);
    }

    public function testWithXLeavesOriginalUntouched(): void
    {
        $point = new Point(1.0, 2.0);
        $point->withX(9.0);

        self::assertSame(1.0, $point->x);
        self::assertSame(2.0, $point->y);
    }

    public function testDirectMutationOfReadonlyPropertyThrows(): void
    {
        $point = new Point(1.0, 2.0);

        $this->expectException(\Error::class);

        $point->x = 5.0;
    }

    public function testWithYOnAlreadyModifiedPointChainsCorrectly(): void
    {
        $point = new Point(1.0, 2.0);
        $result = $point->withX(3.0)->withY(4.0);

        self::assertSame(3.0, $result->x);
        self::assertSame(4.0, $result->y);
    }
}
