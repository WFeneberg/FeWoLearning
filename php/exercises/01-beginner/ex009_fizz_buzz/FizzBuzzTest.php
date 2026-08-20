<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex009FizzBuzz;

require_once __DIR__ . '/FizzBuzz.php';

use PHPUnit\Framework\TestCase;

final class FizzBuzzTest extends TestCase
{
    public function testFizzBuzzUpToFifteenMatchesExpectedSequence(): void
    {
        $expected = [
            '1', '2', 'Fizz', '4', 'Buzz',
            'Fizz', '7', '8', 'Fizz', 'Buzz',
            '11', 'Fizz', '13', '14', 'FizzBuzz',
        ];

        self::assertSame($expected, FizzBuzz::fizzBuzz(15));
    }

    public function testFizzBuzzReturnsExactlyNElements(): void
    {
        self::assertCount(20, FizzBuzz::fizzBuzz(20));
    }

    public function testFizzBuzzMultipleOfFifteenIsFizzBuzzNotFizzOrBuzz(): void
    {
        $result = FizzBuzz::fizzBuzz(30);

        self::assertSame('FizzBuzz', $result[29]);
        self::assertSame('FizzBuzz', $result[14]);
    }

    public function testFizzBuzzPlainNumberIsStringified(): void
    {
        $result = FizzBuzz::fizzBuzz(1);

        self::assertSame(['1'], $result);
        self::assertIsString($result[0]);
    }
}
