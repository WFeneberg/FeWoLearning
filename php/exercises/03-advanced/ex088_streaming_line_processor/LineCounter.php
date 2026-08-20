<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex088StreamingLineProcessor;

/*
Exercise 088 - Streaming line processor (advanced).

Goal:   Process a file line-by-line via fopen/fgets in a memory-bounded way,
        instead of loading the whole file into memory at once.
Drills: fopen, fgets, streaming, resource cleanup with finally.
Passes: LineCounterTest
*/
final class LineCounter
{
    public static function countMatchingLines(string $path, string $needle): int
    {
        throw new \RuntimeException('TODO');
    }
}
