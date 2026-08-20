<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex013ExceptionHandling;

/*
Exercise 013 - Exception handling (beginner).

Goal:   Implement a safe integer division that reports errors instead of throwing.
Drills: try/catch/finally, catching specific throwable types, intdiv's DivisionByZeroError.
Passes: ExceptionHandlingTest
*/
final class ExceptionHandling
{
    public static int $finallyRuns = 0;

    private function __construct()
    {
    }

    public static function safeDivide(int $numerator, int $denominator): array
    {
        throw new \RuntimeException('TODO');
    }
}
