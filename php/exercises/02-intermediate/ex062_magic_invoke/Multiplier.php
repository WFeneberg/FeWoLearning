<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex062MagicInvoke;

/*
Exercise 062 - Magic __invoke (intermediate).

Goal:   Make an object callable like a function via __invoke.
Drills: __invoke, is_callable, array_map with a callable object.
Passes: MultiplierTest
*/
final class Multiplier
{
    public function __construct(private readonly int $factor)
    {
    }

    public function __invoke(int $n): int
    {
        throw new \RuntimeException('TODO');
    }
}
