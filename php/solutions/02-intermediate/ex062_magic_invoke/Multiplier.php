<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex062MagicInvoke;

final class Multiplier
{
    public function __construct(private readonly int $factor)
    {
    }

    public function __invoke(int $n): int
    {
        return $n * $this->factor;
    }
}
