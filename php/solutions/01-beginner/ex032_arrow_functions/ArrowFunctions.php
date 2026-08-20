<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex032ArrowFunctions;

final class ArrowFunctions
{
    private function __construct()
    {
    }

    public static function buildMultiplier(int $factor): \Closure
    {
        return fn (int $n): int => $n * $factor;
    }

    public static function doubleEachViaArrowFn(array $numbers): array
    {
        return array_map(fn ($n) => $n * 2, $numbers);
    }
}
