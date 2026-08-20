<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex011VariadicFunctions;

final class VariadicFunctions
{
    private function __construct()
    {
    }

    public static function sumAll(int ...$numbers): int
    {
        return array_sum($numbers);
    }

    public static function joinWithSeparator(string $separator, string ...$parts): string
    {
        return implode($separator, $parts);
    }
}
