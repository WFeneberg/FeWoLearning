<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Expert\Ex091MicroRouter;

final class Router
{
    private array $routes = [];

    public function add(string $method, string $path, \Closure $handler): void
    {
        $key = strtoupper($method) . ' ' . $path;
        $this->routes[$key] = $handler;
    }

    public function dispatch(string $method, string $path): mixed
    {
        $key = strtoupper($method) . ' ' . $path;

        if (!array_key_exists($key, $this->routes)) {
            throw new \OutOfBoundsException("route not found: {$key}");
        }

        return ($this->routes[$key])();
    }
}
