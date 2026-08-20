<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex055RegexNamedGroups;

require_once __DIR__ . '/LogLineParser.php';

use PHPUnit\Framework\TestCase;

final class LogLineParserTest extends TestCase
{
    public function testParsesLevelAndMessage(): void
    {
        $result = LogLineParser::parse('[ERROR] Disk full');

        self::assertSame(['level' => 'ERROR', 'message' => 'Disk full'], $result);
    }

    public function testParsesDifferentLevel(): void
    {
        $result = LogLineParser::parse('[INFO] Service started');

        self::assertSame('INFO', $result['level']);
        self::assertSame('Service started', $result['message']);
    }

    public function testReturnsNullWhenNoMatch(): void
    {
        $result = LogLineParser::parse('this is not a log line');

        self::assertNull($result);
    }

    public function testReturnsNullWhenLevelMissingBrackets(): void
    {
        $result = LogLineParser::parse('ERROR Disk full');

        self::assertNull($result);
    }
}
