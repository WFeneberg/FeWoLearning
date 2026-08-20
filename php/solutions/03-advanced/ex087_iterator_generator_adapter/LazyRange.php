<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex087IteratorGeneratorAdapter;

final class LazyRange implements \IteratorAggregate
{
    public function __construct(
        private readonly int $start,
        private readonly int $end,
    ) {
    }

    public function getIterator(): \Generator
    {
        for ($i = $this->start; $i <= $this->end; $i++) {
            yield $i;
        }
    }
}
