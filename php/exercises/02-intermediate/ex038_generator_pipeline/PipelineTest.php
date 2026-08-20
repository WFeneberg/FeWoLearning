<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex038GeneratorPipeline;

require_once __DIR__ . '/Pipeline.php';

use PHPUnit\Framework\TestCase;

final class PipelineTest extends TestCase
{
    public function testMapTransformsEachItem(): void
    {
        $result = iterator_to_array(Pipeline::map([1, 2, 3], fn ($n) => $n * 2));

        self::assertSame([2, 4, 6], $result);
    }

    public function testFilterKeepsOnlyMatchingItems(): void
    {
        $result = iterator_to_array(Pipeline::filter([1, 2, 3, 4, 5], fn ($n) => $n % 2 === 0));

        self::assertSame([2, 4], array_values($result));
    }

    public function testMapAndFilterCompose(): void
    {
        $input = [1, 2, 3, 4, 5];

        $doubled = Pipeline::map($input, fn ($n) => $n * 2);
        $result = Pipeline::filter($doubled, fn ($n) => $n > 5);

        self::assertSame([6, 8, 10], array_values(iterator_to_array($result)));
    }

    public function testFilterOfEmptySourceIsEmpty(): void
    {
        $result = iterator_to_array(Pipeline::filter([], fn ($n) => true));

        self::assertSame([], $result);
    }

    public function testMapOverGeneratorSource(): void
    {
        $source = (function () {
            yield 1;
            yield 2;
            yield 3;
        })();

        $result = iterator_to_array(Pipeline::map($source, fn ($n) => $n + 10));

        self::assertSame([11, 12, 13], array_values($result));
    }
}
