<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex037GeneratorsBasics;

final class Sequences
{
    private function __construct()
    {
    }

    public static function fibonacci(int $count): \Generator
    {
        $previous = 0;
        $current = 1;

        for ($i = 0; $i < $count; $i++) {
            yield $previous;

            [$previous, $current] = [$current, $previous + $current];
        }
    }
}
