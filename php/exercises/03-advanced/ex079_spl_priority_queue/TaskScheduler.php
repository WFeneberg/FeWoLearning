<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex079SplPriorityQueue;

/*
Exercise 079 - Priority-ordered task draining (advanced).

Goal:   Order a list of named tasks by descending priority using SplPriorityQueue.
Drills: \SplPriorityQueue, insert()/extract(), heap-based ordering vs. insertion order.
Passes: TaskSchedulerTest
*/
final class TaskScheduler
{
    private function __construct()
    {
    }

    public static function orderByPriority(array $tasks): array
    {
        throw new \RuntimeException('TODO');
    }
}
