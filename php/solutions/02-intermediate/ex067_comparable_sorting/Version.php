<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex067ComparableSorting;

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
        /** @var Version $other */
        return $this->major <=> $other->major
            ?: $this->minor <=> $other->minor
            ?: $this->patch <=> $other->patch;
    }
}

final class Versions
{
    public static function sort(array $versions): array
    {
        $copy = $versions;

        usort($copy, static fn (Version $a, Version $b): int => $a->compareTo($b));

        return $copy;
    }
}
