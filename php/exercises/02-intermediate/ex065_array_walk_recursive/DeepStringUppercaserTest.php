<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex065ArrayWalkRecursive;

require_once __DIR__ . '/DeepStringUppercaser.php';

use PHPUnit\Framework\TestCase;

final class DeepStringUppercaserTest extends TestCase
{
    public function testUppercasesTopLevelStrings(): void
    {
        $result = DeepStringUppercaser::uppercaseAllStrings(['a' => 'hello']);

        self::assertSame(['a' => 'HELLO'], $result);
    }

    public function testUppercasesNestedStringsAtEveryDepth(): void
    {
        $data = [
            'name' => 'alice',
            'meta' => [
                'city' => 'berlin',
                'tags' => ['admin', 'active'],
            ],
        ];

        $result = DeepStringUppercaser::uppercaseAllStrings($data);

        self::assertSame([
            'name' => 'ALICE',
            'meta' => [
                'city' => 'BERLIN',
                'tags' => ['ADMIN', 'ACTIVE'],
            ],
        ], $result);
    }

    public function testNonStringLeavesAreUntouched(): void
    {
        $data = ['count' => 42, 'label' => 'items'];

        $result = DeepStringUppercaser::uppercaseAllStrings($data);

        self::assertSame(42, $result['count']);
        self::assertSame('ITEMS', $result['label']);
    }

    public function testOriginalArrayIsNotMutated(): void
    {
        $data = ['a' => ['b' => 'value']];

        DeepStringUppercaser::uppercaseAllStrings($data);

        self::assertSame(['a' => ['b' => 'value']], $data);
    }
}
