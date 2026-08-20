<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex003StringFunctions;

/*
Exercise 003 - String functions (beginner).

Goal:   Implement helpers built on PHP's built-in substring/search functions.
Drills: str_contains, str_starts_with, str_ends_with, substr, strtr.
Passes: StringFunctionsTest
*/
final class StringFunctions
{
    private function __construct()
    {
    }

    public static function hasSubstring(string $haystack, string $needle): bool
    {
        throw new \RuntimeException('TODO');
    }

    public static function startsWithPrefix(string $text, string $prefix): bool
    {
        throw new \RuntimeException('TODO');
    }

    public static function endsWithSuffix(string $text, string $suffix): bool
    {
        throw new \RuntimeException('TODO');
    }

    public static function truncate(string $text, int $maxLength): string
    {
        throw new \RuntimeException('TODO');
    }

    public static function replaceAll(string $subject, array $map): string
    {
        throw new \RuntimeException('TODO');
    }
}
