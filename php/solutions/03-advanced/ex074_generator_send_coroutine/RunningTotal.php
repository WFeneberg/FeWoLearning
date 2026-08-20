<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex074GeneratorSendCoroutine;

final class RunningTotal
{
    private function __construct()
    {
    }

    public static function accumulator(): \Generator
    {
        $total = 0;

        while (true) {
            $amount = yield $total;
            $total += $amount;
        }
    }
}
