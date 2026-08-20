<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex014InputValidation;

final class InputValidation
{
    private function __construct()
    {
    }

    public static function validateAge(int $age): int
    {
        if ($age < 0 || $age > 150) {
            throw new \InvalidArgumentException("Age must be between 0 and 150, got {$age}.");
        }

        return $age;
    }
}
