<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex028ArrayRotation;

require_once __DIR__ . '/ArrayRotation.php';

use PHPUnit\Framework\TestCase;

final class ArrayRotationTest extends TestCase
{
    public function testRotateLeftByOne(): void
    {
        self::assertSame(['b', 'c', 'd', 'a'], ArrayRotation::rotateLeft(['a', 'b', 'c', 'd'], 1));
    }

    public function testRotateLeftByZeroIsIdentity(): void
    {
        self::assertSame(['a', 'b', 'c', 'd'], ArrayRotation::rotateLeft(['a', 'b', 'c', 'd'], 0));
    }

    public function testRotateLeftWrapsWhenPositionsExceedCount(): void
    {
        self::assertSame(
            ['b', 'c', 'd', 'a'],
            ArrayRotation::rotateLeft(['a', 'b', 'c', 'd'], 5)
        );
    }

    public function testRotateLeftDoesNotMutateOriginalArray(): void
    {
        $original = ['a', 'b', 'c', 'd'];
        ArrayRotation::rotateLeft($original, 2);

        self::assertSame(['a', 'b', 'c', 'd'], $original);
    }

    public function testRotateLeftByFullCountIsIdentity(): void
    {
        self::assertSame(['a', 'b', 'c', 'd'], ArrayRotation::rotateLeft(['a', 'b', 'c', 'd'], 4));
    }
}
