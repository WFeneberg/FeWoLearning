<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex022MinMaxFunctions;

final class MinMaxFunctions
{
    private function __construct()
    {
    }

    public static function rangeSpan(array $numbers): int|float
    {
        if (count($numbers) === 0) {
            throw new \InvalidArgumentException('numbers must not be empty');
        }

        return max($numbers) - min($numbers);
    }

    public static function average(array $numbers): float
    {
        if (count($numbers) === 0) {
            throw new \InvalidArgumentException('numbers must not be empty');
        }

        return array_sum($numbers) / count($numbers);
    }

    public static function product(array $numbers): int|float
    {
        return array_product($numbers);
    }
}
