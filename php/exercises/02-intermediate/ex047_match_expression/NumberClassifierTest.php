<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex047MatchExpression;

require_once __DIR__ . '/NumberClassifier.php';

use PHPUnit\Framework\TestCase;

final class NumberClassifierTest extends TestCase
{
    public function testClassifyNegative(): void
    {
        self::assertSame('negative', NumberClassifier::classify(-5));
    }

    public function testClassifyZero(): void
    {
        self::assertSame('zero', NumberClassifier::classify(0));
    }

    public function testClassifyEven(): void
    {
        self::assertSame('even', NumberClassifier::classify(4));
    }

    public function testClassifyOdd(): void
    {
        self::assertSame('odd', NumberClassifier::classify(3));
    }

    public function testClassifyStrictHandlesZeroAndOne(): void
    {
        self::assertSame('zero', NumberClassifier::classifyStrict(0));
        self::assertSame('one', NumberClassifier::classifyStrict(1));
    }

    public function testClassifyStrictThrowsForUnhandledValue(): void
    {
        $this->expectException(\UnhandledMatchError::class);

        NumberClassifier::classifyStrict(2);
    }
}
