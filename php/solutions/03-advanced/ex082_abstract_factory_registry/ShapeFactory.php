<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex082AbstractFactoryRegistry;

final class ShapeFactory
{
    private static array $registry = [];

    public static function register(string $key, \Closure $factory): void
    {
        self::$registry[$key] = $factory;
    }

    public static function create(string $key, mixed ...$args): object
    {
        if (!array_key_exists($key, self::$registry)) {
            throw new \InvalidArgumentException("no factory registered for key '{$key}'");
        }

        return (self::$registry[$key])(...$args);
    }

    public static function reset(): void
    {
        self::$registry = [];
    }
}
