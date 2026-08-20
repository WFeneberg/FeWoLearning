<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex076ReflectionBasics;

/*
Exercise 076 - Reflection basics (advanced).

Goal:   List the public method names of an arbitrary object via Reflection.
Drills: \ReflectionClass, \ReflectionMethod, IS_PUBLIC filter, reflection introspection.
Passes: ReflectionInspectorTest
*/
final class ReflectionInspector
{
    private function __construct()
    {
    }

    public static function publicMethodNames(object $target): array
    {
        throw new \RuntimeException('TODO');
    }
}
