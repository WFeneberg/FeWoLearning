<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex021SortMultipleKeys;

/*
Exercise 021 - Sorting by multiple keys (beginner).

Goal:   Sort a list of people by age ascending, then by name ascending for ties.
Drills: usort with a multi-field comparator over an array of associative arrays.
Passes: SortMultipleKeysTest
*/
final class SortMultipleKeys
{
    private function __construct()
    {
    }

    public static function sortPeopleByAgeThenName(array $people): array
    {
        throw new \RuntimeException('TODO');
    }
}
