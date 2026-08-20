<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex059StaticFactoryPattern;

require_once __DIR__ . '/EmailAddress.php';

use PHPUnit\Framework\TestCase;

final class EmailAddressTest extends TestCase
{
    public function testValidEmailRoundTripsThroughToString(): void
    {
        $email = EmailAddress::fromString('user@example.com');

        self::assertSame('user@example.com', (string) $email);
    }

    public function testValidEmailWorksInStringInterpolation(): void
    {
        $email = EmailAddress::fromString('jane.doe@example.org');

        self::assertSame('Contact: jane.doe@example.org', "Contact: {$email}");
    }

    public function testInvalidEmailThrows(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        EmailAddress::fromString('not-an-email');
    }

    public function testEmailWithoutDomainThrows(): void
    {
        $this->expectException(\InvalidArgumentException::class);

        EmailAddress::fromString('missing@');
    }
}
