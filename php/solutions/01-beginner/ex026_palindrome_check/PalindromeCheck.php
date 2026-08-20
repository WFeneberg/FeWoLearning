<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex026PalindromeCheck;

final class PalindromeCheck
{
    private function __construct()
    {
    }

    public static function isPalindrome(string $text): bool
    {
        $normalized = preg_replace('/[^a-z0-9]/', '', mb_strtolower($text));

        return $normalized === strrev($normalized);
    }
}
