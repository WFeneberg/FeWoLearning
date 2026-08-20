<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex073FibersCooperativeScheduler;

/*
Exercise 073 - Cooperative Fiber scheduler (advanced).

Goal:   Drive multiple Fibers to completion via a simple round-robin loop.
Drills: \Fiber, cooperative scheduling, Fiber::isTerminated(), Fiber::getReturn().
Passes: FiberSchedulerTest
*/
final class FiberScheduler
{
    private function __construct()
    {
    }

    public static function runAll(array $fiberBodies): array
    {
        throw new \RuntimeException('TODO');
    }
}
