<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex034BasicClassConstructor;

final class Rectangle
{
    public function __construct(
        private readonly float $width,
        private readonly float $height
    ) {
        if ($width < 0.0 || $height < 0.0) {
            throw new \InvalidArgumentException('Width and height must not be negative.');
        }
    }

    public function area(): float
    {
        return $this->width * $this->height;
    }

    public function perimeter(): float
    {
        return 2.0 * ($this->width + $this->height);
    }
}
