<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex023MathFunctions;

/*
Exercise 023 - Math functions (beginner).

Goal:   Implement small wrappers around PHP's built-in math functions.
Drills: round, floor, ceil, intdiv, abs, pow.
Passes: MathFunctionsTest
*/
final class MathFunctions
{
    private function __construct()
    {
    }

    public static function roundHalfUp(float $value, int $precision): float
    {
        throw new \RuntimeException('TODO');
    }

    public static function floorToInt(float $value): int
    {
        throw new \RuntimeException('TODO');
    }

    public static function ceilToInt(float $value): int
    {
        throw new \RuntimeException('TODO');
    }

    public static function integerDivide(int $a, int $b): int
    {
        throw new \RuntimeException('TODO');
    }

    public static function absoluteDifference(int|float $a, int|float $b): int|float
    {
        throw new \RuntimeException('TODO');
    }

    public static function power(int|float $base, int|float $exp): int|float
    {
        throw new \RuntimeException('TODO');
    }
}
