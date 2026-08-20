<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex011VariadicFunctions;

/*
Exercise 011 - Variadic functions (beginner).

Goal:   Implement functions that accept a variable number of arguments.
Drills: ...$args variadic parameters, ... spread operator when calling.
Passes: VariadicFunctionsTest
*/
final class VariadicFunctions
{
    private function __construct()
    {
    }

    public static function sumAll(int ...$numbers): int
    {
        throw new \RuntimeException('TODO');
    }

    public static function joinWithSeparator(string $separator, string ...$parts): string
    {
        throw new \RuntimeException('TODO');
    }
}
