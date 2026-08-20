<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex044ReadonlyProperties;

final class Point
{
    public function __construct(
        public readonly float $x,
        public readonly float $y,
    ) {
    }

    public function withX(float $x): self
    {
        return new self($x, $this->y);
    }

    public function withY(float $y): self
    {
        return new self($this->x, $y);
    }
}
