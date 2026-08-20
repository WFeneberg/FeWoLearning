<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex040ClosuresBinding;

require_once __DIR__ . '/ClosureBinding.php';

use PHPUnit\Framework\TestCase;

final class ClosureReaderHolder
{
    public function __construct(public int $value)
    {
    }

    public function makeValueReader(): \Closure
    {
        return function () {
            return $this->value;
        };
    }
}

final class ClosureBindingTest extends TestCase
{
    public function testRebindsClosureToNewObjectsProperty(): void
    {
        $original = new ClosureReaderHolder(1);
        $closure = $original->makeValueReader();

        self::assertSame(1, $closure());

        $newTarget = new ClosureReaderHolder(42);
        $rebound = ClosureBinding::bindToObject($closure, $newTarget);

        self::assertSame(42, $rebound());
    }

    public function testOriginalClosureIsUnaffected(): void
    {
        $original = new ClosureReaderHolder(7);
        $closure = $original->makeValueReader();

        $newTarget = new ClosureReaderHolder(99);
        ClosureBinding::bindToObject($closure, $newTarget);

        self::assertSame(7, $closure());
    }

    public function testReboundClosureReflectsLaterChangesToNewTarget(): void
    {
        $original = new ClosureReaderHolder(1);
        $closure = $original->makeValueReader();

        $newTarget = new ClosureReaderHolder(10);
        $rebound = ClosureBinding::bindToObject($closure, $newTarget);

        $newTarget->value = 20;

        self::assertSame(20, $rebound());
    }

    public function testBindToObjectReturnsAClosure(): void
    {
        $original = new ClosureReaderHolder(5);
        $closure = $original->makeValueReader();
        $newTarget = new ClosureReaderHolder(6);

        $rebound = ClosureBinding::bindToObject($closure, $newTarget);

        self::assertInstanceOf(\Closure::class, $rebound);
    }
}
