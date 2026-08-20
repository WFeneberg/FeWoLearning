<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex020ArraySortWithCallback;

final class ArraySortWithCallback
{
    private function __construct()
    {
    }

    public static function sortByLength(array $strings): array
    {
        $copy = $strings;

        usort($copy, static fn (string $a, string $b): int => strlen($a) <=> strlen($b));

        return $copy;
    }
}
