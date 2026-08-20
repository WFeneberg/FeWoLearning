<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex012NullCoalescing;

/*
Exercise 012 - Null coalescing (beginner).

Goal:   Implement helpers using the null-coalescing operator and its assignment form.
Drills: ?? null-coalescing operator, ??= null-coalescing assignment.
Passes: NullCoalescingTest
*/
final class NullCoalescing
{
    private function __construct()
    {
    }

    public static function firstNonNull(mixed ...$values): mixed
    {
        throw new \RuntimeException('TODO');
    }

    public static function configWithDefault(array &$config, string $key, mixed $default): mixed
    {
        throw new \RuntimeException('TODO');
    }
}
