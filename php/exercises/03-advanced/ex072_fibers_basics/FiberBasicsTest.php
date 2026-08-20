<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex072FibersBasics;

require_once __DIR__ . '/FiberBasics.php';

use PHPUnit\Framework\TestCase;

final class FiberBasicsTest extends TestCase
{
    public function testReturnsSuspendedValueAndFinalReturn(): void
    {
        $result = FiberBasics::runWithSuspension();

        self::assertSame(['paused', 'resumed with hello'], $result);
    }

    public function testFirstElementIsTheSuspendValue(): void
    {
        [$startResult, ] = FiberBasics::runWithSuspension();

        self::assertSame('paused', $startResult);
    }

    public function testSecondElementIncorporatesTheResumeValue(): void
    {
        [, $returnValue] = FiberBasics::runWithSuspension();

        self::assertStringContainsString('hello', $returnValue);
    }

    public function testResultHasExactlyTwoElements(): void
    {
        $result = FiberBasics::runWithSuspension();

        self::assertCount(2, $result);
    }
}
