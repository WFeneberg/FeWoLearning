<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex027RangeStep;

/*
Exercise 027 - Range with step (beginner).

Goal:   Build ascending/descending integer ranges and reverse an array.
Drills: range() with a step argument, array_reverse.
Passes: RangeStepTest
*/
final class RangeStep
{
    private function __construct()
    {
    }

    public static function rangeWithStep(int $start, int $end, int $step): array
    {
        throw new \RuntimeException('TODO');
    }

    public static function reverseRange(array $values): array
    {
        throw new \RuntimeException('TODO');
    }
}
