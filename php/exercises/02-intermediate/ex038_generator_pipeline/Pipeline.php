<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex038GeneratorPipeline;

/*
Exercise 038 - Generator pipeline (intermediate).

Goal:   Implement composable map/filter generators that stream transformations
        without building intermediate arrays.
Drills: chained/composed generators, streaming transformation without intermediate arrays.
Passes: PipelineTest
*/
final class Pipeline
{
    private function __construct()
    {
    }

    public static function map(iterable $source, callable $fn): \Generator
    {
        throw new \RuntimeException('TODO');
    }

    public static function filter(iterable $source, callable $predicate): \Generator
    {
        throw new \RuntimeException('TODO');
    }
}
