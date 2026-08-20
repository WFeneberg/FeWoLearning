<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex061MagicTostring;

require_once __DIR__ . '/Money2.php';

use PHPUnit\Framework\TestCase;

final class Money2Test extends TestCase
{
    public function testExplicitStringCast(): void
    {
        $money = new Money2(1234, 'EUR');

        self::assertSame('12.34 EUR', (string) $money);
    }

    public function testImplicitConversionViaConcatenation(): void
    {
        $money = new Money2(500, 'USD');

        self::assertSame('Total: 5.00 USD', 'Total: ' . $money);
    }

    public function testZeroCentsFormatsWithTwoDecimals(): void
    {
        $money = new Money2(0, 'GBP');

        self::assertSame('0.00 GBP', (string) $money);
    }

    public function testCentsThatRoundUpToWholeUnit(): void
    {
        $money = new Money2(100, 'CHF');

        self::assertSame('1.00 CHF', (string) $money);
    }
}
