<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex032ArrowFunctions;

require_once __DIR__ . '/ArrowFunctions.php';

use PHPUnit\Framework\TestCase;

final class ArrowFunctionsTest extends TestCase
{
    public function testBuildMultiplierMultipliesByFactor(): void
    {
        $triple = ArrowFunctions::buildMultiplier(3);

        self::assertSame(9, $triple(3));
        self::assertSame(30, $triple(10));
    }

    public function testBuildMultiplierCapturesFactorByValue(): void
    {
        $factor = 5;
        $multiplier = ArrowFunctions::buildMultiplier($factor);

        $factor = 100;

        self::assertSame(10, $multiplier(2));
    }

    public function testDoubleEachViaArrowFn(): void
    {
        self::assertSame([2, 4, 6, 8], ArrowFunctions::doubleEachViaArrowFn([1, 2, 3, 4]));
    }

    public function testDoubleEachViaArrowFnEmptyArray(): void
    {
        self::assertSame([], ArrowFunctions::doubleEachViaArrowFn([]));
    }

    public function testDoubleEachViaArrowFnNegativeNumbers(): void
    {
        self::assertSame([-2, 0, 2], ArrowFunctions::doubleEachViaArrowFn([-1, 0, 1]));
    }
}
