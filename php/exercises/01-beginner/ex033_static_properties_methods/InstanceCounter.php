<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex033StaticPropertiesMethods;

/*
Exercise 033 - Static properties and methods (beginner).

Goal:   Track how many instances of a class have been created using a static property.
Drills: static properties shared across all instances, self:: access, static counting method.
Passes: InstanceCounterTest
*/
final class InstanceCounter
{
    private static int $count = 0;

    public function __construct()
    {
        throw new \RuntimeException('TODO');
    }

    public static function current(): int
    {
        throw new \RuntimeException('TODO');
    }

    public static function reset(): void
    {
        throw new \RuntimeException('TODO');
    }
}
