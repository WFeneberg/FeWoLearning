<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex017StringToArrayCsv;

/*
Exercise 017 - CSV line parsing and joining (beginner).

Goal:   Parse a single CSV line into fields and join fields back into a CSV line.
Drills: str_getcsv, implode.
Passes: StringToArrayCsvTest
*/
final class StringToArrayCsv
{
    private function __construct()
    {
    }

    public static function parseCsvLine(string $line): array
    {
        throw new \RuntimeException('TODO');
    }

    public static function joinFields(array $fields, string $delimiter = ','): string
    {
        throw new \RuntimeException('TODO');
    }
}
