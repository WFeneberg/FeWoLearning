<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex035InterfacesBasic;

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
        return M_PI * $this->radius ** 2;
    }
}

final class Square implements Shape
{
    public function __construct(private readonly float $side)
    {
    }

    public function area(): float
    {
        return $this->side ** 2;
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
        $total = 0.0;
        foreach ($shapes as $shape) {
            $total += $shape->area();
        }

        return $total;
    }
}
