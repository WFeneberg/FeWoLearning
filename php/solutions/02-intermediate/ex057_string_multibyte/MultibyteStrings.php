<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex057StringMultibyte;

final class MultibyteStrings
{
    public static function characterCount(string $text): int
    {
        return mb_strlen($text, 'UTF-8');
    }

    public static function safeTruncate(string $text, int $maxChars): string
    {
        return mb_substr($text, 0, $maxChars, 'UTF-8');
    }
}
