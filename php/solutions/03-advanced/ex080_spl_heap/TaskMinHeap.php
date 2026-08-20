<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex080SplHeap;

final class TaskMinHeap extends \SplHeap
{
    protected function compare(mixed $value1, mixed $value2): int
    {
        return $value2['deadline'] <=> $value1['deadline'];
    }
}
