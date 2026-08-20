<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex070DtoWithValidation;

/*
Exercise 070 - DTO with constructor validation (intermediate).

Goal:   Validate promoted constructor properties and reject invalid input.
Drills: constructor property promotion, readonly properties, validation in the constructor.
Passes: RegistrationTest
*/
final class Registration
{
    public function __construct(
        public readonly string $email,
        public readonly int $age,
    ) {
        throw new \RuntimeException('TODO');
    }
}
