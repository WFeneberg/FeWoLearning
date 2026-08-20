<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex001Temperature;

final class Temperature
{
    private function __construct()
    {
    }

    public static function celsiusToFahrenheit(float $celsius): float
    {
        return $celsius * 9.0 / 5.0 + 32.0;
    }

    public static function fahrenheitToCelsius(float $fahrenheit): float
    {
        return ($fahrenheit - 32.0) * 5.0 / 9.0;
    }

    public static function roundToOneDecimal(float $value): float
    {
        return round($value, 1);
    }
}
