<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex030StringPadding;

require_once __DIR__ . '/StringPadding.php';

use PHPUnit\Framework\TestCase;

final class StringPaddingTest extends TestCase
{
    public function testPadLeftZero(): void
    {
        self::assertSame('007', StringPadding::padLeftZero(7, 3));
        self::assertSame('042', StringPadding::padLeftZero(42, 3));
    }

    public function testPadLeftZeroIsNoOpWhenNumberWiderThanWidth(): void
    {
        self::assertSame('12345', StringPadding::padLeftZero(12345, 3));
    }

    public function testCenterText(): void
    {
        self::assertSame('  hi  ', StringPadding::centerText('hi', 6));
    }

    public function testCenterTextIsNoOpWhenTextWiderThanWidth(): void
    {
        self::assertSame('toolong', StringPadding::centerText('toolong', 3));
    }

    public function testRepeatSeparator(): void
    {
        self::assertSame('----', StringPadding::repeatSeparator('-', 4));
        self::assertSame('', StringPadding::repeatSeparator('-', 0));
    }
}
