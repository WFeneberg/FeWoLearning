<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex043AbstractClasses;

/*
Exercise 043 - Abstract classes and the template-method pattern (intermediate).

Goal:   Implement a concrete subclass of an abstract Sorter that supplies the
        comparison logic used by the fixed template-method `sort()`.
Drills: abstract class, abstract method, template-method pattern.
Passes: SorterTest
*/
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
        throw new \RuntimeException('TODO');
    }
}
