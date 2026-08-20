<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex074GeneratorSendCoroutine;

require_once __DIR__ . '/RunningTotal.php';

use PHPUnit\Framework\TestCase;

final class RunningTotalTest extends TestCase
{
    public function testCurrentStartsAtZero(): void
    {
        $generator = RunningTotal::accumulator();

        self::assertSame(0, $generator->current());
    }

    public function testSendAccumulatesAcrossCalls(): void
    {
        $generator = RunningTotal::accumulator();

        self::assertSame(0, $generator->current());
        self::assertSame(5, $generator->send(5));
        self::assertSame(8, $generator->send(3));
    }

    public function testNegativeAmountsSubtractFromTotal(): void
    {
        $generator = RunningTotal::accumulator();
        $generator->send(10);

        self::assertSame(4, $generator->send(-6));
    }

    public function testEachGeneratorInstanceHasIndependentState(): void
    {
        $first = RunningTotal::accumulator();
        $second = RunningTotal::accumulator();

        $first->send(100);

        self::assertSame(100, $first->current());
        self::assertSame(0, $second->current());
    }
}
