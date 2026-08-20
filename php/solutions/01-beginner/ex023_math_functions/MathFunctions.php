<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex023MathFunctions;

final class MathFunctions
{
    private function __construct()
    {
    }

    public static function roundHalfUp(float $value, int $precision): float
    {
        return round($value, $precision);
    }

    public static function floorToInt(float $value): int
    {
        return (int) floor($value);
    }

    public static function ceilToInt(float $value): int
    {
        return (int) ceil($value);
    }

    public static function integerDivide(int $a, int $b): int
    {
        return intdiv($a, $b);
    }

    public static function absoluteDifference(int|float $a, int|float $b): int|float
    {
        return abs($a - $b);
    }

    public static function power(int|float $base, int|float $exp): int|float
    {
        return $base ** $exp;
    }
}
