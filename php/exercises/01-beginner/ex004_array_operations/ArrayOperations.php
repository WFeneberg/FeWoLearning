<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex004ArrayOperations;

/*
Exercise 004 - Array operations (beginner).

Goal:   Implement helpers over PHP's classic stack/merge/sort array functions.
Drills: array_push/pop/shift/unshift, array_merge, array_unique, sort.
Passes: ArrayOperationsTest
*/
final class ArrayOperations
{
    private function __construct()
    {
    }

    public static function pushThenPop(array $items, mixed $newItem): array
    {
        throw new \RuntimeException('TODO');
    }

    public static function mergeUnique(array $a, array $b): array
    {
        throw new \RuntimeException('TODO');
    }

    public static function sortAscending(array $numbers): array
    {
        throw new \RuntimeException('TODO');
    }
}
