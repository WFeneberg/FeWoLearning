<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex039YieldFromFlatten;

require_once __DIR__ . '/Flattener.php';

use PHPUnit\Framework\TestCase;

final class FlattenerTest extends TestCase
{
    public function testFlattenDeeplyNestedArray(): void
    {
        $nested = [1, [2, [3, 4], 5], 6];

        $result = iterator_to_array(Flattener::flatten($nested), false);

        self::assertSame([1, 2, 3, 4, 5, 6], $result);
    }

    public function testFlattenAlreadyFlatArray(): void
    {
        $result = iterator_to_array(Flattener::flatten([1, 2, 3]), false);

        self::assertSame([1, 2, 3], $result);
    }

    public function testFlattenEmptyArrayYieldsNothing(): void
    {
        $result = iterator_to_array(Flattener::flatten([]), false);

        self::assertSame([], $result);
    }

    public function testFlattenHandlesMultipleNestedLevelsWithoutKeyCollisions(): void
    {
        $nested = [[1, 2], [3, 4], [5, 6]];

        $result = iterator_to_array(Flattener::flatten($nested), false);

        self::assertSame([1, 2, 3, 4, 5, 6], $result);
        self::assertCount(6, $result);
    }

    public function testFlattenSingleDeeplyNestedValue(): void
    {
        $nested = [[[[42]]]];

        $result = iterator_to_array(Flattener::flatten($nested), false);

        self::assertSame([42], $result);
    }
}
