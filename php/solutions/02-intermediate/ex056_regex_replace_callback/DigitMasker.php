<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex056RegexReplaceCallback;

final class DigitMasker
{
    public static function maskDigitRuns(string $text): string
    {
        return preg_replace_callback(
            '/\d+/',
            static fn (array $m): string => str_repeat('*', strlen($m[0])),
            $text,
        );
    }
}
