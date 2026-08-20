<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex055RegexNamedGroups;

/*
Exercise 055 - Regex named groups (intermediate).

Goal:   Parse a log line into its level and message using named regex capture groups.
Drills: preg_match, named capture groups, accessing matches by string key, no-match handling.
Passes: LogLineParserTest
*/
final class LogLineParser
{
    public static function parse(string $line): ?array
    {
        throw new \RuntimeException('TODO');
    }
}
