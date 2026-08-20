<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex084SingledispatchMatch;

require_once __DIR__ . '/TypeDescriber.php';

use PHPUnit\Framework\TestCase;

final class TypeDescriberTest extends TestCase
{
    public function testDescribesInteger(): void
    {
        self::assertSame('integer: 42', TypeDescriber::describe(42));
    }

    public function testDescribesString(): void
    {
        self::assertSame('string: hello', TypeDescriber::describe('hello'));
    }

    public function testDescribesArrayByCount(): void
    {
        self::assertSame('array of 3', TypeDescriber::describe([1, 2, 3]));
    }

    public function testDescribesDateTimeImmutable(): void
    {
        $date = new \DateTimeImmutable('2026-08-19');

        self::assertSame('date: 2026-08-19', TypeDescriber::describe($date));
    }

    public function testDescribesUnknownTypeUsingDebugType(): void
    {
        $result = TypeDescriber::describe(3.14);

        self::assertSame('unknown: float', $result);
    }
}
