<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex021SortMultipleKeys;

require_once __DIR__ . '/SortMultipleKeys.php';

use PHPUnit\Framework\TestCase;

final class SortMultipleKeysTest extends TestCase
{
    public function testSortsByAgeAscending(): void
    {
        $people = [
            ['name' => 'Carol', 'age' => 40],
            ['name' => 'Alice', 'age' => 25],
            ['name' => 'Bob', 'age' => 30],
        ];

        $sorted = SortMultipleKeys::sortPeopleByAgeThenName($people);

        self::assertSame(['Alice', 'Bob', 'Carol'], array_column($sorted, 'name'));
    }

    public function testTiesAreBrokenByNameAscending(): void
    {
        $people = [
            ['name' => 'Zoe', 'age' => 30],
            ['name' => 'Amy', 'age' => 30],
            ['name' => 'Max', 'age' => 30],
        ];

        $sorted = SortMultipleKeys::sortPeopleByAgeThenName($people);

        self::assertSame(['Amy', 'Max', 'Zoe'], array_column($sorted, 'name'));
    }

    public function testMixedAgesWithATieGroup(): void
    {
        $people = [
            ['name' => 'Dan', 'age' => 50],
            ['name' => 'Eve', 'age' => 20],
            ['name' => 'Zed', 'age' => 20],
        ];

        $sorted = SortMultipleKeys::sortPeopleByAgeThenName($people);

        self::assertSame(['Eve', 'Zed', 'Dan'], array_column($sorted, 'name'));
    }

    public function testOriginalArrayIsNotModified(): void
    {
        $people = [
            ['name' => 'Bob', 'age' => 30],
            ['name' => 'Alice', 'age' => 25],
        ];

        SortMultipleKeys::sortPeopleByAgeThenName($people);

        self::assertSame(['Bob', 'Alice'], array_column($people, 'name'));
    }
}
