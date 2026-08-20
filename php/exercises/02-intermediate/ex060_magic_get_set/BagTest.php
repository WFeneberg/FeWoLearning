<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex060MagicGetSet;

require_once __DIR__ . '/Bag.php';

use PHPUnit\Framework\TestCase;

final class BagTest extends TestCase
{
    public function testSetThenGetReturnsSameValue(): void
    {
        $bag = new Bag();
        $bag->foo = 'bar';

        self::assertSame('bar', $bag->foo);
    }

    public function testIssetIsTrueForSetProperty(): void
    {
        $bag = new Bag();
        $bag->foo = 'bar';

        self::assertTrue(isset($bag->foo));
    }

    public function testIssetIsFalseForMissingProperty(): void
    {
        $bag = new Bag();

        self::assertFalse(isset($bag->missing));
    }

    public function testMultiplePropertiesAreIndependent(): void
    {
        $bag = new Bag();
        $bag->foo = 'one';
        $bag->baz = 'two';

        self::assertSame('one', $bag->foo);
        self::assertSame('two', $bag->baz);
    }

    public function testOverwritingPropertyUpdatesValue(): void
    {
        $bag = new Bag();
        $bag->foo = 'first';
        $bag->foo = 'second';

        self::assertSame('second', $bag->foo);
    }
}
