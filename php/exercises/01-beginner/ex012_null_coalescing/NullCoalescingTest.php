<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex012NullCoalescing;

require_once __DIR__ . '/NullCoalescing.php';

use PHPUnit\Framework\TestCase;

final class NullCoalescingTest extends TestCase
{
    public function testFirstNonNullReturnsFirstSetValue(): void
    {
        self::assertSame('b', NullCoalescing::firstNonNull(null, 'b', 'c'));
    }

    public function testFirstNonNullSkipsLeadingNulls(): void
    {
        self::assertSame(3, NullCoalescing::firstNonNull(null, null, 3, 4));
    }

    public function testFirstNonNullReturnsNullWhenAllNull(): void
    {
        self::assertNull(NullCoalescing::firstNonNull(null, null, null));
    }

    public function testFirstNonNullReturnsNullWhenEmpty(): void
    {
        self::assertNull(NullCoalescing::firstNonNull());
    }

    public function testConfigWithDefaultAppliesDefaultOnlyOnce(): void
    {
        $config = [];

        $first = NullCoalescing::configWithDefault($config, 'timeout', 30);
        self::assertSame(30, $first);
        self::assertSame(30, $config['timeout']);

        $second = NullCoalescing::configWithDefault($config, 'timeout', 99);
        self::assertSame(30, $second);
        self::assertSame(30, $config['timeout']);
    }
}
