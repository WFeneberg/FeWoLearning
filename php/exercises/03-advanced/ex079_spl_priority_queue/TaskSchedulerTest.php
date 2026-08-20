<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex079SplPriorityQueue;

require_once __DIR__ . '/TaskScheduler.php';

use PHPUnit\Framework\TestCase;

final class TaskSchedulerTest extends TestCase
{
    public function testOrdersStrictlyByDescendingPriorityNotInsertionOrder(): void
    {
        $tasks = [
            ['name' => 'low', 'priority' => 1],
            ['name' => 'high', 'priority' => 10],
            ['name' => 'medium', 'priority' => 5],
        ];

        $ordered = TaskScheduler::orderByPriority($tasks);

        self::assertSame(['high', 'medium', 'low'], $ordered);
    }

    public function testSingleTaskReturnsItself(): void
    {
        $ordered = TaskScheduler::orderByPriority([
            ['name' => 'only', 'priority' => 42],
        ]);

        self::assertSame(['only'], $ordered);
    }

    public function testEmptyListReturnsEmptyArray(): void
    {
        self::assertSame([], TaskScheduler::orderByPriority([]));
    }

    public function testNegativePrioritiesSortCorrectly(): void
    {
        $tasks = [
            ['name' => 'urgent', 'priority' => 100],
            ['name' => 'background', 'priority' => -5],
            ['name' => 'normal', 'priority' => 0],
        ];

        $ordered = TaskScheduler::orderByPriority($tasks);

        self::assertSame(['urgent', 'normal', 'background'], $ordered);
    }
}
