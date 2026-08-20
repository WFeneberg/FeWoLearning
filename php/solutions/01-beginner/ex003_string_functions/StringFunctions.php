<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex003StringFunctions;

final class StringFunctions
{
    private function __construct()
    {
    }

    public static function hasSubstring(string $haystack, string $needle): bool
    {
        return str_contains($haystack, $needle);
    }

    public static function startsWithPrefix(string $text, string $prefix): bool
    {
        return str_starts_with($text, $prefix);
    }

    public static function endsWithSuffix(string $text, string $suffix): bool
    {
        return str_ends_with($text, $suffix);
    }

    public static function truncate(string $text, int $maxLength): string
    {
        if (strlen($text) > $maxLength) {
            return substr($text, 0, $maxLength) . '...';
        }

        return $text;
    }

    public static function replaceAll(string $subject, array $map): string
    {
        return strtr($subject, $map);
    }
}
