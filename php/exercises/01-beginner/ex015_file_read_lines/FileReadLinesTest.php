<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex015FileReadLines;

require_once __DIR__ . '/FileReadLines.php';

use PHPUnit\Framework\TestCase;

final class FileReadLinesTest extends TestCase
{
    private string $path;

    protected function setUp(): void
    {
        $this->path = (string) tempnam(sys_get_temp_dir(), 'fewo_');
        file_put_contents(
            $this->path,
            "first line\n\n   \nsecond line\n\t\nthird line\n"
        );
    }

    protected function tearDown(): void
    {
        if (file_exists($this->path)) {
            unlink($this->path);
        }
    }

    public function testReadsOnlyNonEmptyTrimmedLines(): void
    {
        self::assertSame(
            ['first line', 'second line', 'third line'],
            FileReadLines::readNonEmptyLines($this->path)
        );
    }

    public function testResultIsReindexedFromZero(): void
    {
        $lines = FileReadLines::readNonEmptyLines($this->path);

        self::assertSame([0, 1, 2], array_keys($lines));
    }

    public function testEmptyFileYieldsEmptyArray(): void
    {
        file_put_contents($this->path, "\n\n   \n");

        self::assertSame([], FileReadLines::readNonEmptyLines($this->path));
    }
}
