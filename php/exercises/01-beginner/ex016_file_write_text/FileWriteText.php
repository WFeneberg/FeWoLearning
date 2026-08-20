<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex016FileWriteText;

/*
Exercise 016 - Writing and appending text files (beginner).

Goal:   Implement helpers to overwrite a file's contents and to append lines to it.
Drills: file_put_contents, appending via FILE_APPEND, file_get_contents.
Passes: FileWriteTextTest
*/
final class FileWriteText
{
    private function __construct()
    {
    }

    public static function writeText(string $path, string $content): void
    {
        throw new \RuntimeException('TODO');
    }

    public static function appendLine(string $path, string $line): void
    {
        throw new \RuntimeException('TODO');
    }
}
