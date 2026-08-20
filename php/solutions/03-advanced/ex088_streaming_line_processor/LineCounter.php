<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex088StreamingLineProcessor;

final class LineCounter
{
    public static function countMatchingLines(string $path, string $needle): int
    {
        $handle = fopen($path, 'rb');

        if ($handle === false) {
            throw new \RuntimeException("could not open file '{$path}'");
        }

        $count = 0;

        try {
            while (($line = fgets($handle)) !== false) {
                if (str_contains($line, $needle)) {
                    $count++;
                }
            }
        } finally {
            fclose($handle);
        }

        return $count;
    }
}
