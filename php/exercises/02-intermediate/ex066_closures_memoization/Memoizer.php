<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex066ClosuresMemoization;

/*
Exercise 066 - Closures and memoization (intermediate).

Goal:   Wrap a callable in a memoizing closure that caches results by argument list.
Drills: closures, use (&$cache) by-reference capture, serialize as a cache key.
Passes: MemoizerTest
*/
final class Memoizer
{
    public static function memoize(callable $fn): \Closure
    {
        throw new \RuntimeException('TODO');
    }
}
