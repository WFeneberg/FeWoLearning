<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex049NullsafeOperator;

/*
Exercise 049 - Nullsafe operator (intermediate).

Goal:   Resolve a user's city through a chain of possibly-null objects using
        the nullsafe operator, falling back to a default when anything is null.
Drills: nullsafe operator (?->), null coalescing, avoiding manual isset pyramids.
Passes: UserProfileTest
*/
final class Address
{
    public function __construct(public readonly string $city)
    {
    }
}

final class User
{
    public function __construct(public readonly ?Address $address = null)
    {
    }
}

final class UserProfiles
{
    public static function cityOrDefault(?User $user): string
    {
        throw new \RuntimeException('TODO');
    }
}
