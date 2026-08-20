<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex051CountableIteratorAggregate;

/*
Exercise 051 - Countable and IteratorAggregate (intermediate).

Goal:   Make a Collection object work with PHP's built-in count() and foreach
        by implementing the Countable and IteratorAggregate interfaces.
Drills: Countable interface, IteratorAggregate, ArrayIterator delegation.
Passes: CollectionTest
*/
final class Collection implements \Countable, \IteratorAggregate
{
    private array $items;

    public function __construct(array $items)
    {
        $this->items = $items;
    }

    public function count(): int
    {
        throw new \RuntimeException('TODO');
    }

    public function getIterator(): \Iterator
    {
        throw new \RuntimeException('TODO');
    }
}
