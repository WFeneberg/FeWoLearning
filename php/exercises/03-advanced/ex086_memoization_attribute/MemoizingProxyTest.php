<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex086MemoizationAttribute;

require_once __DIR__ . '/Memoizable.php';

use PHPUnit\Framework\TestCase;

final class MemoizableCalculatorFixture
{
    public int $callCount = 0;

    #[Memoize]
    public function square(int $n): int
    {
        $this->callCount++;

        return $n * $n;
    }

    public function cube(int $n): int
    {
        $this->callCount++;

        return $n * $n * $n;
    }
}

final class MemoizingProxyTest extends TestCase
{
    public function testMarkedMethodIsCachedOnRepeatedCall(): void
    {
        $calculator = new MemoizableCalculatorFixture();
        $memoizedSquare = MemoizingProxy::wrap($calculator, 'square');

        $first = $memoizedSquare(5);
        $second = $memoizedSquare(5);

        self::assertSame(25, $first);
        self::assertSame(25, $second);
        self::assertSame(1, $calculator->callCount);
    }

    public function testDifferentArgumentsAreCachedSeparately(): void
    {
        $calculator = new MemoizableCalculatorFixture();
        $memoizedSquare = MemoizingProxy::wrap($calculator, 'square');

        $memoizedSquare(2);
        $memoizedSquare(3);
        $memoizedSquare(2);

        self::assertSame(2, $calculator->callCount);
    }

    public function testWrappingUnmarkedMethodThrows(): void
    {
        $calculator = new MemoizableCalculatorFixture();

        $this->expectException(\InvalidArgumentException::class);

        MemoizingProxy::wrap($calculator, 'cube');
    }
}
