<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex037GeneratorsBasics;

/*
Exercise 037 - Generators basics (intermediate).

Goal:   Implement a generator that lazily yields Fibonacci numbers.
Drills: yield, generator functions, laziness (a generator body doesn't run until iterated).
Passes: SequencesTest
*/
final class Sequences
{
    private function __construct()
    {
    }

    public static function fibonacci(int $count): \Generator
    {
        throw new \RuntimeException('TODO');

        yield 0; // unreachable; keeps this function a generator so it stays lazy until iterated
    }
}
