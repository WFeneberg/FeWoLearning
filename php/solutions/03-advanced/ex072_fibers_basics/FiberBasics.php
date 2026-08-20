<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex072FibersBasics;

final class FiberBasics
{
    private function __construct()
    {
    }

    public static function runWithSuspension(): array
    {
        $fiber = new \Fiber(function (): string {
            $received = \Fiber::suspend('paused');

            return "resumed with {$received}";
        });

        $startResult = $fiber->start();
        $fiber->resume('hello');

        return [$startResult, $fiber->getReturn()];
    }
}
