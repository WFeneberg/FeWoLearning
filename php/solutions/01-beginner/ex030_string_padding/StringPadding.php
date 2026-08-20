<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex030StringPadding;

final class StringPadding
{
    private function __construct()
    {
    }

    public static function padLeftZero(int $number, int $width): string
    {
        return str_pad((string) $number, $width, '0', STR_PAD_LEFT);
    }

    public static function centerText(string $text, int $width): string
    {
        return str_pad($text, $width, ' ', STR_PAD_BOTH);
    }

    public static function repeatSeparator(string $char, int $times): string
    {
        return str_repeat($char, $times);
    }
}
