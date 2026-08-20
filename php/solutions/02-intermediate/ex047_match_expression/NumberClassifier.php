<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex047MatchExpression;

final class NumberClassifier
{
    public static function classify(int $n): string
    {
        return match (true) {
            $n < 0 => 'negative',
            $n === 0 => 'zero',
            $n % 2 === 0 => 'even',
            default => 'odd',
        };
    }

    public static function classifyStrict(int $n): string
    {
        return match ($n) {
            0 => 'zero',
            1 => 'one',
        };
    }
}
