<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex061MagicTostring;

/*
Exercise 061 - Magic __toString (intermediate).

Goal:   Format a monetary value as a currency string via implicit string conversion.
Drills: __toString magic method, string casting, string concatenation, number_format.
Passes: Money2Test
*/
final class Money2
{
    public function __construct(private readonly int $cents, private readonly string $currency)
    {
    }

    public function __toString(): string
    {
        throw new \RuntimeException('TODO');
    }
}
