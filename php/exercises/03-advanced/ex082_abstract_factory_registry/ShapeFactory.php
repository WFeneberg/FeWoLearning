<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex082AbstractFactoryRegistry;

/*
Exercise 082 - Abstract factory registry (advanced).

Goal:   Implement a static registry mapping string keys to factory closures,
        enabling runtime-extensible object creation without a big switch/match.
Drills: static registry, closures, runtime-extensible object creation.
Passes: ShapeFactoryTest
*/
final class ShapeFactory
{
    private static array $registry = [];

    public static function register(string $key, \Closure $factory): void
    {
        throw new \RuntimeException('TODO');
    }

    public static function create(string $key, mixed ...$args): object
    {
        throw new \RuntimeException('TODO');
    }

    public static function reset(): void
    {
        self::$registry = [];
    }
}
