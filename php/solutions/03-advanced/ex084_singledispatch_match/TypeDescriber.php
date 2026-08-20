<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex084SingledispatchMatch;

final class TypeDescriber
{
    public static function describe(mixed $value): string
    {
        return match (true) {
            is_int($value) => 'integer: ' . $value,
            is_string($value) => 'string: ' . $value,
            is_array($value) => 'array of ' . count($value),
            $value instanceof \DateTimeInterface => 'date: ' . $value->format('Y-m-d'),
            default => 'unknown: ' . get_debug_type($value),
        };
    }
}
