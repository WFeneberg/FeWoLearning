<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex081LateStaticBinding;

require_once __DIR__ . '/Builder.php';

use PHPUnit\Framework\TestCase;

final class GadgetBuilder extends Builder
{
    public function describe(): string
    {
        return 'gadget';
    }
}

final class BuilderTest extends TestCase
{
    public function testCreateReturnsAnInstanceOfTheCallingSubclass(): void
    {
        $widget = WidgetBuilder::create();

        self::assertInstanceOf(WidgetBuilder::class, $widget);
    }

    public function testCreateOnADifferentSubclassReturnsThatSubclass(): void
    {
        $gadget = GadgetBuilder::create();

        self::assertInstanceOf(GadgetBuilder::class, $gadget);
        self::assertNotInstanceOf(WidgetBuilder::class, $gadget);
    }

    public function testCreatedInstanceDescribesItself(): void
    {
        $widget = WidgetBuilder::create();
        $gadget = GadgetBuilder::create();

        self::assertSame('widget', $widget->describe());
        self::assertSame('gadget', $gadget->describe());
    }

    public function testResultIsAlwaysAlsoInstanceOfTheBaseClass(): void
    {
        $widget = WidgetBuilder::create();

        self::assertInstanceOf(Builder::class, $widget);
    }
}
