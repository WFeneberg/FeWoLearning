<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex026PalindromeCheck;

/*
Exercise 026 - Palindrome check (beginner).

Goal:   Determine whether a string is a palindrome, ignoring case and punctuation.
Drills: string normalization (mb_strtolower), regex filtering, strrev.
Passes: PalindromeCheckTest
*/
final class PalindromeCheck
{
    private function __construct()
    {
    }

    public static function isPalindrome(string $text): bool
    {
        throw new \RuntimeException('TODO');
    }
}
