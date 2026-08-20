<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex006AssociativeArrays;

require_once __DIR__ . '/AssociativeArrays.php';

use PHPUnit\Framework\TestCase;

final class AssociativeArraysTest extends TestCase
{
    public function testGetOrDefaultReturnsExistingValue(): void
    {
        self::assertSame('bob', AssociativeArrays::getOrDefault(['name' => 'bob'], 'name', 'anon'));
    }

    public function testGetOrDefaultReturnsDefaultForMissingKey(): void
    {
        self::assertSame('anon', AssociativeArrays::getOrDefault(['name' => 'bob'], 'missing', 'anon'));
    }

    public function testGetOrDefaultReturnsStoredNullRatherThanDefault(): void
    {
        // A naive implementation using `$map[$key] ?? $default` would incorrectly
        // return $default here, since `??` treats an existing null the same as a
        // missing key.
        $result = AssociativeArrays::getOrDefault(['value' => null], 'value', 'fallback');

        self::assertNull($result);
    }

    public function testHasKeyEvenIfNullAndHasNonNullValueDiffer(): void
    {
        $map = ['present' => null];

        self::assertTrue(AssociativeArrays::hasKeyEvenIfNull($map, 'present'));
        self::assertFalse(AssociativeArrays::hasNonNullValue($map, 'present'));
    }

    public function testHasKeyEvenIfNullFalseForTrulyMissingKey(): void
    {
        self::assertFalse(AssociativeArrays::hasKeyEvenIfNull([], 'missing'));
    }

    public function testHasNonNullValueTrueForRealValue(): void
    {
        self::assertTrue(AssociativeArrays::hasNonNullValue(['count' => 0], 'count'));
    }
}
