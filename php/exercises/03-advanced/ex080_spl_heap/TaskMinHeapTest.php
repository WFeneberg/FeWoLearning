<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex080SplHeap;

require_once __DIR__ . '/TaskMinHeap.php';

use PHPUnit\Framework\TestCase;

final class TaskMinHeapTest extends TestCase
{
    public function testExtractsInAscendingDeadlineOrder(): void
    {
        $heap = new TaskMinHeap();
        $heap->insert(['name' => 'c', 'deadline' => 30]);
        $heap->insert(['name' => 'a', 'deadline' => 10]);
        $heap->insert(['name' => 'b', 'deadline' => 20]);

        $order = [];

        while (!$heap->isEmpty()) {
            $order[] = $heap->extract()['name'];
        }

        self::assertSame(['a', 'b', 'c'], $order);
    }

    public function testTopReflectsTheSmallestDeadlineWithoutRemoving(): void
    {
        $heap = new TaskMinHeap();
        $heap->insert(['name' => 'later', 'deadline' => 99]);
        $heap->insert(['name' => 'soonest', 'deadline' => 1]);

        self::assertSame('soonest', $heap->top()['name']);
        self::assertCount(2, $heap);
    }

    public function testWouldFailUnderDefaultMaxHeapBehavior(): void
    {
        $heap = new TaskMinHeap();
        $heap->insert(['name' => 'small', 'deadline' => 5]);
        $heap->insert(['name' => 'big', 'deadline' => 500]);

        // A naive/default max-heap compare() would extract 'big' first.
        self::assertSame('small', $heap->extract()['name']);
    }

    public function testSingleElementHeap(): void
    {
        $heap = new TaskMinHeap();
        $heap->insert(['name' => 'only', 'deadline' => 7]);

        self::assertSame('only', $heap->extract()['name']);
        self::assertTrue($heap->isEmpty());
    }
}
