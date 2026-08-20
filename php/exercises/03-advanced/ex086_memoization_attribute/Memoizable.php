<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex086MemoizationAttribute;

/*
Exercise 086 - Memoization attribute (advanced).

Goal:   Implement a caching proxy that only wraps methods explicitly opted in
        via a #[Memoize] marker attribute, enforced through reflection.
Drills: attributes, reflection, closures, caching by serialized arguments.
Passes: MemoizingProxyTest
*/
#[\Attribute(\Attribute::TARGET_METHOD)]
final class Memoize
{
}

final class MemoizingProxy
{
    public static function wrap(object $target, string $methodName): \Closure
    {
        throw new \RuntimeException('TODO');
    }
}
