<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Intermediate\Ex048NamedArguments;

require_once __DIR__ . '/UserBuilder.php';

use PHPUnit\Framework\TestCase;

final class UserBuilderTest extends TestCase
{
    public function testBuildWithAllPositionalArguments(): void
    {
        $user = UserBuilder::build('Bob', 30, 'FR');

        self::assertSame(['name' => 'Bob', 'age' => 30, 'country' => 'FR'], $user);
    }

    public function testBuildWithOnlyNameUsesDefaults(): void
    {
        $user = UserBuilder::build('Ada');

        self::assertSame(['name' => 'Ada', 'age' => 18, 'country' => 'DE'], $user);
    }

    public function testBuildWithNameAndNamedCountrySkippingAge(): void
    {
        $user = UserBuilder::build(name: 'Ada', country: 'UK');

        self::assertSame(['name' => 'Ada', 'age' => 18, 'country' => 'UK'], $user);
    }

    public function testBuildWithAllNamedArgumentsInDifferentOrder(): void
    {
        $user = UserBuilder::build(country: 'US', name: 'Grace', age: 45);

        self::assertSame(['name' => 'Grace', 'age' => 45, 'country' => 'US'], $user);
    }
}
