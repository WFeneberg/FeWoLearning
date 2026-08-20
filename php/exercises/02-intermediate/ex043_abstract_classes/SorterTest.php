<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex043AbstractClasses;

require_once __DIR__ . '/Sorter.php';

use PHPUnit\Framework\TestCase;

final class SorterTest extends TestCase
{
    public function testSortsAscending(): void
    {
        $sorter = new NumericAscendingSorter();

        self::assertSame([1, 2, 3], $sorter->sort([3, 1, 2]));
    }

    public function testSortsAlreadySortedList(): void
    {
        $sorter = new NumericAscendingSorter();

        self::assertSame([1, 2, 3, 4], $sorter->sort([1, 2, 3, 4]));
    }

    public function testSortsWithDuplicates(): void
    {
        $sorter = new NumericAscendingSorter();

        self::assertSame([1, 1, 2, 5], $sorter->sort([5, 1, 2, 1]));
    }

    public function testSortDoesNotMutateOriginalArray(): void
    {
        $sorter = new NumericAscendingSorter();
        $original = [3, 1, 2];
        $sorter->sort($original);

        self::assertSame([3, 1, 2], $original);
    }

    public function testSortOnSingleElementArray(): void
    {
        $sorter = new NumericAscendingSorter();

        self::assertSame([42], $sorter->sort([42]));
    }
}
