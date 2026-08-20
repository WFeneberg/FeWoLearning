<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex078WeakmapCache;

/*
Exercise 078 - WeakMap-backed memoization cache (advanced).

Goal:   Cache a per-object computed value using a WeakMap so cache entries
        don't keep their key objects alive.
Drills: \WeakMap, object-keyed storage, memoization, \Closure invocation.
Passes: WeakMapCacheTest
*/
final class WeakMapCache
{
    private \WeakMap $map;

    public function __construct()
    {
        $this->map = new \WeakMap();
    }

    public function rememberFor(object $key, \Closure $compute): mixed
    {
        throw new \RuntimeException('TODO');
    }
}
