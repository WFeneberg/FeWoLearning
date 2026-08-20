<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex084SingledispatchMatch;

/*
Exercise 084 - Single-dispatch via match (advanced).

Goal:   Implement type-based dispatch using match(true) combined with
        is_*/instanceof checks, in the spirit of a single-dispatch function.
Drills: match(true), instanceof, get_debug_type, type-based dispatch.
Passes: TypeDescriberTest
*/
final class TypeDescriber
{
    public static function describe(mixed $value): string
    {
        throw new \RuntimeException('TODO');
    }
}
