<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex006AssociativeArrays;

/*
Exercise 006 - Associative arrays (beginner).

Goal:   Implement helpers that distinguish a missing key from a key mapped to null.
Drills: isset() vs array_key_exists(), the ?? operator.
Passes: AssociativeArraysTest
*/
final class AssociativeArrays
{
    private function __construct()
    {
    }

    public static function getOrDefault(array $map, string $key, mixed $default): mixed
    {
        throw new \RuntimeException('TODO');
    }

    public static function hasKeyEvenIfNull(array $map, string $key): bool
    {
        throw new \RuntimeException('TODO');
    }

    public static function hasNonNullValue(array $map, string $key): bool
    {
        throw new \RuntimeException('TODO');
    }
}
