<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex045EnumsBasic;

require_once __DIR__ . '/Direction.php';

use PHPUnit\Framework\TestCase;

final class DirectionTest extends TestCase
{
    public function testOppositeOfNorthIsSouth(): void
    {
        self::assertSame(Direction::South, Directions::opposite(Direction::North));
    }

    public function testOppositeOfEastIsWest(): void
    {
        self::assertSame(Direction::West, Directions::opposite(Direction::East));
    }

    public function testOppositeIsSymmetric(): void
    {
        $opposite = Directions::opposite(Direction::South);

        self::assertSame(Direction::North, $opposite);
        self::assertSame(Direction::South, Directions::opposite($opposite));
    }

    public function testAllReturnsExactlyFourCases(): void
    {
        self::assertCount(4, Directions::all());
    }

    public function testAllContainsEveryDirection(): void
    {
        $all = Directions::all();

        self::assertContains(Direction::North, $all);
        self::assertContains(Direction::East, $all);
        self::assertContains(Direction::South, $all);
        self::assertContains(Direction::West, $all);
    }
}
