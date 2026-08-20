<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex054JsonRoundtrip;

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
        return ['x' => $this->x, 'y' => $this->y, 'z' => $this->z];
    }

    public static function fromJson(string $json): self
    {
        $decoded = json_decode($json, true, flags: JSON_THROW_ON_ERROR);

        return new self((float) $decoded['x'], (float) $decoded['y'], (float) $decoded['z']);
    }
}
