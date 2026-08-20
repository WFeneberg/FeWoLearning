<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex027RangeStep;

require_once __DIR__ . '/RangeStep.php';

use PHPUnit\Framework\TestCase;

final class RangeStepTest extends TestCase
{
    public function testAscendingRange(): void
    {
        self::assertSame([2, 4, 6, 8], RangeStep::rangeWithStep(2, 8, 2));
    }

    public function testDescendingRange(): void
    {
        self::assertSame([10, 8, 6, 4], RangeStep::rangeWithStep(10, 4, 2));
    }

    public function testSingleValueRangeWhenStartEqualsEnd(): void
    {
        self::assertSame([5], RangeStep::rangeWithStep(5, 5, 1));
    }

    public function testReverseRange(): void
    {
        self::assertSame([3, 2, 1], RangeStep::reverseRange([1, 2, 3]));
    }

    public function testReverseRangeOnEmptyArray(): void
    {
        self::assertSame([], RangeStep::reverseRange([]));
    }
}
