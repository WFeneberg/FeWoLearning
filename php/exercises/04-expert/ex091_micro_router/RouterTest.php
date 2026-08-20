<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Expert\Ex091MicroRouter;

require_once __DIR__ . '/Router.php';

use PHPUnit\Framework\TestCase;

final class RouterTest extends TestCase
{
    public function testDispatchesToTheHandlerRegisteredForMethodAndPath(): void
    {
        $router = new Router();
        $router->add('GET', '/users', fn (): string => 'list-users');
        $router->add('POST', '/users', fn (): string => 'create-user');

        self::assertSame('list-users', $router->dispatch('GET', '/users'));
        self::assertSame('create-user', $router->dispatch('POST', '/users'));
    }

    public function testMethodIsCaseInsensitive(): void
    {
        $router = new Router();
        $router->add('GET', '/health', fn (): string => 'ok');

        self::assertSame('ok', $router->dispatch('get', '/health'));
    }

    public function testDifferentPathsWithSameMethodAreDistinct(): void
    {
        $router = new Router();
        $router->add('GET', '/a', fn (): string => 'a');
        $router->add('GET', '/b', fn (): string => 'b');

        self::assertSame('a', $router->dispatch('GET', '/a'));
        self::assertSame('b', $router->dispatch('GET', '/b'));
    }

    public function testDispatchingUnregisteredRouteThrows(): void
    {
        $router = new Router();
        $router->add('GET', '/users', fn (): string => 'list-users');

        $this->expectException(\OutOfBoundsException::class);

        $router->dispatch('DELETE', '/users');
    }
}
