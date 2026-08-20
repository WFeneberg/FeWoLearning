<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex068EnumWithInterface;

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
        return match ($this) {
            self::Hearts => 'Hearts',
            self::Diamonds => 'Diamonds',
            self::Clubs => 'Clubs',
            self::Spades => 'Spades',
        };
    }
}
