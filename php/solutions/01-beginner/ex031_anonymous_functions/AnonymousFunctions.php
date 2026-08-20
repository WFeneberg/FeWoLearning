<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex031AnonymousFunctions;

final class AnonymousFunctions
{
    private function __construct()
    {
    }

    public static function makeAdder(int $amount): \Closure
    {
        return function (int $n) use ($amount): int {
            return $n + $amount;
        };
    }

    public static function makeCounter(): \Closure
    {
        $count = 0;

        return function () use (&$count): int {
            $count++;

            return $count;
        };
    }
}
