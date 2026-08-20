<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex063SplStackQueue;

require_once __DIR__ . '/OrderingHelpers.php';

use PHPUnit\Framework\TestCase;

final class OrderingHelpersTest extends TestCase
{
    public function testReverseWithStackReversesOrder(): void
    {
        self::assertSame([4, 3, 2, 1], OrderingHelpers::reverseWithStack([1, 2, 3, 4]));
    }

    public function testPassThroughQueuePreservesOrder(): void
    {
        self::assertSame([1, 2, 3, 4], OrderingHelpers::passThroughQueue([1, 2, 3, 4]));
    }

    public function testStackAndQueueDifferOnSameInput(): void
    {
        $items = ['a', 'b', 'c'];

        self::assertNotSame(
            OrderingHelpers::reverseWithStack($items),
            OrderingHelpers::passThroughQueue($items)
        );
    }

    public function testReverseWithStackOnSingleElement(): void
    {
        self::assertSame(['only'], OrderingHelpers::reverseWithStack(['only']));
    }

    public function testPassThroughQueueOnEmptyArray(): void
    {
        self::assertSame([], OrderingHelpers::passThroughQueue([]));
    }
}
