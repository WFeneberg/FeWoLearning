<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex004ArrayOperations;

final class ArrayOperations
{
    private function __construct()
    {
    }

    public static function pushThenPop(array $items, mixed $newItem): array
    {
        $copy = $items;
        array_push($copy, $newItem);
        $afterPush = $copy;
        $popped = array_pop($copy);

        return [$afterPush, $popped];
    }

    public static function mergeUnique(array $a, array $b): array
    {
        return array_values(array_unique(array_merge($a, $b)));
    }

    public static function sortAscending(array $numbers): array
    {
        $copy = $numbers;
        sort($copy);

        return $copy;
    }
}
