<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex016FileWriteText;

require_once __DIR__ . '/FileWriteText.php';

use PHPUnit\Framework\TestCase;

final class FileWriteTextTest extends TestCase
{
    private string $path;

    protected function setUp(): void
    {
        $this->path = (string) tempnam(sys_get_temp_dir(), 'fewo_');
    }

    protected function tearDown(): void
    {
        if (file_exists($this->path)) {
            unlink($this->path);
        }
    }

    public function testWriteTextOverwritesContent(): void
    {
        FileWriteText::writeText($this->path, 'hello');
        self::assertSame('hello', file_get_contents($this->path));

        FileWriteText::writeText($this->path, 'world');
        self::assertSame('world', file_get_contents($this->path));
    }

    public function testAppendLineAddsLinesInOrder(): void
    {
        FileWriteText::writeText($this->path, '');
        FileWriteText::appendLine($this->path, 'first');
        FileWriteText::appendLine($this->path, 'second');

        $expected = 'first' . PHP_EOL . 'second' . PHP_EOL;

        self::assertSame($expected, file_get_contents($this->path));
    }

    public function testAppendLineDoesNotTruncateExistingContent(): void
    {
        FileWriteText::writeText($this->path, 'header' . PHP_EOL);
        FileWriteText::appendLine($this->path, 'body');

        self::assertStringContainsString('header', (string) file_get_contents($this->path));
        self::assertStringContainsString('body', (string) file_get_contents($this->path));
    }
}
