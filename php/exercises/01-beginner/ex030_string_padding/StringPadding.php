<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex030StringPadding;

/*
Exercise 030 - String padding (beginner).

Goal:   Implement zero-padding, centering, and separator-repeating helpers.
Drills: str_pad, str_repeat, STR_PAD_LEFT/STR_PAD_BOTH.
Passes: StringPaddingTest
*/
final class StringPadding
{
    private function __construct()
    {
    }

    public static function padLeftZero(int $number, int $width): string
    {
        throw new \RuntimeException('TODO');
    }

    public static function centerText(string $text, int $width): string
    {
        throw new \RuntimeException('TODO');
    }

    public static function repeatSeparator(string $char, int $times): string
    {
        throw new \RuntimeException('TODO');
    }
}
