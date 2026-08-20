<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex062MagicInvoke;

require_once __DIR__ . '/Multiplier.php';

use PHPUnit\Framework\TestCase;

final class MultiplierTest extends TestCase
{
    public function testInvokeDirectly(): void
    {
        $multiplier = new Multiplier(3);

        self::assertSame(15, $multiplier(5));
    }

    public function testIsCallable(): void
    {
        $multiplier = new Multiplier(2);

        self::assertTrue(is_callable($multiplier));
    }

    public function testArrayMapWithCallableObject(): void
    {
        $multiplier = new Multiplier(10);

        self::assertSame([10, 20, 30], array_map($multiplier, [1, 2, 3]));
    }

    public function testZeroFactorAlwaysReturnsZero(): void
    {
        $multiplier = new Multiplier(0);

        self::assertSame(0, $multiplier(999));
    }
}
