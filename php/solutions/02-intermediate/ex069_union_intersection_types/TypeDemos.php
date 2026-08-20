<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex069UnionIntersectionTypes;

final class TypeDemos
{
    public static function normalizeId(int|string $id): string
    {
        if (is_int($id)) {
            return (string) $id;
        }

        return ltrim($id, '0') ?: '0';
    }

    public static function combinedLength(\Countable&\IteratorAggregate $collection): int
    {
        return count($collection);
    }
}
