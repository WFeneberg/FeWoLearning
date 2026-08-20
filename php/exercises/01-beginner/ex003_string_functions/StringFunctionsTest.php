<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex003StringFunctions;

require_once __DIR__ . '/StringFunctions.php';

use PHPUnit\Framework\TestCase;

final class StringFunctionsTest extends TestCase
{
    public function testHasSubstringTrueAndFalse(): void
    {
        self::assertTrue(StringFunctions::hasSubstring('hello world', 'lo wo'));
        self::assertFalse(StringFunctions::hasSubstring('hello world', 'xyz'));
    }

    public function testStartsWithPrefix(): void
    {
        self::assertTrue(StringFunctions::startsWithPrefix('filename.txt', 'file'));
        self::assertFalse(StringFunctions::startsWithPrefix('filename.txt', 'name'));
    }

    public function testEndsWithSuffix(): void
    {
        self::assertTrue(StringFunctions::endsWithSuffix('filename.txt', '.txt'));
        self::assertFalse(StringFunctions::endsWithSuffix('filename.txt', '.csv'));
    }

    public function testTruncateShortensLongStringsAndAppendsEllipsis(): void
    {
        self::assertSame('hello...', StringFunctions::truncate('hello world', 5));
    }

    public function testTruncateLeavesShortStringsUnchanged(): void
    {
        self::assertSame('hi', StringFunctions::truncate('hi', 5));
    }

    public function testReplaceAllReplacesEveryKeyInOnePass(): void
    {
        $result = StringFunctions::replaceAll('foo bar baz', ['foo' => 'baz', 'baz' => 'foo']);

        self::assertSame('baz bar foo', $result);
    }
}
