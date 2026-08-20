<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex007ArrayMapFilter;

require_once __DIR__ . '/ArrayMapFilter.php';

use PHPUnit\Framework\TestCase;

final class ArrayMapFilterTest extends TestCase
{
    public function testDoubleAllDoublesEveryElement(): void
    {
        self::assertSame([2, 4, 6], ArrayMapFilter::doubleAll([1, 2, 3]));
    }

    public function testDoubleAllOnEmptyArray(): void
    {
        self::assertSame([], ArrayMapFilter::doubleAll([]));
    }

    public function testFilterEvenKeepsOnlyEvenNumbers(): void
    {
        self::assertSame([2, 4], ArrayMapFilter::filterEven([1, 2, 3, 4, 5]));
    }

    public function testFilterEvenReindexesResultSequentially(): void
    {
        $result = ArrayMapFilter::filterEven([1, 2, 3, 4]);

        self::assertSame(array_values($result), $result);
        self::assertSame([0, 1], array_keys($result));
    }

    public function testSumWithReduceSumsAllElements(): void
    {
        self::assertSame(15, ArrayMapFilter::sumWithReduce([1, 2, 3, 4, 5]));
    }

    public function testSumWithReduceOnEmptyArrayIsZero(): void
    {
        self::assertSame(0, ArrayMapFilter::sumWithReduce([]));
    }
}
