<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex078WeakmapCache;

require_once __DIR__ . '/WeakMapCache.php';

use PHPUnit\Framework\TestCase;

final class WeakMapCacheTest extends TestCase
{
    public function testComputesOnlyOnceForTheSameKey(): void
    {
        $cache = new WeakMapCache();
        $key = new \stdClass();
        $calls = 0;

        $compute = static function () use (&$calls): string {
            $calls++;

            return 'computed';
        };

        $first = $cache->rememberFor($key, $compute);
        $second = $cache->rememberFor($key, $compute);

        self::assertSame('computed', $first);
        self::assertSame('computed', $second);
        self::assertSame(1, $calls);
    }

    public function testDifferentKeysAreCachedIndependently(): void
    {
        $cache = new WeakMapCache();
        $keyA = new \stdClass();
        $keyB = new \stdClass();

        $resultA = $cache->rememberFor($keyA, static fn (): string => 'a');
        $resultB = $cache->rememberFor($keyB, static fn (): string => 'b');

        self::assertSame('a', $resultA);
        self::assertSame('b', $resultB);
    }

    public function testCachedValueCanBeFalsyWithoutRecomputing(): void
    {
        $cache = new WeakMapCache();
        $key = new \stdClass();
        $calls = 0;

        $compute = static function () use (&$calls): int {
            $calls++;

            return 0;
        };

        $cache->rememberFor($key, $compute);
        $cache->rememberFor($key, $compute);

        self::assertSame(1, $calls);
    }

    public function testNaiveAlwaysRecomputeWouldFailThisAssertion(): void
    {
        $cache = new WeakMapCache();
        $key = new \stdClass();
        $calls = 0;

        $compute = static function () use (&$calls): int {
            $calls++;

            return $calls;
        };

        $first = $cache->rememberFor($key, $compute);
        $second = $cache->rememberFor($key, $compute);

        self::assertSame($first, $second);
    }
}
