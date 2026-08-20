<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex070DtoWithValidation;

require_once __DIR__ . '/Registration.php';

use PHPUnit\Framework\TestCase;

final class RegistrationTest extends TestCase
{
    public function testValidRegistrationConstructsSuccessfully(): void
    {
        $registration = new Registration('user@example.com', 30);

        self::assertSame('user@example.com', $registration->email);
        self::assertSame(30, $registration->age);
    }

    public function testInvalidEmailThrows(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        new Registration('not-an-email', 30);
    }

    public function testNegativeAgeThrows(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        new Registration('user@example.com', -1);
    }

    public function testAgeAboveMaximumThrows(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        new Registration('user@example.com', 151);
    }

    public function testBoundaryAgesAreAccepted(): void
    {
        $newborn = new Registration('user@example.com', 0);
        $oldest = new Registration('user@example.com', 150);

        self::assertSame(0, $newborn->age);
        self::assertSame(150, $oldest->age);
    }
}
