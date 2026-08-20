<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex025NestedArrayAccess;

final class NestedArrayAccess
{
    private function __construct()
    {
    }

    public static function getNested(array $data, array $path, mixed $default = null): mixed
    {
        $current = $data;

        foreach ($path as $key) {
            if (!is_array($current) || !array_key_exists($key, $current)) {
                return $default;
            }

            $current = $current[$key];
        }

        return $current;
    }
}
