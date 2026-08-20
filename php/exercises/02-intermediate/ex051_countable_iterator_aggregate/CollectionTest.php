<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex051CountableIteratorAggregate;

require_once __DIR__ . '/Collection.php';

use PHPUnit\Framework\TestCase;

final class CollectionTest extends TestCase
{
    public function testCountReturnsNumberOfItems(): void
    {
        $collection = new Collection(['a', 'b', 'c']);

        self::assertSame(3, count($collection));
    }

    public function testCountOnEmptyCollection(): void
    {
        $collection = new Collection([]);

        self::assertSame(0, count($collection));
    }

    public function testForeachIteratesAllItemsInOrder(): void
    {
        $collection = new Collection(['x', 'y', 'z']);

        $seen = [];
        foreach ($collection as $item) {
            $seen[] = $item;
        }

        self::assertSame(['x', 'y', 'z'], $seen);
    }

    public function testGetIteratorReturnsIteratorInstance(): void
    {
        $collection = new Collection([1, 2]);

        self::assertInstanceOf(\Iterator::class, $collection->getIterator());
    }

    public function testCollectionImplementsExpectedInterfaces(): void
    {
        $collection = new Collection([1]);

        self::assertInstanceOf(\Countable::class, $collection);
        self::assertInstanceOf(\IteratorAggregate::class, $collection);
    }
}
