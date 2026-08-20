<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex052CustomIterator;

final class NumberRange implements \Iterator
{
    private int $current;

    public function __construct(private readonly int $start, private readonly int $end)
    {
        $this->current = $start;
    }

    public function current(): int
    {
        return $this->current;
    }

    public function key(): int
    {
        return $this->current - $this->start;
    }

    public function next(): void
    {
        $this->current++;
    }

    public function valid(): bool
    {
        return $this->current <= $this->end;
    }

    public function rewind(): void
    {
        $this->current = $this->start;
    }
}
