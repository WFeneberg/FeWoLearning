<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex054JsonRoundtrip;

require_once __DIR__ . '/Point3D.php';

use PHPUnit\Framework\TestCase;

final class Point3DTest extends TestCase
{
    public function testJsonEncodeProducesExpectedFields(): void
    {
        $point = new Point3D(1.5, 2.5, 3.5);

        $decoded = json_decode(json_encode($point), true);

        self::assertSame(1.5, $decoded['x']);
        self::assertSame(2.5, $decoded['y']);
        self::assertSame(3.5, $decoded['z']);
    }

    public function testFromJsonRoundTrips(): void
    {
        $point = new Point3D(-4.0, 0.0, 9.25);

        $restored = Point3D::fromJson(json_encode($point));

        self::assertSame($point->x, $restored->x);
        self::assertSame($point->y, $restored->y);
        self::assertSame($point->z, $restored->z);
    }

    public function testFromJsonParsesRawJsonString(): void
    {
        $restored = Point3D::fromJson('{"x": 1.0, "y": 2.0, "z": 3.0}');

        self::assertSame(1.0, $restored->x);
        self::assertSame(2.0, $restored->y);
        self::assertSame(3.0, $restored->z);
    }

    public function testFromJsonThrowsOnMalformedJson(): void
    {
        $this->expectException(\JsonException::class);

        Point3D::fromJson('{not valid json');
    }
}
