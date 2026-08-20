<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex020ArraySortWithCallback;

/*
Exercise 020 - Sorting an array with a comparator callback (beginner).

Goal:   Sort an array of strings by length, ascending, without mutating the input.
Drills: usort, the spaceship operator <=>.
Passes: ArraySortWithCallbackTest
*/
final class ArraySortWithCallback
{
    private function __construct()
    {
    }

    public static function sortByLength(array $strings): array
    {
        throw new \RuntimeException('TODO');
    }
}
