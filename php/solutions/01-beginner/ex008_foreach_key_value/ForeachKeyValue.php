<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex008ForeachKeyValue;

final class ForeachKeyValue
{
    private function __construct()
    {
    }

    public static function invertMap(array $map): array
    {
        $result = [];

        foreach ($map as $k => $v) {
            $result[$v] = $k;
        }

        return $result;
    }

    public static function incrementAllByReference(array &$numbers): void
    {
        foreach ($numbers as &$n) {
            $n++;
        }
        unset($n);
    }
}
