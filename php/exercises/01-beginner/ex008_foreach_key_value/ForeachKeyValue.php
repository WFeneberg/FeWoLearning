<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex008ForeachKeyValue;

/*
Exercise 008 - foreach key/value and by-reference iteration (beginner).

Goal:   Implement helpers that iterate arrays by key/value and by reference.
Drills: foreach ($arr as $k => $v), reference iteration foreach (... as &$v).
Passes: ForeachKeyValueTest
*/
final class ForeachKeyValue
{
    private function __construct()
    {
    }

    public static function invertMap(array $map): array
    {
        throw new \RuntimeException('TODO');
    }

    public static function incrementAllByReference(array &$numbers): void
    {
        throw new \RuntimeException('TODO');
    }
}
