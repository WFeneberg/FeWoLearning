<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex048NamedArguments;

/*
Exercise 048 - Named arguments (intermediate).

Goal:   Build a user array from positional and named arguments with defaults.
Drills: named arguments, default parameter values, skipping optional params.
Passes: UserBuilderTest
*/
final class UserBuilder
{
    public static function build(string $name, int $age = 18, string $country = 'DE'): array
    {
        throw new \RuntimeException('TODO');
    }
}
