<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex033StaticPropertiesMethods;

require_once __DIR__ . '/InstanceCounter.php';

use PHPUnit\Framework\TestCase;

final class InstanceCounterTest extends TestCase
{
    protected function setUp(): void
    {
        InstanceCounter::reset();
    }

    public function testCurrentIsZeroInitially(): void
    {
        self::assertSame(0, InstanceCounter::current());
    }

    public function testCreatingInstancesIncrementsCount(): void
    {
        new InstanceCounter();
        new InstanceCounter();
        new InstanceCounter();

        self::assertSame(3, InstanceCounter::current());
    }

    public function testResetBringsCountBackToZero(): void
    {
        new InstanceCounter();
        new InstanceCounter();

        InstanceCounter::reset();

        self::assertSame(0, InstanceCounter::current());
    }

    public function testCountIsSharedAcrossAllInstances(): void
    {
        $first = new InstanceCounter();
        self::assertSame(1, InstanceCounter::current());

        $second = new InstanceCounter();
        self::assertSame(2, InstanceCounter::current());
    }
}
