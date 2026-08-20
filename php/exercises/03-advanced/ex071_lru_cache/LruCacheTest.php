<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex071LruCache;

require_once __DIR__ . '/LruCache.php';

use PHPUnit\Framework\TestCase;

final class LruCacheTest extends TestCase
{
    public function testEvictsLeastRecentlyUsedNotOldestInserted(): void
    {
        $cache = new LruCache(3);
        $cache->put('a', 1);
        $cache->put('b', 2);
        $cache->put('c', 3);

        // Refresh 'a' via get() so 'b' becomes the least-recently-used entry.
        self::assertSame(1, $cache->get('a'));

        $cache->put('d', 4);

        self::assertNull($cache->get('b'));
        self::assertSame(1, $cache->get('a'));
        self::assertSame(3, $cache->get('c'));
        self::assertSame(4, $cache->get('d'));
    }

    public function testGetOnMissingKeyReturnsNull(): void
    {
        $cache = new LruCache(2);

        self::assertNull($cache->get('missing'));
    }

    public function testPutOnExistingKeyUpdatesValue(): void
    {
        $cache = new LruCache(2);
        $cache->put('x', 1);
        $cache->put('x', 2);

        self::assertSame(2, $cache->get('x'));
    }

    public function testCapacityOneKeepsOnlyTheLatestEntry(): void
    {
        $cache = new LruCache(1);
        $cache->put('a', 1);
        $cache->put('b', 2);

        self::assertNull($cache->get('a'));
        self::assertSame(2, $cache->get('b'));
    }
}
