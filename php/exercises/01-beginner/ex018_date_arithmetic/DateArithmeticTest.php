<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex018DateArithmetic;

require_once __DIR__ . '/DateArithmetic.php';

use PHPUnit\Framework\TestCase;

final class DateArithmeticTest extends TestCase
{
    public function testAddDaysCrossesMonthBoundary(): void
    {
        $start = new \DateTimeImmutable('2026-01-30');

        $result = DateArithmetic::addDays($start, 5);

        self::assertSame('2026-02-04', $result->format('Y-m-d'));
    }

    public function testAddDaysWithNegativeValueSubtractsAndCrossesMonthBoundary(): void
    {
        $start = new \DateTimeImmutable('2026-03-03');

        $result = DateArithmetic::addDays($start, -5);

        self::assertSame('2026-02-26', $result->format('Y-m-d'));
    }

    public function testAddDaysDoesNotMutateOriginal(): void
    {
        $start = new \DateTimeImmutable('2026-01-30');

        DateArithmetic::addDays($start, 5);

        self::assertSame('2026-01-30', $start->format('Y-m-d'));
    }

    public function testDaysBetweenIsSymmetric(): void
    {
        $a = new \DateTimeImmutable('2026-08-01');
        $b = new \DateTimeImmutable('2026-08-10');

        self::assertSame(9, DateArithmetic::daysBetween($a, $b));
        self::assertSame(9, DateArithmetic::daysBetween($b, $a));
    }

    public function testDaysBetweenSameDateIsZero(): void
    {
        $a = new \DateTimeImmutable('2026-08-19');

        self::assertSame(0, DateArithmetic::daysBetween($a, $a));
    }
}
