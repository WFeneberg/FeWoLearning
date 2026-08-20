<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex024RandomNumbers;

/*
Exercise 024 - Random numbers (beginner).

Goal:   Implement a deterministic seeded roll and a real random dice roll.
Drills: mt_srand seeding for deterministic tests, random_int for real randomness.
Passes: RandomNumbersTest
*/
final class RandomNumbers
{
    private function __construct()
    {
    }

    public static function seededRoll(int $seed, int $sides): int
    {
        throw new \RuntimeException('TODO');
    }

    public static function diceRoll(int $sides): int
    {
        throw new \RuntimeException('TODO');
    }
}
