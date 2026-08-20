<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex078WeakmapCache;

final class WeakMapCache
{
    private \WeakMap $map;

    public function __construct()
    {
        $this->map = new \WeakMap();
    }

    public function rememberFor(object $key, \Closure $compute): mixed
    {
        if (isset($this->map[$key])) {
            return $this->map[$key];
        }

        $value = $compute();
        $this->map[$key] = $value;

        return $value;
    }
}
