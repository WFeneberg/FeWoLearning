<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex046EnumsBacked;

require_once __DIR__ . '/StatusCode.php';

use PHPUnit\Framework\TestCase;

final class StatusCodeTest extends TestCase
{
    public function testLabelForOk(): void
    {
        self::assertSame('OK', StatusCodes::label(StatusCode::Ok));
    }

    public function testLabelForNotFound(): void
    {
        self::assertSame('Not Found', StatusCodes::label(StatusCode::NotFound));
    }

    public function testLabelForServerError(): void
    {
        self::assertSame('Server Error', StatusCodes::label(StatusCode::ServerError));
    }

    public function testFromValueWithKnownValue(): void
    {
        self::assertSame(StatusCode::NotFound, StatusCodes::fromValue(404));
    }

    public function testFromValueWithUnknownValueReturnsNull(): void
    {
        self::assertNull(StatusCodes::fromValue(999));
    }
}
