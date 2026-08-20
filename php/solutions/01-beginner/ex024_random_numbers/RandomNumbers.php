<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex024RandomNumbers;

final class RandomNumbers
{
    private function __construct()
    {
    }

    public static function seededRoll(int $seed, int $sides): int
    {
        mt_srand($seed);

        return mt_rand(1, $sides);
    }

    public static function diceRoll(int $sides): int
    {
        return random_int(1, $sides);
    }
}
