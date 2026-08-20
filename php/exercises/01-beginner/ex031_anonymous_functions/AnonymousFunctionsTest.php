<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex031AnonymousFunctions;

require_once __DIR__ . '/AnonymousFunctions.php';

use PHPUnit\Framework\TestCase;

final class AnonymousFunctionsTest extends TestCase
{
    public function testMakeAdderAddsCapturedAmount(): void
    {
        $addFive = AnonymousFunctions::makeAdder(5);

        self::assertSame(15, $addFive(10));
        self::assertSame(8, $addFive(3));
    }

    public function testMakeAdderWithDifferentAmountsAreIndependent(): void
    {
        $addOne = AnonymousFunctions::makeAdder(1);
        $addTen = AnonymousFunctions::makeAdder(10);

        self::assertSame(11, $addOne(10));
        self::assertSame(20, $addTen(10));
    }

    public function testMakeCounterIncrementsSequentially(): void
    {
        $counter = AnonymousFunctions::makeCounter();

        self::assertSame(1, $counter());
        self::assertSame(2, $counter());
        self::assertSame(3, $counter());
    }

    public function testTwoSeparateCountersDoNotShareState(): void
    {
        $counterA = AnonymousFunctions::makeCounter();
        $counterB = AnonymousFunctions::makeCounter();

        self::assertSame(1, $counterA());
        self::assertSame(2, $counterA());
        self::assertSame(1, $counterB());
    }
}
