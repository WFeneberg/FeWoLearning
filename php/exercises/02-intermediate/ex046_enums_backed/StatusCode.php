<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex046EnumsBacked;

/*
Exercise 046 - Backed enums (intermediate).

Goal:   Implement a human-readable label lookup and a safe value-to-case
        conversion for a backed enum.
Drills: backed enum: int, ::from()/::tryFrom().
Passes: StatusCodeTest
*/
enum StatusCode: int
{
    case Ok = 200;
    case NotFound = 404;
    case ServerError = 500;
}

final class StatusCodes
{
    public static function label(StatusCode $status): string
    {
        throw new \RuntimeException('TODO');
    }

    public static function fromValue(int $value): ?StatusCode
    {
        throw new \RuntimeException('TODO');
    }
}
