<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex011VariadicFunctions;

require_once __DIR__ . '/VariadicFunctions.php';

use PHPUnit\Framework\TestCase;

final class VariadicFunctionsTest extends TestCase
{
    public function testSumAllWithSeveralPlainArguments(): void
    {
        self::assertSame(6, VariadicFunctions::sumAll(1, 2, 3));
    }

    public function testSumAllWithNoArgumentsIsZero(): void
    {
        self::assertSame(0, VariadicFunctions::sumAll());
    }

    public function testSumAllWithSpreadOperatorOnExistingArray(): void
    {
        $numbers = [4, 5, 6, 7];

        self::assertSame(22, VariadicFunctions::sumAll(...$numbers));
    }

    public function testJoinWithSeparatorUsingPlainArguments(): void
    {
        self::assertSame('a-b-c', VariadicFunctions::joinWithSeparator('-', 'a', 'b', 'c'));
    }

    public function testJoinWithSeparatorUsingSpreadOperatorOnExistingArray(): void
    {
        $parts = ['x', 'y', 'z'];

        self::assertSame('x, y, z', VariadicFunctions::joinWithSeparator(', ', ...$parts));
    }

    public function testJoinWithSeparatorWithSinglePartHasNoSeparator(): void
    {
        self::assertSame('solo', VariadicFunctions::joinWithSeparator('-', 'solo'));
    }
}
