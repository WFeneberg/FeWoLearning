<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex040ClosuresBinding;

final class ClosureBinding
{
    private function __construct()
    {
    }

    public static function bindToObject(\Closure $closure, object $newThis): \Closure
    {
        return \Closure::bind($closure, $newThis, $newThis::class);
    }
}
