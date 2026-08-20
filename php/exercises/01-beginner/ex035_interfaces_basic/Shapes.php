<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex035InterfacesBasic;

/*
Exercise 035 - Interfaces (beginner).

Goal:   Implement a Shape interface with two concrete shapes and a helper that
        sums their areas polymorphically.
Drills: interface declaration, implements, polymorphism via a type-hinted parameter.
Passes: ShapesTest
*/
interface Shape
{
    public function area(): float;
}

final class Circle implements Shape
{
    public function __construct(private readonly float $radius)
    {
    }

    public function area(): float
    {
        throw new \RuntimeException('TODO');
    }
}

final class Square implements Shape
{
    public function __construct(private readonly float $side)
    {
    }

    public function area(): float
    {
        throw new \RuntimeException('TODO');
    }
}

final class ShapeMath
{
    private function __construct()
    {
    }

    /**
     * @param Shape[] $shapes
     */
    public static function totalArea(array $shapes): float
    {
        throw new \RuntimeException('TODO');
    }
}
