<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex042TraitsBasic;

/*
Exercise 042 - Traits and conflict resolution (intermediate).

Goal:   Implement two traits that both declare a conflicting `shout` method,
        then resolve the conflict in a consuming class with `insteadof`/`as`.
Drills: trait declaration, method-name conflict resolution with insteadof/as.
Passes: FormattersTest
*/
trait Loud
{
    public function shout(string $s): string
    {
        throw new \RuntimeException('TODO');
    }
}

trait Quiet
{
    public function shout(string $s): string
    {
        throw new \RuntimeException('TODO');
    }
}

final class Announcer
{
    use Loud, Quiet {
        Loud::shout insteadof Quiet;
        Quiet::shout as whisper;
    }
}
