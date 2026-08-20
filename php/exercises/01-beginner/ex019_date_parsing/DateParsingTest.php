<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex019DateParsing;

require_once __DIR__ . '/DateParsing.php';

use PHPUnit\Framework\TestCase;

final class DateParsingTest extends TestCase
{
    public function testParseIsoDateRoundTripsToGermanFormat(): void
    {
        $date = DateParsing::parseIsoDate('2026-08-19');

        self::assertSame('19.08.2026', DateParsing::formatGermanDate($date));
    }

    public function testParseIsoDateProducesCorrectComponents(): void
    {
        $date = DateParsing::parseIsoDate('2026-01-30');

        self::assertSame('2026-01-30', $date->format('Y-m-d'));
    }

    public function testParseIsoDateThrowsOnInvalidString(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        DateParsing::parseIsoDate('not-a-date');
    }

    public function testFormatGermanDateUsesDotSeparators(): void
    {
        $date = new \DateTimeImmutable('2001-02-03');

        self::assertSame('03.02.2001', DateParsing::formatGermanDate($date));
    }
}
