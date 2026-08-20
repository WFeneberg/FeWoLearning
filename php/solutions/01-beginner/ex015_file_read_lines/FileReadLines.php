<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex015FileReadLines;

final class FileReadLines
{
    private function __construct()
    {
    }

    public static function readNonEmptyLines(string $path): array
    {
        $lines = file($path, FILE_IGNORE_NEW_LINES);
        if ($lines === false) {
            return [];
        }

        $trimmed = array_map(static fn (string $line): string => trim($line), $lines);

        return array_values(array_filter(
            $trimmed,
            static fn (string $line): bool => $line !== ''
        ));
    }
}
