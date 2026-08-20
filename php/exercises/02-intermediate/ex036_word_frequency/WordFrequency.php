<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex036WordFrequency;

/*
Exercise 036 - Word frequency counting (intermediate).

Goal:   Count how often each word occurs in a text and return the counts sorted
        from most to least frequent.
Drills: counting via an associative array, arsort, preg_split for tokenizing.
Passes: WordFrequencyTest
*/
final class WordFrequency
{
    private function __construct()
    {
    }

    public static function wordFrequencies(string $text): array
    {
        throw new \RuntimeException('TODO');
    }
}
