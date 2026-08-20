<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex085ExceptionChaining;

final class RepositoryException extends \RuntimeException
{
}

final class UserRepository
{
    public function findById(int $id): array
    {
        try {
            throw new \UnexpectedValueException("row {$id} not found");
        } catch (\UnexpectedValueException $e) {
            throw new RepositoryException("could not load user {$id}", previous: $e);
        }
    }
}
