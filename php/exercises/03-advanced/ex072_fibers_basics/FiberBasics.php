<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex072FibersBasics;

/*
Exercise 072 - Fiber suspend/resume basics (advanced).

Goal:   Demonstrate a single Fiber's suspend/resume/return round-trip.
Drills: \Fiber, Fiber::suspend(), Fiber::start(), Fiber::resume(), Fiber::getReturn().
Passes: FiberBasicsTest
*/
final class FiberBasics
{
    private function __construct()
    {
    }

    public static function runWithSuspension(): array
    {
        throw new \RuntimeException('TODO');
    }
}
