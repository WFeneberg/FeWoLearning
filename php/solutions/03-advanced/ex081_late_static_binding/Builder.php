<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex081LateStaticBinding;

abstract class Builder
{
    public static function create(): static
    {
        return new static();
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
