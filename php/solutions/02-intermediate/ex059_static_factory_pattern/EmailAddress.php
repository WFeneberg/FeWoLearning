<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex059StaticFactoryPattern;

final class EmailAddress
{
    private function __construct(private readonly string $value)
    {
    }

    public static function fromString(string $value): self
    {
        if (filter_var($value, FILTER_VALIDATE_EMAIL) === false) {
            throw new \InvalidArgumentException("Invalid email address: {$value}");
        }

        return new self($value);
    }

    public function __toString(): string
    {
        return $this->value;
    }
}
