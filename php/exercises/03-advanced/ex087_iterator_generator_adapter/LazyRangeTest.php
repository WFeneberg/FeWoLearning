<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex087IteratorGeneratorAdapter;

require_once __DIR__ . '/LazyRange.php';

use PHPUnit\Framework\TestCase;

final class LazyRangeTest extends TestCase
{
    public function testForeachCollectsExpectedValues(): void
    {
        $range = new LazyRange(1, 5);

        $collected = [];
        foreach ($range as $n) {
            $collected[] = $n;
        }

        self::assertSame([1, 2, 3, 4, 5], $collected);
    }

    public function testSingleValueRange(): void
    {
        $range = new LazyRange(7, 7);

        $collected = [];
        foreach ($range as $n) {
            $collected[] = $n;
        }

        self::assertSame([7], $collected);
    }

    public function testGetIteratorReturnsGenerator(): void
    {
        $range = new LazyRange(0, 2);

        self::assertInstanceOf(\Generator::class, $range->getIterator());
    }

    public function testHugeRangeIsLazyAndBreakingEarlyIsFast(): void
    {
        $range = new LazyRange(0, 1_000_000);

        $firstValue = null;
        $iterations = 0;

        foreach ($range as $n) {
            $firstValue = $n;
            $iterations++;
            break;
        }

        self::assertSame(0, $firstValue);
        self::assertSame(1, $iterations);
    }
}
