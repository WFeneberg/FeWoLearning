<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex066ClosuresMemoization;

final class Memoizer
{
    public static function memoize(callable $fn): \Closure
    {
        $cache = [];

        return function (...$args) use (&$cache, $fn) {
            $key = serialize($args);

            if (!array_key_exists($key, $cache)) {
                $cache[$key] = $fn(...$args);
            }

            return $cache[$key];
        };
    }
}
