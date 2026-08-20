<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex069UnionIntersectionTypes;

/*
Exercise 069 - Union and intersection types (intermediate).

Goal:   Accept a union-typed parameter and an intersection-typed parameter.
Drills: union types (int|string), intersection types (\Countable&\IteratorAggregate).
Passes: TypeDemosTest
*/
final class TypeDemos
{
    public static function normalizeId(int|string $id): string
    {
        throw new \RuntimeException('TODO');
    }

    public static function combinedLength(\Countable&\IteratorAggregate $collection): int
    {
        throw new \RuntimeException('TODO');
    }
}
