<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex080SplHeap;

/*
Exercise 080 - Min-heap ordering via SplHeap::compare() (advanced).

Goal:   Extend SplHeap so tasks extract in ascending-deadline (min-first) order.
Drills: \SplHeap, compare() contract (max-heap by default), heap inversion for min-first.
Passes: TaskMinHeapTest
*/
final class TaskMinHeap extends \SplHeap
{
    protected function compare(mixed $value1, mixed $value2): int
    {
        throw new \RuntimeException('TODO');
    }
}
