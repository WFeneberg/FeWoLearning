<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex052CustomIterator;

require_once __DIR__ . '/NumberRange.php';

use PHPUnit\Framework\TestCase;

final class NumberRangeTest extends TestCase
{
    public function testForeachCollectsKeysAndValues(): void
    {
        $range = new NumberRange(5, 8);

        $keys = [];
        $values = [];
        foreach ($range as $key => $value) {
            $keys[] = $key;
            $values[] = $value;
        }

        self::assertSame([0, 1, 2, 3], $keys);
        self::assertSame([5, 6, 7, 8], $values);
    }

    public function testSingleValueRange(): void
    {
        $range = new NumberRange(3, 3);

        $values = [];
        foreach ($range as $value) {
            $values[] = $value;
        }

        self::assertSame([3], $values);
    }

    public function testIteratingSameInstanceTwiceResetsState(): void
    {
        $range = new NumberRange(1, 3);

        $first = [];
        foreach ($range as $value) {
            $first[] = $value;
        }

        $second = [];
        foreach ($range as $value) {
            $second[] = $value;
        }

        self::assertSame([1, 2, 3], $first);
        self::assertSame($first, $second);
    }

    public function testValidBecomesFalsePastEnd(): void
    {
        $range = new NumberRange(0, 1);
        $range->rewind();

        self::assertTrue($range->valid());
        $range->next();
        self::assertTrue($range->valid());
        $range->next();
        self::assertFalse($range->valid());
    }
}
