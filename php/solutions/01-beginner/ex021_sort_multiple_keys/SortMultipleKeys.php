<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex021SortMultipleKeys;

final class SortMultipleKeys
{
    private function __construct()
    {
    }

    public static function sortPeopleByAgeThenName(array $people): array
    {
        $copy = $people;

        usort(
            $copy,
            static fn (array $a, array $b): int => $a['age'] <=> $b['age'] ?: $a['name'] <=> $b['name']
        );

        return $copy;
    }
}
