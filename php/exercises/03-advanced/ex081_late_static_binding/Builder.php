<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex081LateStaticBinding;

/*
Exercise 081 - Late static binding (advanced).

Goal:   Make an abstract base's factory method return an instance of whichever
        subclass it is called on, via late static binding.
Drills: static:: vs self::, new static(), late static binding.
Passes: BuilderTest
*/
abstract class Builder
{
    public static function create(): static
    {
        throw new \RuntimeException('TODO');
    }

    abstract public function describe(): string;
}

final class WidgetBuilder extends Builder
{
    public function describe(): string
    {
        return 'widget';
    }
}
