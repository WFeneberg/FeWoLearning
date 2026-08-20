<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex056RegexReplaceCallback;

require_once __DIR__ . '/DigitMasker.php';

use PHPUnit\Framework\TestCase;

final class DigitMaskerTest extends TestCase
{
    public function testMasksTwoRunsOfDifferentLengths(): void
    {
        self::assertSame('card **** exp **', DigitMasker::maskDigitRuns('card 4111 exp 25'));
    }

    public function testStringWithoutDigitsIsUnchanged(): void
    {
        self::assertSame('no digits here', DigitMasker::maskDigitRuns('no digits here'));
    }

    public function testSingleDigitIsMaskedWithOneAsterisk(): void
    {
        self::assertSame('item * only', DigitMasker::maskDigitRuns('item 7 only'));
    }

    public function testAdjacentDigitRunAtStringBoundary(): void
    {
        self::assertSame('**** start', DigitMasker::maskDigitRuns('2024 start'));
    }
}
