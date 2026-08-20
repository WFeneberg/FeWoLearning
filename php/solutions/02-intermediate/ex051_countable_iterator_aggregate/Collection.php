<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex051CountableIteratorAggregate;

final class Collection implements \Countable, \IteratorAggregate
{
    private array $items;

    public function __construct(array $items)
    {
        $this->items = $items;
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
