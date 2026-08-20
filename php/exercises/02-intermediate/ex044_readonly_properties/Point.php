<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex044ReadonlyProperties;

/*
Exercise 044 - Readonly properties (intermediate).

Goal:   Implement immutable "with"-style update methods for a readonly Point.
Drills: readonly properties, immutability, Error on mutation attempt.
Passes: PointTest
*/
final class Point
{
    public function __construct(
        public readonly float $x,
        public readonly float $y,
    ) {
    }

    public function withX(float $x): self
    {
        throw new \RuntimeException('TODO');
    }

    public function withY(float $y): self
    {
        throw new \RuntimeException('TODO');
    }
}
