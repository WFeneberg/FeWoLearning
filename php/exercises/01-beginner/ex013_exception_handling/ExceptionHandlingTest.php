<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex013ExceptionHandling;

require_once __DIR__ . '/ExceptionHandling.php';

use PHPUnit\Framework\TestCase;

final class ExceptionHandlingTest extends TestCase
{
    protected function setUp(): void
    {
        ExceptionHandling::$finallyRuns = 0;
    }

    public function testSafeDivideSuccess(): void
    {
        $outcome = ExceptionHandling::safeDivide(10, 4);

        self::assertSame(2.5, $outcome['result']);
        self::assertNull($outcome['error']);
    }

    public function testSafeDivideByZero(): void
    {
        $outcome = ExceptionHandling::safeDivide(10, 0);

        self::assertNull($outcome['result']);
        self::assertSame('division by zero', $outcome['error']);
    }

    public function testFinallyRunsOnSuccessPath(): void
    {
        ExceptionHandling::safeDivide(6, 3);

        self::assertSame(1, ExceptionHandling::$finallyRuns);
    }

    public function testFinallyRunsOnErrorPath(): void
    {
        ExceptionHandling::safeDivide(6, 0);

        self::assertSame(1, ExceptionHandling::$finallyRuns);
    }

    public function testFinallyRunsOnBothPathsAcrossCalls(): void
    {
        ExceptionHandling::safeDivide(6, 3);
        ExceptionHandling::safeDivide(6, 0);

        self::assertSame(2, ExceptionHandling::$finallyRuns);
    }
}
