<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex010DefaultArguments;

final class DefaultArguments
{
    private function __construct()
    {
    }

    public static function greet(string $name, string $greeting = 'Hello'): string
    {
        return "{$greeting}, {$name}!";
    }

    public static function repeatString(string $text, int $times = 1): string
    {
        return str_repeat($text, $times);
    }
}
