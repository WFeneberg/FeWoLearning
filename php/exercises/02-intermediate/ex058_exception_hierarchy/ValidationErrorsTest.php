<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex058ExceptionHierarchy;

require_once __DIR__ . '/ValidationErrors.php';

use PHPUnit\Framework\TestCase;

final class ValidationErrorsTest extends TestCase
{
    public function testWrappedExceptionPreservesPreviousCause(): void
    {
        $cause = new \InvalidArgumentException('raw input malformed');

        $wrapped = ErrorTranslator::wrapLowLevelError($cause);

        self::assertSame($cause, $wrapped->getPrevious());
    }

    public function testWrappedExceptionIsADomainException(): void
    {
        $cause = new \RuntimeException('low level failure');

        $wrapped = ErrorTranslator::wrapLowLevelError($cause);

        self::assertInstanceOf(\DomainException::class, $wrapped);
        self::assertInstanceOf(ValidationException::class, $wrapped);
    }

    public function testWrappedExceptionHasNonEmptyMessage(): void
    {
        $cause = new \LogicException('bad state');

        $wrapped = ErrorTranslator::wrapLowLevelError($cause);

        self::assertNotSame('', $wrapped->getMessage());
    }
}
