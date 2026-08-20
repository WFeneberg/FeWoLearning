<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex053ArrayAccessInterface;

/*
Exercise 053 - ArrayAccess interface (intermediate).

Goal:   Implement a list-like object that supports [] indexing via ArrayAccess.
Drills: ArrayAccess interface, offsetExists/offsetGet/offsetSet/offsetUnset, append via null offset.
Passes: TypedListTest
*/
final class TypedList implements \ArrayAccess
{
    private array $items = [];

    public function offsetExists(mixed $offset): bool
    {
        throw new \RuntimeException('TODO');
    }

    public function offsetGet(mixed $offset): mixed
    {
        throw new \RuntimeException('TODO');
    }

    public function offsetSet(mixed $offset, mixed $value): void
    {
        throw new \RuntimeException('TODO');
    }

    public function offsetUnset(mixed $offset): void
    {
        throw new \RuntimeException('TODO');
    }
}
