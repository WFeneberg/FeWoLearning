<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex001Temperature;

require_once __DIR__ . '/Temperature.php';

use PHPUnit\Framework\TestCase;

final class TemperatureTest extends TestCase
{
    public function testCelsiusToFahrenheitFreezing(): void
    {
        self::assertEqualsWithDelta(32.0, Temperature::celsiusToFahrenheit(0.0), 0.001);
    }

    public function testCelsiusToFahrenheitBoiling(): void
    {
        self::assertEqualsWithDelta(212.0, Temperature::celsiusToFahrenheit(100.0), 0.001);
    }

    public function testFahrenheitToCelsiusNegative(): void
    {
        self::assertEqualsWithDelta(-40.0, Temperature::fahrenheitToCelsius(-40.0), 0.001);
    }

    public function testRoundToOneDecimal(): void
    {
        self::assertEqualsWithDelta(21.4, Temperature::roundToOneDecimal(21.351), 0.0001);
        self::assertEqualsWithDelta(21.4, Temperature::roundToOneDecimal(21.449), 0.0001);
    }

    public function testRoundTripIsIdentity(): void
    {
        $celsius = 36.6;
        $roundTripped = Temperature::fahrenheitToCelsius(Temperature::celsiusToFahrenheit($celsius));

        self::assertEqualsWithDelta($celsius, $roundTripped, 0.0001);
    }
}
