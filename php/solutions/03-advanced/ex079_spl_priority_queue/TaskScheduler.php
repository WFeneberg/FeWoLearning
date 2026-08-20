<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex079SplPriorityQueue;

final class TaskScheduler
{
    private function __construct()
    {
    }

    public static function orderByPriority(array $tasks): array
    {
        $queue = new \SplPriorityQueue();

        foreach ($tasks as $task) {
            $queue->insert($task['name'], $task['priority']);
        }

        $ordered = [];

        while (!$queue->isEmpty()) {
            $ordered[] = $queue->extract();
        }

        return $ordered;
    }
}
