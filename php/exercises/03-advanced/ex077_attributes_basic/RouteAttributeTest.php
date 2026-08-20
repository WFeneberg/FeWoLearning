<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex077AttributesBasic;

require_once __DIR__ . '/RouteAttribute.php';

use PHPUnit\Framework\TestCase;

final class SampleController
{
    #[Route('/users')]
    public function listUsers(): void
    {
    }

    public function helper(): void
    {
    }
}

final class RouteAttributeTest extends TestCase
{
    public function testReturnsRouteForAnnotatedMethod(): void
    {
        $routes = RouteReader::methodRoutes(SampleController::class);

        self::assertSame(['listUsers' => '/users'], $routes);
    }

    public function testMethodWithoutAttributeIsExcluded(): void
    {
        $routes = RouteReader::methodRoutes(SampleController::class);

        self::assertArrayNotHasKey('helper', $routes);
    }

    public function testRouteInstanceExposesThePathValue(): void
    {
        $route = new Route('/orders');

        self::assertSame('/orders', $route->path);
    }

    public function testClassWithNoRoutesReturnsEmptyArray(): void
    {
        $plain = new class {
            public function noop(): void
            {
            }
        };

        self::assertSame([], RouteReader::methodRoutes($plain::class));
    }
}
