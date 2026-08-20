<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex012NullCoalescing;

final class NullCoalescing
{
    private function __construct()
    {
    }

    public static function firstNonNull(mixed ...$values): mixed
    {
        return array_reduce(
            $values,
            static fn (mixed $carry, mixed $value): mixed => $carry ?? $value,
            null
        );
    }

    public static function configWithDefault(array &$config, string $key, mixed $default): mixed
    {
        $config[$key] ??= $default;

        return $config[$key];
    }
}
