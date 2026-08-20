<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Expert\Ex091MicroRouter;

/*
Exercise 091 - Micro router (expert).

Goal:   Implement a minimal HTTP-method + exact-path router built from
        closures, dispatching purely in-memory (no superglobals involved).
Drills: closures, routing tables, string keys, custom runtime exceptions.
Passes: RouterTest
*/
final class Router
{
    private array $routes = [];

    public function add(string $method, string $path, \Closure $handler): void
    {
        throw new \RuntimeException('TODO');
    }

    public function dispatch(string $method, string $path): mixed
    {
        throw new \RuntimeException('TODO');
    }
}
