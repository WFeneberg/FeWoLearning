<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex025NestedArrayAccess;

/*
Exercise 025 - Nested array access (beginner).

Goal:   Safely walk a nested array along a sequence of keys.
Drills: traversing nested arrays safely, path-walking helper.
Passes: NestedArrayAccessTest
*/
final class NestedArrayAccess
{
    private function __construct()
    {
    }

    public static function getNested(array $data, array $path, mixed $default = null): mixed
    {
        throw new \RuntimeException('TODO');
    }
}
