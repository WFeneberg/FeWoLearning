<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex046EnumsBacked;

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
        return match ($status) {
            StatusCode::Ok => 'OK',
            StatusCode::NotFound => 'Not Found',
            StatusCode::ServerError => 'Server Error',
        };
    }

    public static function fromValue(int $value): ?StatusCode
    {
        return StatusCode::tryFrom($value);
    }
}
