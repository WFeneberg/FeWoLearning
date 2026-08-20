<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex083SerializationHooks;

/*
Exercise 083 - Serialization hooks (advanced).

Goal:   Implement __serialize()/__unserialize() so that a secret field is
        deliberately excluded from PHP's native serialize()/unserialize()
        round-trip.
Drills: __serialize, __unserialize, controlling serialized state, readonly properties.
Passes: SessionTest
*/
final class Session
{
    public function __construct(
        private string $userId,
        private array $data,
        private readonly string $secretToken,
    ) {
    }

    public function __serialize(): array
    {
        throw new \RuntimeException('TODO');
    }

    public function __unserialize(array $data): void
    {
        throw new \RuntimeException('TODO');
    }

    public function userId(): string
    {
        return $this->userId;
    }

    public function data(): array
    {
        return $this->data;
    }

    public function secretToken(): ?string
    {
        return $this->secretToken ?? null;
    }
}
