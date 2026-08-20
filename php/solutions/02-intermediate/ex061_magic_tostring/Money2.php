<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex061MagicTostring;

final class Money2
{
    public function __construct(private readonly int $cents, private readonly string $currency)
    {
    }

    public function __toString(): string
    {
        return number_format($this->cents / 100, 2) . " {$this->currency}";
    }
}
