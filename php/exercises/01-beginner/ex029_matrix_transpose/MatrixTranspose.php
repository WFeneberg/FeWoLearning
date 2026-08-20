<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex029MatrixTranspose;

/*
Exercise 029 - Matrix transpose (beginner).

Goal:   Transpose a rectangular matrix represented as a list of row arrays.
Drills: nested arrays, the array_map(null, ...$matrix) transpose idiom.
Passes: MatrixTransposeTest
*/
final class MatrixTranspose
{
    private function __construct()
    {
    }

    public static function transpose(array $matrix): array
    {
        throw new \RuntimeException('TODO');
    }
}
