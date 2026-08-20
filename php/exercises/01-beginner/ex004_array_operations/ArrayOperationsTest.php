<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex004ArrayOperations;

require_once __DIR__ . '/ArrayOperations.php';

use PHPUnit\Framework\TestCase;

final class ArrayOperationsTest extends TestCase
{
    public function testPushThenPopReturnsStateAfterPushAndThePoppedValue(): void
    {
        [$afterPush, $popped] = ArrayOperations::pushThenPop([1, 2, 3], 4);

        self::assertSame([1, 2, 3, 4], $afterPush);
        self::assertSame(4, $popped);
    }

    public function testPushThenPopDoesNotMutateOriginalArray(): void
    {
        $original = [1, 2, 3];
        ArrayOperations::pushThenPop($original, 99);

        self::assertSame([1, 2, 3], $original);
    }

    public function testMergeUniqueDedupesAndReindexes(): void
    {
        $result = ArrayOperations::mergeUnique([1, 2, 3], [3, 4, 2]);

        self::assertSame([1, 2, 3, 4], $result);
        self::assertSame(array_values($result), $result);
    }

    public function testSortAscendingReturnsSortedCopy(): void
    {
        $original = [3, 1, 2];
        $sorted = ArrayOperations::sortAscending($original);

        self::assertSame([1, 2, 3], $sorted);
        self::assertSame([3, 1, 2], $original);
    }
}
