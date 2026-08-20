<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex048NamedArguments;

final class UserBuilder
{
    public static function build(string $name, int $age = 18, string $country = 'DE'): array
    {
        return ['name' => $name, 'age' => $age, 'country' => $country];
    }
}
