<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex002StringFormatting;

require_once __DIR__ . '/StringFormatting.php';

use PHPUnit\Framework\TestCase;

final class StringFormattingTest extends TestCase
{
    public function testFormatCurrencyAddsThousandsSeparatorAndTwoDecimals(): void
    {
        self::assertSame('1,234.50', StringFormatting::formatCurrency(1234.5));
    }

    public function testFormatCurrencySmallAmount(): void
    {
        self::assertSame('7.00', StringFormatting::formatCurrency(7.0));
    }

    public function testFormatPercentageRoundsToRequestedDecimals(): void
    {
        self::assertSame('45.7%', StringFormatting::formatPercentage(0.4567, 1));
    }

    public function testFormatPercentageZeroDecimals(): void
    {
        self::assertSame('46%', StringFormatting::formatPercentage(0.4567, 0));
    }

    public function testPadLabelPadsWithSpacesToWidth(): void
    {
        $padded = StringFormatting::padLabel('id', 5);

        self::assertSame('id   ', $padded);
        self::assertSame(5, strlen($padded));
    }

    public function testPadLabelLongerThanWidthIsUnchanged(): void
    {
        self::assertSame('longlabel', StringFormatting::padLabel('longlabel', 3));
    }
}
