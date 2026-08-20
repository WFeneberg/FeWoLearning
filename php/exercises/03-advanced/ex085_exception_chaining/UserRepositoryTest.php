<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex085ExceptionChaining;

require_once __DIR__ . '/RepositoryException.php';

use PHPUnit\Framework\TestCase;

final class UserRepositoryTest extends TestCase
{
    public function testFindByIdThrowsRepositoryException(): void
    {
        $repository = new UserRepository();

        $this->expectException(RepositoryException::class);

        $repository->findById(7);
    }

    public function testPreviousExceptionIsPreserved(): void
    {
        $repository = new UserRepository();

        try {
            $repository->findById(7);
            self::fail('expected a RepositoryException to be thrown');
        } catch (RepositoryException $e) {
            self::assertInstanceOf(\UnexpectedValueException::class, $e->getPrevious());
        }
    }

    public function testRepositoryExceptionIsARuntimeException(): void
    {
        self::assertInstanceOf(\RuntimeException::class, new RepositoryException('boom'));
    }

    public function testPreviousExceptionMessageMentionsId(): void
    {
        $repository = new UserRepository();

        try {
            $repository->findById(99);
            self::fail('expected a RepositoryException to be thrown');
        } catch (RepositoryException $e) {
            $previous = $e->getPrevious();
            self::assertNotNull($previous);
            self::assertStringContainsString('99', $previous->getMessage());
        }
    }
}
