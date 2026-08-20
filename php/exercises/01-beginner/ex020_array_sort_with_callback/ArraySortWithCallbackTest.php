<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex020ArraySortWithCallback;

require_once __DIR__ . '/ArraySortWithCallback.php';

use PHPUnit\Framework\TestCase;

final class ArraySortWithCallbackTest extends TestCase
{
    public function testSortsAscendingByLength(): void
    {
        $input = ['banana', 'fig', 'kiwi', 'a'];

        self::assertSame(
            ['a', 'fig', 'kiwi', 'banana'],
            ArraySortWithCallback::sortByLength($input)
        );
    }

    public function testEqualLengthStringsAreBothPresent(): void
    {
        $input = ['bb', 'aa', 'c'];

        $result = ArraySortWithCallback::sortByLength($input);

        self::assertSame('c', $result[0]);
        self::assertEqualsCanonicalizing(['bb', 'aa'], array_slice($result, 1));
    }

    public function testOriginalArrayIsNotModified(): void
    {
        $input = ['ccc', 'a', 'bb'];

        ArraySortWithCallback::sortByLength($input);

        self::assertSame(['ccc', 'a', 'bb'], $input);
    }

    public function testReturnedArrayIsReindexed(): void
    {
        $input = ['ccc', 'a', 'bb'];

        $result = ArraySortWithCallback::sortByLength($input);

        self::assertSame([0, 1, 2], array_keys($result));
    }
}
