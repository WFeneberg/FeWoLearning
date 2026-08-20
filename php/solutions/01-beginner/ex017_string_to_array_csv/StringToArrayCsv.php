<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex017StringToArrayCsv;

final class StringToArrayCsv
{
    private function __construct()
    {
    }

    public static function parseCsvLine(string $line): array
    {
        $fields = str_getcsv($line);

        return array_map(static fn (mixed $field): string => (string) $field, $fields);
    }

    public static function joinFields(array $fields, string $delimiter = ','): string
    {
        return implode($delimiter, $fields);
    }
}
