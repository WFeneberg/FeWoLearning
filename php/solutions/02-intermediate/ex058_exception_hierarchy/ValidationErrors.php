<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex058ExceptionHierarchy;

class ValidationException extends \DomainException
{
}

final class ErrorTranslator
{
    public static function wrapLowLevelError(\Throwable $cause): ValidationException
    {
        return new ValidationException('validation failed', previous: $cause);
    }
}
