<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex090ValueObjectEquality;

final class Money3
{
    public function __construct(
        public readonly int $cents,
        public readonly string $currency,
    ) {
    }

    public function equals(self $other): bool
    {
        return $this->cents === $other->cents && $this->currency === $other->currency;
    }

    public function add(self $other): self
    {
        if ($this->currency !== $other->currency) {
            throw new \InvalidArgumentException(
                "cannot add {$other->currency} to {$this->currency}"
            );
        }

        return new self($this->cents + $other->cents, $this->currency);
    }
}
