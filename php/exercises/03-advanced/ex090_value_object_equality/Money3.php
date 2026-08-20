<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex090ValueObjectEquality;

/*
Exercise 090 - Value object equality (advanced).

Goal:   Implement an immutable value object with an explicit equals() method
        alongside PHP's structural (==) comparison, and an immutable add().
Drills: readonly properties, value objects, structural vs identity equality.
Passes: Money3Test
*/
final class Money3
{
    public function __construct(
        public readonly int $cents,
        public readonly string $currency,
    ) {
    }

    public function equals(self $other): bool
    {
        throw new \RuntimeException('TODO');
    }

    public function add(self $other): self
    {
        throw new \RuntimeException('TODO');
    }
}
