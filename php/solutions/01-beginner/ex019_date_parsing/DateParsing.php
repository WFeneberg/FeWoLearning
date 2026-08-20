<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex019DateParsing;

final class DateParsing
{
    private function __construct()
    {
    }

    public static function parseIsoDate(string $iso): \DateTimeImmutable
    {
        $date = \DateTimeImmutable::createFromFormat('Y-m-d', $iso);

        if ($date === false) {
            throw new \InvalidArgumentException("Invalid ISO date: {$iso}");
        }

        return $date;
    }

    public static function formatGermanDate(\DateTimeImmutable $date): string
    {
        return $date->format('d.m.Y');
    }
}
