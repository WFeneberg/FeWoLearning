<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex026PalindromeCheck;

require_once __DIR__ . '/PalindromeCheck.php';

use PHPUnit\Framework\TestCase;

final class PalindromeCheckTest extends TestCase
{
    public function testSentenceWithPunctuationAndMixedCaseIsPalindrome(): void
    {
        self::assertTrue(PalindromeCheck::isPalindrome('A man, a plan, a canal: Panama'));
    }

    public function testGenuineNonPalindrome(): void
    {
        self::assertFalse(PalindromeCheck::isPalindrome('This is not a palindrome'));
    }

    public function testSimpleLowercasePalindrome(): void
    {
        self::assertTrue(PalindromeCheck::isPalindrome('racecar'));
    }

    public function testSingleCharacterIsPalindrome(): void
    {
        self::assertTrue(PalindromeCheck::isPalindrome('a'));
    }

    public function testEmptyStringIsPalindrome(): void
    {
        self::assertTrue(PalindromeCheck::isPalindrome(''));
    }
}
