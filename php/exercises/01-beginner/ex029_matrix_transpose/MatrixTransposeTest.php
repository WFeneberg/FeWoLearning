<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex029MatrixTranspose;

require_once __DIR__ . '/MatrixTranspose.php';

use PHPUnit\Framework\TestCase;

final class MatrixTransposeTest extends TestCase
{
    public function testTransposeNonSquareMatrixSwapsDimensions(): void
    {
        $matrix = [
            [1, 2, 3],
            [4, 5, 6],
        ];

        $expected = [
            [1, 4],
            [2, 5],
            [3, 6],
        ];

        self::assertSame($expected, MatrixTranspose::transpose($matrix));
    }

    public function testTransposeSquareMatrix(): void
    {
        $matrix = [
            [1, 2],
            [3, 4],
        ];

        $expected = [
            [1, 3],
            [2, 4],
        ];

        self::assertSame($expected, MatrixTranspose::transpose($matrix));
    }

    public function testTransposeSingleRowBecomesSingleColumn(): void
    {
        $matrix = [[1, 2, 3]];

        $expected = [[1], [2], [3]];

        self::assertSame($expected, MatrixTranspose::transpose($matrix));
    }

    public function testTransposeTwiceReturnsOriginal(): void
    {
        $matrix = [
            [1, 2, 3],
            [4, 5, 6],
        ];

        $twiceTransposed = MatrixTranspose::transpose(MatrixTranspose::transpose($matrix));

        self::assertSame($matrix, $twiceTransposed);
    }
}
