<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex055RegexNamedGroups;

final class LogLineParser
{
    public static function parse(string $line): ?array
    {
        $matched = preg_match('/^\[(?<level>\w+)\]\s+(?<message>.+)$/', $line, $matches);

        if ($matched !== 1) {
            return null;
        }

        return [
            'level' => $matches['level'],
            'message' => $matches['message'],
        ];
    }
}
