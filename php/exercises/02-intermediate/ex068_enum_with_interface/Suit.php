<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex068EnumWithInterface;

/*
Exercise 068 - Enum implementing an interface (intermediate).

Goal:   Implement an interface on a pure enum and give it a real method.
Drills: enum implements interface, match on $this, enum instanceof.
Passes: SuitTest
*/
interface HasLabel
{
    public function label(): string;
}

enum Suit implements HasLabel
{
    case Hearts;
    case Diamonds;
    case Clubs;
    case Spades;

    public function label(): string
    {
        throw new \RuntimeException('TODO');
    }
}
