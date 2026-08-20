<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Beginner\Ex025NestedArrayAccess;

require_once __DIR__ . '/NestedArrayAccess.php';

use PHPUnit\Framework\TestCase;

final class NestedArrayAccessTest extends TestCase
{
    private array $data;

    protected function setUp(): void
    {
        $this->data = [
            'user' => [
                'name' => 'Ada',
                'address' => [
                    'city' => 'London',
                ],
            ],
        ];
    }

    public function testExistingPathReturnsValue(): void
    {
        self::assertSame('London', NestedArrayAccess::getNested($this->data, ['user', 'address', 'city']));
    }

    public function testMissingIntermediateKeyReturnsDefault(): void
    {
        self::assertSame(
            'fallback',
            NestedArrayAccess::getNested($this->data, ['user', 'missing', 'city'], 'fallback')
        );
    }

    public function testScalarIntermediateValueReturnsDefaultInsteadOfError(): void
    {
        self::assertSame(
            'fallback',
            NestedArrayAccess::getNested($this->data, ['user', 'name', 'first'], 'fallback')
        );
    }

    public function testDefaultDefaultsToNull(): void
    {
        self::assertNull(NestedArrayAccess::getNested($this->data, ['user', 'missing']));
    }

    public function testEmptyPathReturnsWholeData(): void
    {
        self::assertSame($this->data, NestedArrayAccess::getNested($this->data, []));
    }
}
