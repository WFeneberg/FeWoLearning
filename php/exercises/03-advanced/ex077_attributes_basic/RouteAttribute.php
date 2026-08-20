<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex077AttributesBasic;

/*
Exercise 077 - Custom PHP attributes (advanced).

Goal:   Declare a Route attribute and read it back off a class's methods.
Drills: #[Attribute], ReflectionMethod::getAttributes(), ReflectionAttribute::newInstance().
Passes: RouteAttributeTest
*/
#[\Attribute(\Attribute::TARGET_METHOD)]
final class Route
{
    public function __construct(public readonly string $path)
    {
    }
}

final class RouteReader
{
    private function __construct()
    {
    }

    public static function methodRoutes(string $className): array
    {
        throw new \RuntimeException('TODO');
    }
}
