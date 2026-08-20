<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex002StringFormatting;

/*
Exercise 002 - String formatting (beginner).

Goal:   Implement helpers for currency, percentage, and column-aligned label formatting.
Drills: sprintf, number_format, str_pad for column alignment.
Passes: StringFormattingTest
*/
final class StringFormatting
{
    private function __construct()
    {
    }

    public static function formatCurrency(float $amount): string
    {
        throw new \RuntimeException('TODO');
    }

    public static function formatPercentage(float $ratio, int $decimals): string
    {
        throw new \RuntimeException('TODO');
    }

    public static function padLabel(string $label, int $width): string
    {
        throw new \RuntimeException('TODO');
    }
}
