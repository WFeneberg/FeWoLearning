<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex082AbstractFactoryRegistry;

require_once __DIR__ . '/ShapeFactory.php';

use PHPUnit\Framework\TestCase;

final class ShapeFactoryTest extends TestCase
{
    protected function setUp(): void
    {
        ShapeFactory::reset();
    }

    protected function tearDown(): void
    {
        ShapeFactory::reset();
    }

    public function testRegisterAndCreateBuildsExpectedObject(): void
    {
        ShapeFactory::register('circle', function (float $radius): \stdClass {
            $shape = new \stdClass();
            $shape->kind = 'circle';
            $shape->radius = $radius;

            return $shape;
        });

        $circle = ShapeFactory::create('circle', 3.0);

        self::assertInstanceOf(\stdClass::class, $circle);
        self::assertSame('circle', $circle->kind);
        self::assertSame(3.0, $circle->radius);
    }

    public function testCreatePassesMultipleArgumentsThrough(): void
    {
        ShapeFactory::register('rectangle', function (float $width, float $height): \stdClass {
            $shape = new \stdClass();
            $shape->kind = 'rectangle';
            $shape->width = $width;
            $shape->height = $height;

            return $shape;
        });

        $rectangle = ShapeFactory::create('rectangle', 2.0, 5.0);

        self::assertSame(2.0, $rectangle->width);
        self::assertSame(5.0, $rectangle->height);
    }

    public function testCreateWithUnregisteredKeyThrows(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        ShapeFactory::create('triangle');
    }

    public function testResetClearsRegistry(): void
    {
        ShapeFactory::register('circle', fn (): \stdClass => new \stdClass());
        ShapeFactory::reset();

        $this->expectException(\InvalidArgumentException::class);

        ShapeFactory::create('circle');
    }
}
