<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex089AbstractBaseContract;

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
        return $this->side * $this->side;
    }

    public function perimeter(): float
    {
        return 4.0 * $this->side;
    }
}

final class Circle2 extends Shape2
{
    public function __construct(private readonly float $radius)
    {
    }

    public function area(): float
    {
        return M_PI * $this->radius * $this->radius;
    }

    public function perimeter(): float
    {
        return 2.0 * M_PI * $this->radius;
    }
}
