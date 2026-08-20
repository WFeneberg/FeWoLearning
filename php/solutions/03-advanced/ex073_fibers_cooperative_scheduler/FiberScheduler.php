<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex073FibersCooperativeScheduler;

final class FiberScheduler
{
    private function __construct()
    {
    }

    public static function runAll(array $fiberBodies): array
    {
        $fibers = [];

        foreach ($fiberBodies as $body) {
            $fiber = new \Fiber($body);
            $fiber->start();
            $fibers[] = $fiber;
        }

        do {
            $allTerminated = true;

            foreach ($fibers as $fiber) {
                if (!$fiber->isTerminated()) {
                    $fiber->resume(null);
                    $allTerminated = false;
                }
            }
        } while (!$allTerminated);

        return array_map(static fn (\Fiber $fiber): mixed => $fiber->getReturn(), $fibers);
    }
}
