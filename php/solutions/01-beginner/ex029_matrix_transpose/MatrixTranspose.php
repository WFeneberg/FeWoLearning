<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex029MatrixTranspose;

final class MatrixTranspose
{
    private function __construct()
    {
    }

    public static function transpose(array $matrix): array
    {
        return array_map(null, ...$matrix);
    }
}
