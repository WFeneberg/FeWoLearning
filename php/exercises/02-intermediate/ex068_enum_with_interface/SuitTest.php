<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex068EnumWithInterface;

require_once __DIR__ . '/Suit.php';

use PHPUnit\Framework\TestCase;

final class SuitTest extends TestCase
{
    public function testHeartsLabel(): void
    {
        self::assertSame('Hearts', Suit::Hearts->label());
    }

    public function testDiamondsLabel(): void
    {
        self::assertSame('Diamonds', Suit::Diamonds->label());
    }

    public function testClubsLabel(): void
    {
        self::assertSame('Clubs', Suit::Clubs->label());
    }

    public function testSpadesLabel(): void
    {
        self::assertSame('Spades', Suit::Spades->label());
    }

    public function testEnumCaseImplementsHasLabel(): void
    {
        self::assertInstanceOf(HasLabel::class, Suit::Hearts);
    }
}
