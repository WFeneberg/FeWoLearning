<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex023MathFunctions;

require_once __DIR__ . '/MathFunctions.php';

use PHPUnit\Framework\TestCase;

final class MathFunctionsTest extends TestCase
{
    public function testRoundHalfUp(): void
    {
        self::assertEqualsWithDelta(2.68, MathFunctions::roundHalfUp(2.675, 2), 0.0001);
        self::assertEqualsWithDelta(3.0, MathFunctions::roundHalfUp(2.5, 0), 0.0001);
    }

    public function testFloorToIntOnNegativeNumber(): void
    {
        self::assertSame(-4, MathFunctions::floorToInt(-3.2));
        self::assertSame(3, MathFunctions::floorToInt(3.9));
    }

    public function testCeilToIntOnNegativeNumber(): void
    {
        self::assertSame(-3, MathFunctions::ceilToInt(-3.2));
        self::assertSame(4, MathFunctions::ceilToInt(3.1));
    }

    public function testIntegerDivideTruncatesTowardZeroOnNegativeDividend(): void
    {
        self::assertSame(-3, MathFunctions::integerDivide(-7, 2));
        self::assertSame(3, MathFunctions::integerDivide(7, 2));
    }

    public function testAbsoluteDifference(): void
    {
        self::assertEquals(5, MathFunctions::absoluteDifference(3, 8));
        self::assertEquals(5, MathFunctions::absoluteDifference(8, 3));
        self::assertEqualsWithDelta(2.5, MathFunctions::absoluteDifference(1.5, -1.0), 0.0001);
    }

    public function testPower(): void
    {
        self::assertEquals(8, MathFunctions::power(2, 3));
        self::assertEqualsWithDelta(0.25, MathFunctions::power(2, -2), 0.0001);
    }
}
