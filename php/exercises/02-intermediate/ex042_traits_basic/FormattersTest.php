<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex042TraitsBasic;

require_once __DIR__ . '/Formatters.php';

use PHPUnit\Framework\TestCase;

final class FormattersTest extends TestCase
{
    public function testShoutUsesLoudTraitVersion(): void
    {
        $announcer = new Announcer();

        self::assertSame('HELLO!', $announcer->shout('hello'));
    }

    public function testWhisperUsesQuietTraitVersion(): void
    {
        $announcer = new Announcer();

        self::assertSame('(hello)', $announcer->whisper('HELLO'));
    }

    public function testShoutAndWhisperAreIndependent(): void
    {
        $announcer = new Announcer();

        self::assertNotSame($announcer->shout('test'), $announcer->whisper('test'));
    }

    public function testShoutOnDifferentInput(): void
    {
        $announcer = new Announcer();

        self::assertSame('PHP RULES!', $announcer->shout('php rules'));
    }

    public function testWhisperOnDifferentInput(): void
    {
        $announcer = new Announcer();

        self::assertSame('(php rules)', $announcer->whisper('PHP RULES'));
    }
}
