<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex014InputValidation;

/*
Exercise 014 - Input validation (beginner).

Goal:   Validate an age using a guard clause that throws on out-of-range input.
Drills: guard clauses, throwing \InvalidArgumentException with a useful message.
Passes: InputValidationTest
*/
final class InputValidation
{
    private function __construct()
    {
    }

    public static function validateAge(int $age): int
    {
        throw new \RuntimeException('TODO');
    }
}
