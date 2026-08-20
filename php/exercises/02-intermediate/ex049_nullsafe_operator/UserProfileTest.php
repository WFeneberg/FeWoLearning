<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex049NullsafeOperator;

require_once __DIR__ . '/UserProfile.php';

use PHPUnit\Framework\TestCase;

final class UserProfileTest extends TestCase
{
    public function testCityWithFullUserAndAddress(): void
    {
        $user = new User(new Address('Berlin'));

        self::assertSame('Berlin', UserProfiles::cityOrDefault($user));
    }

    public function testCityWithUserButNoAddress(): void
    {
        $user = new User(null);

        self::assertSame('Unknown', UserProfiles::cityOrDefault($user));
    }

    public function testCityWithNullUser(): void
    {
        self::assertSame('Unknown', UserProfiles::cityOrDefault(null));
    }

    public function testCityWithDifferentAddress(): void
    {
        $user = new User(new Address('Munich'));

        self::assertSame('Munich', UserProfiles::cityOrDefault($user));
    }
}
