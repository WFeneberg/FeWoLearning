<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex073FibersCooperativeScheduler;

require_once __DIR__ . '/FiberScheduler.php';

use PHPUnit\Framework\TestCase;

final class FiberSchedulerTest extends TestCase
{
    public function testCollectsReturnsInOriginalOrder(): void
    {
        $suspendingBody = static function (): string {
            \Fiber::suspend();

            return 'a';
        };

        $immediateBody = static function (): string {
            return 'b';
        };

        $result = FiberScheduler::runAll([$suspendingBody, $immediateBody]);

        self::assertSame(['a', 'b'], $result);
    }

    public function testHandlesMultipleSuspendsBeforeReturning(): void
    {
        $body = static function (): string {
            \Fiber::suspend();
            \Fiber::suspend();
            \Fiber::suspend();

            return 'done';
        };

        $result = FiberScheduler::runAll([$body]);

        self::assertSame(['done'], $result);
    }

    public function testAllImmediateReturnsRequiresNoResume(): void
    {
        $result = FiberScheduler::runAll([
            static fn (): string => 'x',
            static fn (): string => 'y',
            static fn (): string => 'z',
        ]);

        self::assertSame(['x', 'y', 'z'], $result);
    }

    public function testOrderPreservedRegardlessOfSuspendCount(): void
    {
        $manySuspends = static function (): string {
            for ($i = 0; $i < 5; $i++) {
                \Fiber::suspend();
            }

            return 'many';
        };

        $noSuspends = static fn (): string => 'none';

        $result = FiberScheduler::runAll([$manySuspends, $noSuspends]);

        self::assertSame(['many', 'none'], $result);
    }
}
