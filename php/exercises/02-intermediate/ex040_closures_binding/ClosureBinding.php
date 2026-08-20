<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex040ClosuresBinding;

/*
Exercise 040 - Closure binding (intermediate).

Goal:   Rebind a closure's $this to a different object at runtime.
Drills: Closure::bind/bindTo, rebinding $this inside a closure to a different object.
Passes: ClosureBindingTest
*/
final class ClosureBinding
{
    private function __construct()
    {
    }

    public static function bindToObject(\Closure $closure, object $newThis): \Closure
    {
        throw new \RuntimeException('TODO');
    }
}
