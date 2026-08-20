<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex041FirstClassCallable;

/*
Exercise 041 - First-class callable syntax (intermediate).

Goal:   Use PHP 8.1's first-class callable syntax to pass a built-in function and
        an instance method directly into array_map, without string callables or
        arrow-function wrappers.
Drills: first-class callable syntax (fn(...)), passing it to array_map, instance method (...) syntax.
Passes: TextTransformsTest
*/
final class TextTransforms
{
    public static function uppercaseAll(array $strings): array
    {
        throw new \RuntimeException('TODO');
    }

    public function suffixAll(array $strings, string $suffix): array
    {
        throw new \RuntimeException('TODO');
    }

    private function appendSuffix(string $s, string $suffix): string
    {
        throw new \RuntimeException('TODO');
    }
}
