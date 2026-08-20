<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex024RandomNumbers;

require_once __DIR__ . '/RandomNumbers.php';

use PHPUnit\Framework\TestCase;

final class RandomNumbersTest extends TestCase
{
    public function testSeededRollIsDeterministicForSameSeed(): void
    {
        $first = RandomNumbers::seededRoll(42, 6);
        $second = RandomNumbers::seededRoll(42, 6);

        self::assertSame($first, $second);
    }

    public function testSeededRollWithinBounds(): void
    {
        $result = RandomNumbers::seededRoll(123, 6);

        self::assertGreaterThanOrEqual(1, $result);
        self::assertLessThanOrEqual(6, $result);
    }

    public function testSeededRollWithOneSideAlwaysReturnsOne(): void
    {
        self::assertSame(1, RandomNumbers::seededRoll(1, 1));
        self::assertSame(1, RandomNumbers::seededRoll(999, 1));
    }

    public function testDiceRollIsWithinBoundsAcrossManyCalls(): void
    {
        for ($i = 0; $i < 200; $i++) {
            $result = RandomNumbers::diceRoll(6);

            self::assertGreaterThanOrEqual(1, $result);
            self::assertLessThanOrEqual(6, $result);
        }
    }

    public function testDiceRollWithOneSideAlwaysReturnsOne(): void
    {
        self::assertSame(1, RandomNumbers::diceRoll(1));
    }
}
