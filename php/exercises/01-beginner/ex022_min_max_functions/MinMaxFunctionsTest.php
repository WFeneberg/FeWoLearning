<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex022MinMaxFunctions;

require_once __DIR__ . '/MinMaxFunctions.php';

use PHPUnit\Framework\TestCase;

final class MinMaxFunctionsTest extends TestCase
{
    public function testRangeSpanWithMixedIntsAndFloats(): void
    {
        self::assertEqualsWithDelta(11.0, MinMaxFunctions::rangeSpan([3, 1.5, 10, -1, 7]), 0.0001);
    }

    public function testRangeSpanSingleElementIsZero(): void
    {
        self::assertEqualsWithDelta(0.0, MinMaxFunctions::rangeSpan([42]), 0.0001);
    }

    public function testRangeSpanThrowsOnEmpty(): void
    {
        $this->expectException(\InvalidArgumentException::class);
        MinMaxFunctions::rangeSpan([]);
    }

    public function testAverage(): void
    {
        self::assertEqualsWithDelta(4.0, MinMaxFunctions::average([2, 4, 6]), 0.0001);
        self::assertEqualsWithDelta(2.5, MinMaxFunctions::average([1, 2, 3, 4]), 0.0001);
    }

    public function testAverageThrowsOnEmpty(): void
    {
        $this->expectException(\InvalidArgumentException::class);
        MinMaxFunctions::average([]);
    }

    public function testProduct(): void
    {
        self::assertEquals(24, MinMaxFunctions::product([2, 3, 4]));
        self::assertEqualsWithDelta(7.5, MinMaxFunctions::product([2.5, 3]), 0.0001);
    }
}
