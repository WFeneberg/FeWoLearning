<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex008ForeachKeyValue;

require_once __DIR__ . '/ForeachKeyValue.php';

use PHPUnit\Framework\TestCase;

final class ForeachKeyValueTest extends TestCase
{
    public function testInvertMapSwapsKeysAndValues(): void
    {
        $result = ForeachKeyValue::invertMap(['a' => 1, 'b' => 2]);

        self::assertSame([1 => 'a', 2 => 'b'], $result);
    }

    public function testInvertMapOnEmptyMap(): void
    {
        self::assertSame([], ForeachKeyValue::invertMap([]));
    }

    public function testIncrementAllByReferenceMutatesEveryElementInPlace(): void
    {
        $numbers = [1, 2, 3];
        ForeachKeyValue::incrementAllByReference($numbers);

        self::assertSame([2, 3, 4], $numbers);
    }

    public function testIncrementAllByReferenceOnEmptyArray(): void
    {
        $numbers = [];
        ForeachKeyValue::incrementAllByReference($numbers);

        self::assertSame([], $numbers);
    }

    public function testIncrementAllByReferenceCalledTwiceOnDifferentLengthArraysDoesNotCorruptEither(): void
    {
        // Regression test for the classic PHP by-reference foreach bug: forgetting
        // `unset($n)` after `foreach ($numbers as &$n)` leaves $n as a dangling
        // reference to the last element, which can silently corrupt later usage.
        $shorter = [5, 6];
        ForeachKeyValue::incrementAllByReference($shorter);
        self::assertSame([6, 7], $shorter);

        $longer = [1, 2, 3, 4];
        ForeachKeyValue::incrementAllByReference($longer);
        self::assertSame([2, 3, 4, 5], $longer);

        // The shorter array must remain untouched by the second call.
        self::assertSame([6, 7], $shorter);
    }
}
