<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex037GeneratorsBasics;

require_once __DIR__ . '/Sequences.php';

use PHPUnit\Framework\TestCase;

final class SequencesTest extends TestCase
{
    public function testFibonacciCallDoesNotThrowBeforeIteration(): void
    {
        $generator = Sequences::fibonacci(5);

        self::assertInstanceOf(\Generator::class, $generator);
    }

    public function testFibonacciThrowsOnlyWhenIterated(): void
    {
        $generator = Sequences::fibonacci(5);

        $this->expectException(\RuntimeException::class);

        $generator->current();
    }

    public function testFibonacciYieldsExpectedSequence(): void
    {
        $result = iterator_to_array(Sequences::fibonacci(8));

        self::assertSame([0, 1, 1, 2, 3, 5, 8, 13], $result);
    }

    public function testFibonacciOfOne(): void
    {
        $result = iterator_to_array(Sequences::fibonacci(1));

        self::assertSame([0], $result);
    }

    public function testFibonacciOfZeroIsEmpty(): void
    {
        $result = iterator_to_array(Sequences::fibonacci(0));

        self::assertSame([], $result);
    }
}
