<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex069UnionIntersectionTypes;

require_once __DIR__ . '/TypeDemos.php';

use PHPUnit\Framework\TestCase;

final class CountableIterableFixture implements \Countable, \IteratorAggregate
{
    public function __construct(private readonly array $items)
    {
    }

    public function count(): int
    {
        return count($this->items);
    }

    public function getIterator(): \Iterator
    {
        return new \ArrayIterator($this->items);
    }
}

final class TypeDemosTest extends TestCase
{
    public function testNormalizeIdFromInt(): void
    {
        self::assertSame('42', TypeDemos::normalizeId(42));
    }

    public function testNormalizeIdFromNumericStringStripsLeadingZeros(): void
    {
        self::assertSame('42', TypeDemos::normalizeId('00042'));
    }

    public function testNormalizeIdFromAllZerosStaysZero(): void
    {
        self::assertSame('0', TypeDemos::normalizeId('000'));
    }

    public function testCombinedLengthCountsItems(): void
    {
        $collection = new CountableIterableFixture(['a', 'b', 'c']);

        self::assertSame(3, TypeDemos::combinedLength($collection));
    }

    public function testCombinedLengthOnEmptyCollection(): void
    {
        $collection = new CountableIterableFixture([]);

        self::assertSame(0, TypeDemos::combinedLength($collection));
    }
}
