<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex036WordFrequency;

require_once __DIR__ . '/WordFrequency.php';

use PHPUnit\Framework\TestCase;

final class WordFrequencyTest extends TestCase
{
    public function testRepeatedWordComesFirst(): void
    {
        $result = WordFrequency::wordFrequencies('the cat sat on the mat with the dog');

        $keys = array_keys($result);

        self::assertSame('the', $keys[0]);
        self::assertSame(3, $result['the']);
    }

    public function testIsCaseInsensitive(): void
    {
        $result = WordFrequency::wordFrequencies('Cat cat CAT dog');

        self::assertSame(3, $result['cat']);
        self::assertSame(1, $result['dog']);
    }

    public function testPunctuationIsIgnored(): void
    {
        $result = WordFrequency::wordFrequencies('Hello, world! Hello world.');

        self::assertSame(2, $result['hello']);
        self::assertSame(2, $result['world']);
        self::assertArrayNotHasKey('hello,', $result);
    }

    public function testResultIsSortedDescendingByCount(): void
    {
        $result = WordFrequency::wordFrequencies('a a a b b c');

        $counts = array_values($result);

        self::assertSame([3, 2, 1], $counts);
    }

    public function testEmptyTextProducesEmptyResult(): void
    {
        self::assertSame([], WordFrequency::wordFrequencies(''));
    }
}
