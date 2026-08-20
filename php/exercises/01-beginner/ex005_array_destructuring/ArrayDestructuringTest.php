<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex005ArrayDestructuring;

require_once __DIR__ . '/ArrayDestructuring.php';

use PHPUnit\Framework\TestCase;

final class ArrayDestructuringTest extends TestCase
{
    public function testSwapPairSwapsTheTwoValues(): void
    {
        self::assertSame([2, 1], ArrayDestructuring::swapPair([1, 2]));
    }

    public function testSwapPairWithStrings(): void
    {
        self::assertSame(['b', 'a'], ArrayDestructuring::swapPair(['a', 'b']));
    }

    public function testExtractCoordinatesReturnsPlainListInXYZOrder(): void
    {
        $result = ArrayDestructuring::extractCoordinates(['x' => 1.0, 'y' => 2.0, 'z' => 3.0]);

        self::assertSame([1.0, 2.0, 3.0], $result);
    }

    public function testExtractCoordinatesIgnoresKeyOrderInInput(): void
    {
        $result = ArrayDestructuring::extractCoordinates(['z' => 9.0, 'x' => 7.0, 'y' => 8.0]);

        self::assertSame([7.0, 8.0, 9.0], $result);
    }

    public function testFirstAndRestSplitsFirstElementFromRemainder(): void
    {
        $result = ArrayDestructuring::firstAndRest([10, 20, 30]);

        self::assertSame(10, $result['first']);
        self::assertSame([20, 30], $result['rest']);
    }

    public function testFirstAndRestWithSingleElementLeavesEmptyRest(): void
    {
        $result = ArrayDestructuring::firstAndRest(['only']);

        self::assertSame('only', $result['first']);
        self::assertSame([], $result['rest']);
    }
}
