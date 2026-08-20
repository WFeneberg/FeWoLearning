<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex006AssociativeArrays;

final class AssociativeArrays
{
    private function __construct()
    {
    }

    public static function getOrDefault(array $map, string $key, mixed $default): mixed
    {
        return array_key_exists($key, $map) ? $map[$key] : $default;
    }

    public static function hasKeyEvenIfNull(array $map, string $key): bool
    {
        return array_key_exists($key, $map);
    }

    public static function hasNonNullValue(array $map, string $key): bool
    {
        return isset($map[$key]);
    }
}
