<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex018DateArithmetic;

final class DateArithmetic
{
    private function __construct()
    {
    }

    public static function addDays(\DateTimeImmutable $date, int $days): \DateTimeImmutable
    {
        if ($days >= 0) {
            return $date->add(new \DateInterval("P{$days}D"));
        }

        return $date->sub(new \DateInterval('P' . abs($days) . 'D'));
    }

    public static function daysBetween(\DateTimeImmutable $a, \DateTimeImmutable $b): int
    {
        return (int) $a->diff($b)->format('%a');
    }
}
