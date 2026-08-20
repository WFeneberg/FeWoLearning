<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex070DtoWithValidation;

final class Registration
{
    public function __construct(
        public readonly string $email,
        public readonly int $age,
    ) {
        if (!filter_var($email, FILTER_VALIDATE_EMAIL)) {
            throw new \InvalidArgumentException('invalid email');
        }

        if ($age < 0 || $age > 150) {
            throw new \InvalidArgumentException('invalid age');
        }
    }
}
