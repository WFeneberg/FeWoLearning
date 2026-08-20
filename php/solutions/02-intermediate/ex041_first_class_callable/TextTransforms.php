<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex041FirstClassCallable;

final class TextTransforms
{
    public static function uppercaseAll(array $strings): array
    {
        return array_map(strtoupper(...), $strings);
    }

    public function suffixAll(array $strings, string $suffix): array
    {
        return array_map($this->appendSuffix(...), $strings, array_fill(0, count($strings), $suffix));
    }

    private function appendSuffix(string $s, string $suffix): string
    {
        return $s . $suffix;
    }
}
