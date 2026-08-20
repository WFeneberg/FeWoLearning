<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex071LruCache;

/*
Exercise 071 - LRU cache (advanced).

Goal:   Implement a fixed-capacity least-recently-used cache backed by a plain array.
Drills: array insertion order, key re-insertion for recency tracking, array_key_first, eviction.
Passes: LruCacheTest
*/
final class LruCache
{
    private array $store = [];

    public function __construct(private readonly int $capacity)
    {
    }

    public function get(string $key): mixed
    {
        throw new \RuntimeException('TODO');
    }

    public function put(string $key, mixed $value): void
    {
        throw new \RuntimeException('TODO');
    }
}
