<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex002StringFormatting;

final class StringFormatting
{
    private function __construct()
    {
    }

    public static function formatCurrency(float $amount): string
    {
        return number_format($amount, 2);
    }

    public static function formatPercentage(float $ratio, int $decimals): string
    {
        return sprintf('%.' . $decimals . 'f%%', $ratio * 100);
    }

    public static function padLabel(string $label, int $width): string
    {
        return str_pad($label, $width);
    }
}
