<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex017StringToArrayCsv;

require_once __DIR__ . '/StringToArrayCsv.php';

use PHPUnit\Framework\TestCase;

final class StringToArrayCsvTest extends TestCase
{
    public function testParseCsvLineSimple(): void
    {
        self::assertSame(['a', 'b', 'c'], StringToArrayCsv::parseCsvLine('a,b,c'));
    }

    public function testParseCsvLineRespectsQuotedDelimiter(): void
    {
        self::assertSame(['a', 'b,c', 'd'], StringToArrayCsv::parseCsvLine('a,"b,c",d'));
    }

    public function testJoinFieldsWithDefaultDelimiter(): void
    {
        self::assertSame('a,b,c', StringToArrayCsv::joinFields(['a', 'b', 'c']));
    }

    public function testJoinFieldsWithCustomDelimiter(): void
    {
        self::assertSame('a;b;c', StringToArrayCsv::joinFields(['a', 'b', 'c'], ';'));
    }

    public function testRoundTripThroughParseAndJoin(): void
    {
        $fields = StringToArrayCsv::parseCsvLine('x,y,z');

        self::assertSame('x,y,z', StringToArrayCsv::joinFields($fields));
    }
}
