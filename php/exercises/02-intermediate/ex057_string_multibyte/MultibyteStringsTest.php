<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex057StringMultibyte;

require_once __DIR__ . '/MultibyteStrings.php';

use PHPUnit\Framework\TestCase;

final class MultibyteStringsTest extends TestCase
{
    public function testCharacterCountDiffersFromByteLength(): void
    {
        $text = 'héllo wörld';

        self::assertNotSame(strlen($text), MultibyteStrings::characterCount($text));
        self::assertSame(11, MultibyteStrings::characterCount($text));
    }

    public function testCharacterCountOnPlainAscii(): void
    {
        self::assertSame(5, MultibyteStrings::characterCount('hello'));
    }

    public function testSafeTruncateKeepsWholeMultibyteCharacters(): void
    {
        $text = 'héllo wörld';

        $truncated = MultibyteStrings::safeTruncate($text, 6);

        self::assertSame('héllo ', $truncated);
        self::assertSame(6, MultibyteStrings::characterCount($truncated));
    }

    public function testSafeTruncateLongerThanStringReturnsWholeString(): void
    {
        self::assertSame('hi', MultibyteStrings::safeTruncate('hi', 10));
    }
}
