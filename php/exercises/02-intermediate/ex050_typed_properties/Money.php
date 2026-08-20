<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex050TypedProperties;

/*
Exercise 050 - Typed properties and union types (intermediate).

Goal:   Add raw cents or another Money value to a Money instance, validating
        currency compatibility, and return a new Money with the summed cents.
Drills: typed properties, union types on a parameter, validation.
Passes: MoneyTest
*/
final class Money
{
    private int $cents;
    private string $currency;

    public function __construct(int $cents, string $currency)
    {
        $this->cents = $cents;
        $this->currency = $currency;
    }

    public function add(int|self $amount): self
    {
        throw new \RuntimeException('TODO');
    }

    public function cents(): int
    {
        return $this->cents;
    }
}
