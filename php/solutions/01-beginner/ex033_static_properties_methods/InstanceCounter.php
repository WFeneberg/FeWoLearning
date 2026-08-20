<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex033StaticPropertiesMethods;

final class InstanceCounter
{
    private static int $count = 0;

    public function __construct()
    {
        self::$count++;
    }

    public static function current(): int
    {
        return self::$count;
    }

    public static function reset(): void
    {
        self::$count = 0;
    }
}
