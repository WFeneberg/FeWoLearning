<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex076ReflectionBasics;

require_once __DIR__ . '/ReflectionInspector.php';

use PHPUnit\Framework\TestCase;

final class SampleTarget
{
    public function alpha(): void
    {
    }

    public function beta(): void
    {
    }

    protected function gamma(): void
    {
    }

    private function delta(): void
    {
    }
}

final class ReflectionInspectorTest extends TestCase
{
    public function testReturnsOnlyPublicMethodNames(): void
    {
        $names = ReflectionInspector::publicMethodNames(new SampleTarget());

        self::assertSame(['alpha', 'beta'], $names);
    }

    public function testExcludesPrivateAndProtectedMethods(): void
    {
        $names = ReflectionInspector::publicMethodNames(new SampleTarget());

        self::assertNotContains('gamma', $names);
        self::assertNotContains('delta', $names);
    }

    public function testResultIsSortedAlphabetically(): void
    {
        $names = ReflectionInspector::publicMethodNames(new SampleTarget());

        $sorted = $names;
        sort($sorted);

        self::assertSame($sorted, $names);
    }

    public function testObjectWithNoPublicMethodsReturnsEmptyArray(): void
    {
        $onlyPrivate = new class {
            private function hidden(): void
            {
            }
        };

        self::assertSame([], ReflectionInspector::publicMethodNames($onlyPrivate));
    }
}
