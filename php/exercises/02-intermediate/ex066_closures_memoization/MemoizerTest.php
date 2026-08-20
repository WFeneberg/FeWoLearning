<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex066ClosuresMemoization;

require_once __DIR__ . '/Memoizer.php';

use PHPUnit\Framework\TestCase;

final class MemoizerTest extends TestCase
{
    public function testMemoizesRepeatedCallWithSameArguments(): void
    {
        $callCount = 0;
        $fn = function (int $x) use (&$callCount): int {
            $callCount++;

            return $x * 2;
        };

        $memoized = Memoizer::memoize($fn);

        self::assertSame(10, $memoized(5));
        self::assertSame(10, $memoized(5));
        self::assertSame(1, $callCount);
    }

    public function testRecomputesForDifferentArguments(): void
    {
        $callCount = 0;
        $fn = function (int $x) use (&$callCount): int {
            $callCount++;

            return $x * 2;
        };

        $memoized = Memoizer::memoize($fn);

        self::assertSame(10, $memoized(5));
        self::assertSame(20, $memoized(10));
        self::assertSame(2, $callCount);
    }

    public function testMemoizesAcrossMultipleArguments(): void
    {
        $callCount = 0;
        $fn = function (int $a, int $b) use (&$callCount): int {
            $callCount++;

            return $a + $b;
        };

        $memoized = Memoizer::memoize($fn);

        self::assertSame(7, $memoized(3, 4));
        self::assertSame(7, $memoized(3, 4));
        self::assertSame(1, $callCount);
    }

    public function testReturnsAClosure(): void
    {
        $memoized = Memoizer::memoize(fn (int $x): int => $x);

        self::assertInstanceOf(\Closure::class, $memoized);
    }
}
