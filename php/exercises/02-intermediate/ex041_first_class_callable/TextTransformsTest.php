<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex041FirstClassCallable;

require_once __DIR__ . '/TextTransforms.php';

use PHPUnit\Framework\TestCase;

final class TextTransformsTest extends TestCase
{
    public function testUppercaseAll(): void
    {
        self::assertSame(
            ['HELLO', 'WORLD'],
            TextTransforms::uppercaseAll(['hello', 'world'])
        );
    }

    public function testUppercaseAllOfEmptyArray(): void
    {
        self::assertSame([], TextTransforms::uppercaseAll([]));
    }

    public function testSuffixAll(): void
    {
        $transforms = new TextTransforms();

        self::assertSame(
            ['foo-ing', 'bar-ing'],
            $transforms->suffixAll(['foo', 'bar'], '-ing')
        );
    }

    public function testSuffixAllOfEmptyArray(): void
    {
        $transforms = new TextTransforms();

        self::assertSame([], $transforms->suffixAll([], '-x'));
    }

    public function testSuffixAllWithEmptySuffixIsIdentity(): void
    {
        $transforms = new TextTransforms();

        self::assertSame(['foo', 'bar'], $transforms->suffixAll(['foo', 'bar'], ''));
    }
}
