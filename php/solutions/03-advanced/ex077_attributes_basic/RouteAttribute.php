<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex077AttributesBasic;

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
        $routes = [];

        foreach ((new \ReflectionClass($className))->getMethods() as $method) {
            $attributes = $method->getAttributes(Route::class);

            if ($attributes === []) {
                continue;
            }

            /** @var Route $route */
            $route = $attributes[0]->newInstance();
            $routes[$method->getName()] = $route->path;
        }

        return $routes;
    }
}
