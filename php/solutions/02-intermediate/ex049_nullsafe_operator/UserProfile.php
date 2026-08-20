<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex049NullsafeOperator;

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
        return $user?->address?->city ?? 'Unknown';
    }
}
