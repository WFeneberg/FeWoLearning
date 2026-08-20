<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex018DateArithmetic;

/*
Exercise 018 - Date arithmetic (beginner).

Goal:   Add/subtract days from a date and compute the number of days between two dates.
Drills: \DateTimeImmutable, \DateInterval, add/sub, diff.
Passes: DateArithmeticTest
*/
final class DateArithmetic
{
    private function __construct()
    {
    }

    public static function addDays(\DateTimeImmutable $date, int $days): \DateTimeImmutable
    {
        throw new \RuntimeException('TODO');
    }

    public static function daysBetween(\DateTimeImmutable $a, \DateTimeImmutable $b): int
    {
        throw new \RuntimeException('TODO');
    }
}
