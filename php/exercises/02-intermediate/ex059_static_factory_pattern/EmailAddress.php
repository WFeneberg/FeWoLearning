<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex059StaticFactoryPattern;

/*
Exercise 059 - Static factory pattern (intermediate).

Goal:   Force validated construction of an email value object via a named constructor.
Drills: private constructor, static factory method, filter_var validation, __toString.
Passes: EmailAddressTest
*/
final class EmailAddress
{
    private function __construct(private readonly string $value)
    {
    }

    public static function fromString(string $value): self
    {
        throw new \RuntimeException('TODO');
    }

    public function __toString(): string
    {
        return $this->value;
    }
}
