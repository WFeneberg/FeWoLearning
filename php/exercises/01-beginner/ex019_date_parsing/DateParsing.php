<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex019DateParsing;

/*
Exercise 019 - Date parsing and formatting (beginner).

Goal:   Parse an ISO date string and format a date in German notation.
Drills: \DateTimeImmutable::createFromFormat, ->format().
Passes: DateParsingTest
*/
final class DateParsing
{
    private function __construct()
    {
    }

    public static function parseIsoDate(string $iso): \DateTimeImmutable
    {
        throw new \RuntimeException('TODO');
    }

    public static function formatGermanDate(\DateTimeImmutable $date): string
    {
        throw new \RuntimeException('TODO');
    }
}
