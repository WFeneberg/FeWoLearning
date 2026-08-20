<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex039YieldFromFlatten;

/*
Exercise 039 - yield from flattening (intermediate).

Goal:   Recursively flatten an arbitrarily nested array into a flat sequence of scalars.
Drills: yield from, recursive flattening of arbitrarily nested arrays.
Passes: FlattenerTest
*/
final class Flattener
{
    private function __construct()
    {
    }

    public static function flatten(array $nested): \Generator
    {
        throw new \RuntimeException('TODO');
    }
}
