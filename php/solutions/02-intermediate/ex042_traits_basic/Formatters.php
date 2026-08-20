<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex042TraitsBasic;

trait Loud
{
    public function shout(string $s): string
    {
        return strtoupper($s) . '!';
    }
}

trait Quiet
{
    public function shout(string $s): string
    {
        return '(' . strtolower($s) . ')';
    }
}

final class Announcer
{
    use Loud, Quiet {
        Loud::shout insteadof Quiet;
        Quiet::shout as whisper;
    }
}
