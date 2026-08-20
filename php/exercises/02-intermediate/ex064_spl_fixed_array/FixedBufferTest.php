<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex064SplFixedArray;

require_once __DIR__ . '/FixedBuffer.php';

use PHPUnit\Framework\TestCase;

final class FixedBufferTest extends TestCase
{
    public function testBuildFilledHasExactSize(): void
    {
        $result = FixedBuffer::buildFilled(5, 'x');

        self::assertCount(5, $result);
    }

    public function testBuildFilledAllElementsEqualFillValue(): void
    {
        $result = FixedBuffer::buildFilled(4, 7);

        self::assertSame([7, 7, 7, 7], $result);
    }

    public function testBuildFilledWithZeroSizeIsEmpty(): void
    {
        self::assertSame([], FixedBuffer::buildFilled(0, 'anything'));
    }

    public function testBuildFilledReturnsPlainArray(): void
    {
        $result = FixedBuffer::buildFilled(3, null);

        self::assertIsArray($result);
        self::assertSame([null, null, null], $result);
    }
}
