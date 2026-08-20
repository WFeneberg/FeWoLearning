<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex045EnumsBasic;

enum Direction
{
    case North;
    case East;
    case South;
    case West;
}

final class Directions
{
    public static function opposite(Direction $d): Direction
    {
        return match ($d) {
            Direction::North => Direction::South,
            Direction::South => Direction::North,
            Direction::East => Direction::West,
            Direction::West => Direction::East,
        };
    }

    public static function all(): array
    {
        return Direction::cases();
    }
}
