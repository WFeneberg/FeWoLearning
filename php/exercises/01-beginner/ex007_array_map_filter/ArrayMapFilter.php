<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex007ArrayMapFilter;

/*
Exercise 007 - array_map / array_filter / array_reduce (beginner).

Goal:   Implement transform/filter/aggregate helpers using PHP's functional array builtins.
Drills: array_map, array_filter (with re-indexing), array_reduce.
Passes: ArrayMapFilterTest
*/
final class ArrayMapFilter
{
    private function __construct()
    {
    }

    public static function doubleAll(array $numbers): array
    {
        throw new \RuntimeException('TODO');
    }

    public static function filterEven(array $numbers): array
    {
        throw new \RuntimeException('TODO');
    }

    public static function sumWithReduce(array $numbers): int|float
    {
        throw new \RuntimeException('TODO');
    }
}
