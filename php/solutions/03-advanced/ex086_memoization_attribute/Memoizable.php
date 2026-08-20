<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex086MemoizationAttribute;

#[\Attribute(\Attribute::TARGET_METHOD)]
final class Memoize
{
}

final class MemoizingProxy
{
    public static function wrap(object $target, string $methodName): \Closure
    {
        $reflectionMethod = new \ReflectionMethod($target, $methodName);

        if (count($reflectionMethod->getAttributes(Memoize::class)) === 0) {
            throw new \InvalidArgumentException(
                "method '{$methodName}' is not marked with #[Memoize]"
            );
        }

        $cache = [];

        return function (mixed ...$args) use (&$cache, $target, $methodName): mixed {
            $key = serialize($args);

            if (!array_key_exists($key, $cache)) {
                $cache[$key] = $target->$methodName(...$args);
            }

            return $cache[$key];
        };
    }
}
