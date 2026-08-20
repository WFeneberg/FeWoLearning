<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex089AbstractBaseContract;

/*
Exercise 089 - Abstract base contract (advanced).

Goal:   Implement two concrete subclasses of an abstract base class so that
        its concrete describe() method (built on the abstract contract)
        produces correct output for both.
Drills: abstract classes, abstract methods, polymorphism, contract testing.
Passes: Shape2Test
*/
abstract class Shape2
{
    abstract public function area(): float;

    abstract public function perimeter(): float;

    public function describe(): string
    {
        return sprintf('area=%.2f perimeter=%.2f', $this->area(), $this->perimeter());
    }
}

final class Square2 extends Shape2
{
    public function __construct(private readonly float $side)
    {
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

final class Circle2 extends Shape2
{
    public function __construct(private readonly float $radius)
    {
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
