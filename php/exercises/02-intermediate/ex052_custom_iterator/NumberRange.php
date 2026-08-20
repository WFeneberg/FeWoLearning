<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex052CustomIterator;

/*
Exercise 052 - Custom iterator (intermediate).

Goal:   Implement an ascending integer range that iterates via the Iterator interface directly.
Drills: Iterator interface, current/key/next/valid/rewind, iterator state, re-iterating an instance.
Passes: NumberRangeTest
*/
final class NumberRange implements \Iterator
{
    private int $current;

    public function __construct(private readonly int $start, private readonly int $end)
    {
        $this->current = $start;
    }

    public function current(): int
    {
        throw new \RuntimeException('TODO');
    }

    public function key(): int
    {
        throw new \RuntimeException('TODO');
    }

    public function next(): void
    {
        throw new \RuntimeException('TODO');
    }

    public function valid(): bool
    {
        throw new \RuntimeException('TODO');
    }

    public function rewind(): void
    {
        throw new \RuntimeException('TODO');
    }
}
