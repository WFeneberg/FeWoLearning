<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex067ComparableSorting;

require_once __DIR__ . '/Version.php';

use PHPUnit\Framework\TestCase;

final class VersionTest extends TestCase
{
    public function testSortOrdersByMajorMinorPatch(): void
    {
        $versions = [
            new Version(1, 2, 0),
            new Version(1, 0, 5),
            new Version(2, 0, 0),
            new Version(1, 2, 3),
        ];

        $sorted = Versions::sort($versions);

        self::assertSame(
            ['1.0.5', '1.2.0', '1.2.3', '2.0.0'],
            array_map(static fn (Version $v): string => "{$v->major}.{$v->minor}.{$v->patch}", $sorted)
        );
    }

    public function testCompareToTiebreaksOnMinorWhenMajorEqual(): void
    {
        $lower = new Version(3, 1, 9);
        $higher = new Version(3, 2, 0);

        self::assertSame(-1, $lower->compareTo($higher));
        self::assertSame(1, $higher->compareTo($lower));
    }

    public function testCompareToEqualVersionsReturnsZero(): void
    {
        $a = new Version(1, 0, 0);
        $b = new Version(1, 0, 0);

        self::assertSame(0, $a->compareTo($b));
    }

    public function testSortDoesNotMutateOriginalArray(): void
    {
        $versions = [new Version(2, 0, 0), new Version(1, 0, 0)];
        $original = $versions;

        Versions::sort($versions);

        self::assertSame($original, $versions);
    }
}
