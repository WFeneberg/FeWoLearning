<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex016FileWriteText;

final class FileWriteText
{
    private function __construct()
    {
    }

    public static function writeText(string $path, string $content): void
    {
        file_put_contents($path, $content);
    }

    public static function appendLine(string $path, string $line): void
    {
        file_put_contents($path, $line . PHP_EOL, FILE_APPEND);
    }
}
