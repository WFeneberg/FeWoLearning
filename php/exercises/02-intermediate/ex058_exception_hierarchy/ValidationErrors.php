<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex058ExceptionHierarchy;

/*
Exercise 058 - Exception hierarchy and chaining (intermediate).

Goal:   Wrap a low-level exception into a domain-specific one while preserving the cause chain.
Drills: custom exception classes, extending a built-in base, exception chaining, getPrevious().
Passes: ValidationErrorsTest
*/
class ValidationException extends \DomainException
{
}

final class ErrorTranslator
{
    public static function wrapLowLevelError(\Throwable $cause): ValidationException
    {
        throw new \RuntimeException('TODO');
    }
}
