<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex014InputValidation;

require_once __DIR__ . '/InputValidation.php';

use PHPUnit\Framework\TestCase;

final class InputValidationTest extends TestCase
{
    public function testValidAgeIsReturnedUnchanged(): void
    {
        self::assertSame(42, InputValidation::validateAge(42));
    }

    public function testBoundaryValuesAreAccepted(): void
    {
        self::assertSame(0, InputValidation::validateAge(0));
        self::assertSame(150, InputValidation::validateAge(150));
    }

    public function testNegativeAgeThrows(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        InputValidation::validateAge(-1);
    }

    public function testTooLargeAgeThrows(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        InputValidation::validateAge(151);
    }
}
