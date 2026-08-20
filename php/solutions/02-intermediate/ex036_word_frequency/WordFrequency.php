<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex036WordFrequency;

final class WordFrequency
{
    private function __construct()
    {
    }

    public static function wordFrequencies(string $text): array
    {
        $words = preg_split('/\W+/u', mb_strtolower($text), -1, PREG_SPLIT_NO_EMPTY);

        $counts = [];
        foreach ($words as $word) {
            $counts[$word] = ($counts[$word] ?? 0) + 1;
        }

        arsort($counts);

        return $counts;
    }
}
