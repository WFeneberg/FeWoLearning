<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex071LruCache;

final class LruCache
{
    private array $store = [];

    public function __construct(private readonly int $capacity)
    {
    }

    public function get(string $key): mixed
    {
        if (!array_key_exists($key, $this->store)) {
            return null;
        }

        $value = $this->store[$key];
        unset($this->store[$key]);
        $this->store[$key] = $value;

        return $value;
    }

    public function put(string $key, mixed $value): void
    {
        if (array_key_exists($key, $this->store)) {
            unset($this->store[$key]);
        }

        $this->store[$key] = $value;

        if (count($this->store) > $this->capacity) {
            $oldestKey = array_key_first($this->store);
            unset($this->store[$oldestKey]);
        }
    }
}
