<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex050TypedProperties;

require_once __DIR__ . '/Money.php';

use PHPUnit\Framework\TestCase;

final class MoneyTest extends TestCase
{
    public function testAddRawIntCents(): void
    {
        $money = new Money(500, 'EUR');
        $result = $money->add(250);

        self::assertSame(750, $result->cents());
    }

    public function testAddSameCurrencyMoney(): void
    {
        $money = new Money(500, 'EUR');
        $other = new Money(300, 'EUR');
        $result = $money->add($other);

        self::assertSame(800, $result->cents());
    }

    public function testAddReturnsNewInstance(): void
    {
        $money = new Money(500, 'EUR');
        $result = $money->add(100);

        self::assertNotSame($money, $result);
        self::assertSame(500, $money->cents());
    }

    public function testAddDifferentCurrencyThrows(): void
    {
        $money = new Money(500, 'EUR');
        $other = new Money(300, 'USD');

        $this->expectException(\InvalidArgumentException::class);

        $money->add($other);
    }
}
