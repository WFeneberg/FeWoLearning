<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex045EnumsBasic;

/*
Exercise 045 - Pure enums and match (intermediate).

Goal:   Implement helpers that compute the opposite of a compass direction
        and list all cases, using match over a pure (non-backed) enum.
Drills: pure enum, ::cases(), match over enum cases.
Passes: DirectionTest
*/
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
        throw new \RuntimeException('TODO');
    }

    public static function all(): array
    {
        throw new \RuntimeException('TODO');
    }
}
