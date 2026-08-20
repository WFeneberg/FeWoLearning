<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex013ExceptionHandling;

final class ExceptionHandling
{
    public static int $finallyRuns = 0;

    private function __construct()
    {
    }

    public static function safeDivide(int $numerator, int $denominator): array
    {
        try {
            intdiv($numerator, $denominator);

            return [
                'result' => (float) $numerator / $denominator,
                'error' => null,
            ];
        } catch (\DivisionByZeroError $e) {
            return [
                'result' => null,
                'error' => 'division by zero',
            ];
        } finally {
            self::$finallyRuns++;
        }
    }
}
