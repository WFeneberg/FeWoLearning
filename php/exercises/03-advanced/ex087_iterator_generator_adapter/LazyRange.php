<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex087IteratorGeneratorAdapter;

/*
Exercise 087 - Iterator/generator adapter (advanced).

Goal:   Implement IteratorAggregate whose getIterator() is itself a generator
        method, combining foreach-ability with generator laziness.
Drills: IteratorAggregate, generators, yield, lazy iteration.
Passes: LazyRangeTest
*/
final class LazyRange implements \IteratorAggregate
{
    public function __construct(
        private readonly int $start,
        private readonly int $end,
    ) {
    }

    public function getIterator(): \Generator
    {
        throw new \RuntimeException('TODO');
    }
}
