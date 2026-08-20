<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex074GeneratorSendCoroutine;

/*
Exercise 074 - Generator::send() coroutine (advanced).

Goal:   Implement a stateful running-total coroutine driven by Generator::send().
Drills: \Generator, yield as an expression, Generator::send(), Generator::current().
Passes: RunningTotalTest
*/
final class RunningTotal
{
    private function __construct()
    {
    }

    public static function accumulator(): \Generator
    {
        throw new \RuntimeException('TODO');
    }
}
