<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex088StreamingLineProcessor;

require_once __DIR__ . '/LineCounter.php';

use PHPUnit\Framework\TestCase;

final class LineCounterTest extends TestCase
{
    private string $path;

    protected function setUp(): void
    {
        $this->path = (string) tempnam(sys_get_temp_dir(), 'fewo_');
        file_put_contents(
            $this->path,
            "apple pie\nbanana split\napple juice\ncherry cake\nAPPLE turnover\n"
        );
    }

    protected function tearDown(): void
    {
        if (is_file($this->path)) {
            unlink($this->path);
        }
    }

    public function testCountsCaseSensitiveMatches(): void
    {
        self::assertSame(2, LineCounter::countMatchingLines($this->path, 'apple'));
    }

    public function testCountsUppercaseMatches(): void
    {
        self::assertSame(1, LineCounter::countMatchingLines($this->path, 'APPLE'));
    }

    public function testReturnsZeroWhenNoLinesMatch(): void
    {
        self::assertSame(0, LineCounter::countMatchingLines($this->path, 'watermelon'));
    }

    public function testCountsAllLinesWhenNeedleIsCommonSubstring(): void
    {
        // "apple pie", "banana split", "apple juice" and "cherry cake" contain a
        // lowercase 'a'; "APPLE turnover" does not (its only "a" is uppercase).
        self::assertSame(4, LineCounter::countMatchingLines($this->path, 'a'));
    }

    public function testEmptyFileYieldsZero(): void
    {
        $emptyPath = (string) tempnam(sys_get_temp_dir(), 'fewo_empty_');
        file_put_contents($emptyPath, '');

        try {
            self::assertSame(0, LineCounter::countMatchingLines($emptyPath, 'apple'));
        } finally {
            unlink($emptyPath);
        }
    }
}
