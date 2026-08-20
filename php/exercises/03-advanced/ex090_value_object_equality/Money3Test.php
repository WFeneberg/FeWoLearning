<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex090ValueObjectEquality;

require_once __DIR__ . '/Money3.php';

use PHPUnit\Framework\TestCase;

final class Money3Test extends TestCase
{
    public function testSeparateInstancesAreNotIdenticalButEqual(): void
    {
        $a = new Money3(500, 'EUR');
        $b = new Money3(500, 'EUR');

        self::assertFalse($a === $b);
        self::assertTrue($a->equals($b));
    }

    public function testPhpEqualityOperatorAlsoConsidersThemStructurallyEqual(): void
    {
        $a = new Money3(500, 'EUR');
        $b = new Money3(500, 'EUR');

        self::assertTrue($a == $b);
    }

    public function testEqualsReturnsFalseForDifferentCurrency(): void
    {
        $a = new Money3(500, 'EUR');
        $b = new Money3(500, 'USD');

        self::assertFalse($a->equals($b));
    }

    public function testAddCombinesAmountsAndLeavesOriginalsUnchanged(): void
    {
        $a = new Money3(300, 'EUR');
        $b = new Money3(200, 'EUR');

        $sum = $a->add($b);

        self::assertSame(500, $sum->cents);
        self::assertSame('EUR', $sum->currency);
        self::assertSame(300, $a->cents);
        self::assertSame(200, $b->cents);
    }

    public function testAddWithMismatchedCurrencyThrows(): void
    {
        $a = new Money3(300, 'EUR');
        $b = new Money3(200, 'USD');

        $this->expectException(\InvalidArgumentException::class);

        $a->add($b);
    }
}
