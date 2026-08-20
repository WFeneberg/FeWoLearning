<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex083SerializationHooks;

require_once __DIR__ . '/Session.php';

use PHPUnit\Framework\TestCase;

final class SessionTest extends TestCase
{
    public function testRoundTripPreservesUserIdAndData(): void
    {
        $session = new Session('user-42', ['role' => 'admin'], 'super-secret');

        $restored = unserialize(serialize($session));

        self::assertSame('user-42', $restored->userId());
        self::assertSame(['role' => 'admin'], $restored->data());
    }

    public function testRoundTripExcludesSecretToken(): void
    {
        $session = new Session('user-42', ['role' => 'admin'], 'super-secret');

        $restored = unserialize(serialize($session));

        self::assertNotSame('super-secret', $restored->secretToken());
        self::assertSame('', $restored->secretToken());
    }

    public function testOriginalInstanceStillHasSecretToken(): void
    {
        $session = new Session('user-42', ['role' => 'admin'], 'super-secret');

        unserialize(serialize($session));

        self::assertSame('super-secret', $session->secretToken());
    }

    public function testSerializedStringDoesNotContainSecret(): void
    {
        $session = new Session('user-1', [], 'top-secret-value');

        $serialized = serialize($session);

        self::assertStringNotContainsString('top-secret-value', $serialized);
    }

    public function testEmptyDataArrayRoundTrips(): void
    {
        $session = new Session('user-empty', [], 'token');

        $restored = unserialize(serialize($session));

        self::assertSame([], $restored->data());
        self::assertSame('user-empty', $restored->userId());
    }
}
