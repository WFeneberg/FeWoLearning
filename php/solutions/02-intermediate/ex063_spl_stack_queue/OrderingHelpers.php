<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex063SplStackQueue;

final class OrderingHelpers
{
    public static function reverseWithStack(array $items): array
    {
        $stack = new \SplStack();

        foreach ($items as $item) {
            $stack->push($item);
        }

        $result = [];
        while (!$stack->isEmpty()) {
            $result[] = $stack->pop();
        }

        return $result;
    }

    public static function passThroughQueue(array $items): array
    {
        $queue = new \SplQueue();

        foreach ($items as $item) {
            $queue->enqueue($item);
        }

        $result = [];
        while (!$queue->isEmpty()) {
            $result[] = $queue->dequeue();
        }

        return $result;
    }
}
