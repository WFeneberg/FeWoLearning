<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex060MagicGetSet;

/*
Exercise 060 - Magic __get/__set/__isset (intermediate).

Goal:   Back dynamic property access on an object with an internal array via magic methods.
Drills: __get, __set, __isset magic methods, dynamic property access.
Passes: BagTest
*/
final class Bag
{
    private array $data = [];

    public function __get(string $name): mixed
    {
        throw new \RuntimeException('TODO');
    }

    public function __set(string $name, mixed $value): void
    {
        throw new \RuntimeException('TODO');
    }

    public function __isset(string $name): bool
    {
        throw new \RuntimeException('TODO');
    }
}
