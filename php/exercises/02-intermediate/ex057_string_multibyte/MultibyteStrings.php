<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex057StringMultibyte;

/*
Exercise 057 - Multibyte-safe strings (intermediate).

Goal:   Count and truncate strings by character, not by byte, using mbstring functions.
Drills: mb_strlen, mb_substr, mb_strtoupper, byte-vs-character length pitfalls with UTF-8.
Passes: MultibyteStringsTest
*/
final class MultibyteStrings
{
    public static function characterCount(string $text): int
    {
        throw new \RuntimeException('TODO');
    }

    public static function safeTruncate(string $text, int $maxChars): string
    {
        throw new \RuntimeException('TODO');
    }
}
