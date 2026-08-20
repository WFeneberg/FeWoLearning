<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex075SplObserverPattern;

require_once __DIR__ . '/EventSubject.php';

use PHPUnit\Framework\TestCase;

final class RecordingObserver implements Observer
{
    /** @var list<string> */
    public array $received = [];

    public function update(string $event): void
    {
        $this->received[] = $event;
    }
}

final class EventSubjectTest extends TestCase
{
    public function testNotifyReachesAllAttachedObservers(): void
    {
        $subject = new EventSubject();
        $first = new RecordingObserver();
        $second = new RecordingObserver();

        $subject->attach($first);
        $subject->attach($second);
        $subject->notify('created');

        self::assertSame(['created'], $first->received);
        self::assertSame(['created'], $second->received);
    }

    public function testDetachedObserverStopsReceivingEvents(): void
    {
        $subject = new EventSubject();
        $first = new RecordingObserver();
        $second = new RecordingObserver();

        $subject->attach($first);
        $subject->attach($second);
        $subject->notify('created');

        $subject->detach($first);
        $subject->notify('updated');

        self::assertSame(['created'], $first->received);
        self::assertSame(['created', 'updated'], $second->received);
    }

    public function testDetachingUnattachedObserverIsANoOp(): void
    {
        $subject = new EventSubject();
        $observer = new RecordingObserver();
        $unattached = new RecordingObserver();

        $subject->attach($observer);
        $subject->detach($unattached);
        $subject->notify('ping');

        self::assertSame(['ping'], $observer->received);
    }

    public function testAttachingSameObserverTwiceDoesNotDuplicateNotifications(): void
    {
        $subject = new EventSubject();
        $observer = new RecordingObserver();

        $subject->attach($observer);
        $subject->attach($observer);
        $subject->notify('ping');

        self::assertSame(['ping'], $observer->received);
    }
}
