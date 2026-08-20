<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex039YieldFromFlatten;

final class Flattener
{
    private function __construct()
    {
    }

    public static function flatten(array $nested): \Generator
    {
        foreach ($nested as $item) {
            if (is_array($item)) {
                yield from self::flatten($item);
            } else {
                yield $item;
            }
        }
    }
}
