<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex010DefaultArguments;

require_once __DIR__ . '/DefaultArguments.php';

use PHPUnit\Framework\TestCase;

final class DefaultArgumentsTest extends TestCase
{
    public function testGreetUsesDefaultGreetingWhenOmitted(): void
    {
        self::assertSame('Hello, Alice!', DefaultArguments::greet('Alice'));
    }

    public function testGreetUsesOverriddenGreetingWhenProvided(): void
    {
        self::assertSame('Hi, Bob!', DefaultArguments::greet('Bob', 'Hi'));
    }

    public function testRepeatStringUsesDefaultTimesOfOne(): void
    {
        self::assertSame('ab', DefaultArguments::repeatString('ab'));
    }

    public function testRepeatStringUsesOverriddenTimes(): void
    {
        self::assertSame('ababab', DefaultArguments::repeatString('ab', 3));
    }
}
