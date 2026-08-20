<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex050TypedProperties;

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
        if ($amount instanceof self) {
            if ($amount->currency !== $this->currency) {
                throw new \InvalidArgumentException('Currency mismatch');
            }

            return new self($this->cents + $amount->cents, $this->currency);
        }

        return new self($this->cents + $amount, $this->currency);
    }

    public function cents(): int
    {
        return $this->cents;
    }
}
