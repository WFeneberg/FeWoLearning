<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex085ExceptionChaining;

/*
Exercise 085 - Exception chaining (advanced).

Goal:   Translate a low-level exception into a domain-specific one while
        preserving the original as the "previous" cause.
Drills: exception chaining, getPrevious(), custom exception subclasses.
Passes: UserRepositoryTest
*/
final class RepositoryException extends \RuntimeException
{
}

final class UserRepository
{
    public function findById(int $id): array
    {
        throw new \RuntimeException('TODO');
    }
}
