<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex031AnonymousFunctions;

/*
Exercise 031 - Anonymous functions (beginner).

Goal:   Build closures that capture variables by value and by reference.
Drills: closures, the use() clause (by value vs by reference use (&$x)).
Passes: AnonymousFunctionsTest
*/
final class AnonymousFunctions
{
    private function __construct()
    {
    }

    public static function makeAdder(int $amount): \Closure
    {
        throw new \RuntimeException('TODO');
    }

    public static function makeCounter(): \Closure
    {
        throw new \RuntimeException('TODO');
    }
}
