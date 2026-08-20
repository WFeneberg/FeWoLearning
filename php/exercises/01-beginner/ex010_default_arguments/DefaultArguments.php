<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex010DefaultArguments;

/*
Exercise 010 - Default arguments (beginner).

Goal:   Implement functions that rely on default parameter values.
Drills: default parameter values, parameter ordering rules.
Passes: DefaultArgumentsTest
*/
final class DefaultArguments
{
    private function __construct()
    {
    }

    public static function greet(string $name, string $greeting = 'Hello'): string
    {
        throw new \RuntimeException('TODO');
    }

    public static function repeatString(string $text, int $times = 1): string
    {
        throw new \RuntimeException('TODO');
    }
}
