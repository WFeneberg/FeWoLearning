<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex065ArrayWalkRecursive;

final class DeepStringUppercaser
{
    public static function uppercaseAllStrings(array $data): array
    {
        $copy = $data;

        array_walk_recursive($copy, function (&$value): void {
            if (is_string($value)) {
                $value = strtoupper($value);
            }
        });

        return $copy;
    }
}
