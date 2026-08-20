<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex067ComparableSorting;

/*
Exercise 067 - Comparable sorting (intermediate).

Goal:   Implement a hand-rolled Comparable interface and sort objects with usort.
Drills: interfaces, compareTo, spaceship operator chaining, usort.
Passes: VersionTest
*/
interface Comparable
{
    public function compareTo(self $other): int;
}

final class Version implements Comparable
{
    public function __construct(
        public readonly int $major,
        public readonly int $minor,
        public readonly int $patch,
    ) {
    }

    public function compareTo(Comparable $other): int
    {
        throw new \RuntimeException('TODO');
    }
}

final class Versions
{
    public static function sort(array $versions): array
    {
        throw new \RuntimeException('TODO');
    }
}
