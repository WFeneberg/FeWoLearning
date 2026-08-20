<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex047MatchExpression;

/*
Exercise 047 - Match expression (intermediate).

Goal:   Classify integers with a match(true) pattern, and demonstrate that
        match throws UnhandledMatchError instead of silently falling through.
Drills: match expression, match(true) boolean conditions, UnhandledMatchError.
Passes: NumberClassifierTest
*/
final class NumberClassifier
{
    public static function classify(int $n): string
    {
        throw new \RuntimeException('TODO');
    }

    public static function classifyStrict(int $n): string
    {
        throw new \RuntimeException('TODO');
    }
}
