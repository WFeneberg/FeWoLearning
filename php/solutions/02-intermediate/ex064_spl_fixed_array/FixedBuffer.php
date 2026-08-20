<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex064SplFixedArray;

final class FixedBuffer
{
    public static function buildFilled(int $size, mixed $fillValue): array
    {
        $buffer = new \SplFixedArray($size);

        for ($i = 0; $i < $size; $i++) {
            $buffer[$i] = $fillValue;
        }

        return $buffer->toArray();
    }
}
