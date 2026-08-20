<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex083SerializationHooks;

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
        return [
            'userId' => $this->userId,
            'data' => $this->data,
        ];
    }

    public function __unserialize(array $data): void
    {
        $this->userId = $data['userId'];
        $this->data = $data['data'];
        $this->secretToken = '';
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
