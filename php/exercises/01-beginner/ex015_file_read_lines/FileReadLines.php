<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex015FileReadLines;

/*
Exercise 015 - Reading non-empty lines from a file (beginner).

Goal:   Read a text file and return only its non-empty, trimmed lines.
Drills: file() with FILE_IGNORE_NEW_LINES, filtering empty lines.
Passes: FileReadLinesTest
*/
final class FileReadLines
{
    private function __construct()
    {
    }

    public static function readNonEmptyLines(string $path): array
    {
        throw new \RuntimeException('TODO');
    }
}
