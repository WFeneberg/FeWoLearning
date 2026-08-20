<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex028ArrayRotation;

final class ArrayRotation
{
    private function __construct()
    {
    }

    public static function rotateLeft(array $items, int $positions): array
    {
        $count = count($items);

        if ($count === 0) {
            return $items;
        }

        $normalizedPositions = $positions % $count;

        $rotated = $items;
        $removed = array_splice($rotated, 0, $normalizedPositions);
        array_splice($rotated, count($rotated), 0, $removed);

        return $rotated;
    }
}
