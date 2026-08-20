<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex034BasicClassConstructor;

/*
Exercise 034 - Constructor property promotion (beginner).

Goal:   Implement a Rectangle using constructor property promotion and computed methods.
Drills: constructor property promotion, computed methods over promoted properties.
Passes: RectangleTest
*/
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
        throw new \RuntimeException('TODO');
    }

    public function perimeter(): float
    {
        throw new \RuntimeException('TODO');
    }
}
