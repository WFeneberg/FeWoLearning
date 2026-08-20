<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex007ArrayMapFilter;

final class ArrayMapFilter
{
    private function __construct()
    {
    }

    public static function doubleAll(array $numbers): array
    {
        return array_map(fn ($n) => $n * 2, $numbers);
    }

    public static function filterEven(array $numbers): array
    {
        return array_values(array_filter($numbers, fn ($n) => $n % 2 === 0));
    }

    public static function sumWithReduce(array $numbers): int|float
    {
        return array_reduce($numbers, fn ($carry, $n) => $carry + $n, 0);
    }
}
