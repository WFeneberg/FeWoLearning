<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex038GeneratorPipeline;

final class Pipeline
{
    private function __construct()
    {
    }

    public static function map(iterable $source, callable $fn): \Generator
    {
        foreach ($source as $item) {
            yield $fn($item);
        }
    }

    public static function filter(iterable $source, callable $predicate): \Generator
    {
        foreach ($source as $item) {
            if ($predicate($item)) {
                yield $item;
            }
        }
    }
}
