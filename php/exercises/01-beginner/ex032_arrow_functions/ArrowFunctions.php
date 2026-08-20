<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex032ArrowFunctions;

/*
Exercise 032 - Arrow functions (beginner).

Goal:   Implement short closures using PHP's `fn() => expr` syntax.
Drills: fn() => expr short closures, automatic by-value capture of outer-scope variables.
Passes: ArrowFunctionsTest
*/
final class ArrowFunctions
{
    private function __construct()
    {
    }

    public static function buildMultiplier(int $factor): \Closure
    {
        throw new \RuntimeException('TODO');
    }

    public static function doubleEachViaArrowFn(array $numbers): array
    {
        throw new \RuntimeException('TODO');
    }
}
