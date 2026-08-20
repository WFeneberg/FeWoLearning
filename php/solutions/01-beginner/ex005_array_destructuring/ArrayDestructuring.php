<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex005ArrayDestructuring;

final class ArrayDestructuring
{
    private function __construct()
    {
    }

    public static function swapPair(array $pair): array
    {
        [$a, $b] = $pair;

        return [$b, $a];
    }

    public static function extractCoordinates(array $point): array
    {
        ['x' => $x, 'y' => $y, 'z' => $z] = $point;

        return [$x, $y, $z];
    }

    public static function firstAndRest(array $items): array
    {
        return [
            'first' => $items[0],
            'rest' => array_slice($items, 1),
        ];
    }
}
