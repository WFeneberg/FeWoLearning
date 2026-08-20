<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex009FizzBuzz;

final class FizzBuzz
{
    private function __construct()
    {
    }

    public static function fizzBuzz(int $n): array
    {
        $result = [];

        for ($i = 1; $i <= $n; $i++) {
            $result[] = match (true) {
                $i % 15 === 0 => 'FizzBuzz',
                $i % 3 === 0 => 'Fizz',
                $i % 5 === 0 => 'Buzz',
                default => (string) $i,
            };
        }

        return $result;
    }
}
