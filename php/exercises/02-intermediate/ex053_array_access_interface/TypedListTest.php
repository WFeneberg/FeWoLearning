<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex053ArrayAccessInterface;

require_once __DIR__ . '/TypedList.php';

use PHPUnit\Framework\TestCase;

final class TypedListTest extends TestCase
{
    public function testAppendAndIndexGet(): void
    {
        $list = new TypedList();
        $list[] = 'a';
        $list[] = 'b';

        self::assertSame('a', $list[0]);
        self::assertSame('b', $list[1]);
    }

    public function testIssetReflectsPresence(): void
    {
        $list = new TypedList();
        $list[] = 'a';

        self::assertTrue(isset($list[0]));
        self::assertFalse(isset($list[5]));
    }

    public function testUnsetRemovesEntry(): void
    {
        $list = new TypedList();
        $list[] = 'a';

        self::assertTrue(isset($list[0]));
        unset($list[0]);
        self::assertFalse(isset($list[0]));
    }

    public function testExplicitOffsetSetOverwrites(): void
    {
        $list = new TypedList();
        $list[0] = 'first';
        $list[0] = 'second';

        self::assertSame('second', $list[0]);
    }
}
