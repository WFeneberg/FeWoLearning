<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex054JsonRoundtrip;

/*
Exercise 054 - JSON round-trip (intermediate).

Goal:   Implement JsonSerializable-based encoding and a matching fromJson factory.
Drills: json_encode, json_decode, JsonSerializable interface, JSON_THROW_ON_ERROR.
Passes: Point3DTest
*/
final class Point3D implements \JsonSerializable
{
    public function __construct(
        public readonly float $x,
        public readonly float $y,
        public readonly float $z,
    ) {
    }

    public function jsonSerialize(): array
    {
        throw new \RuntimeException('TODO');
    }

    public static function fromJson(string $json): self
    {
        throw new \RuntimeException('TODO');
    }
}
