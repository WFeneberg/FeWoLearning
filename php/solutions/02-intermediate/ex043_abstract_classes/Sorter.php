<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex043AbstractClasses;

abstract class Sorter
{
    abstract protected function compare(mixed $a, mixed $b): int;

    public function sort(array $items): array
    {
        $copy = $items;
        usort($copy, fn ($a, $b) => $this->compare($a, $b));

        return $copy;
    }
}

final class NumericAscendingSorter extends Sorter
{
    protected function compare(mixed $a, mixed $b): int
    {
        return $a <=> $b;
    }
}
