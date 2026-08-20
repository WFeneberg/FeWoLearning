<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex027RangeStep;

final class RangeStep
{
    private function __construct()
    {
    }

    public static function rangeWithStep(int $start, int $end, int $step): array
    {
        return range($start, $end, abs($step));
    }

    public static function reverseRange(array $values): array
    {
        return array_reverse($values);
    }
}
