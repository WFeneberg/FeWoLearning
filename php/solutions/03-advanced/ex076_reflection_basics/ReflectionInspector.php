<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex076ReflectionBasics;

final class ReflectionInspector
{
    private function __construct()
    {
    }

    public static function publicMethodNames(object $target): array
    {
        $methods = (new \ReflectionClass($target))->getMethods(\ReflectionMethod::IS_PUBLIC);
        $names = array_map(static fn (\ReflectionMethod $method): string => $method->getName(), $methods);
        sort($names);

        return $names;
    }
}
