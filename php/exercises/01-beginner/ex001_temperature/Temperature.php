<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex001Temperature;

/*
Exercise 001 - Temperature conversion (beginner).

Goal:   Implement Celsius <-> Fahrenheit conversion helpers.
Drills: functions, arithmetic, float return types, rounding.
Passes: TemperatureTest
*/
final class Temperature
{
    private function __construct()
    {
    }

    public static function celsiusToFahrenheit(float $celsius): float
    {
        throw new \RuntimeException('TODO');
    }

    public static function fahrenheitToCelsius(float $fahrenheit): float
    {
        throw new \RuntimeException('TODO');
    }

    public static function roundToOneDecimal(float $value): float
    {
        throw new \RuntimeException('TODO');
    }
}
