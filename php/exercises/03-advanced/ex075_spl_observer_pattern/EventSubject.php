<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex075SplObserverPattern;

/*
Exercise 075 - Observer pattern with attach/detach/notify (advanced).

Goal:   Implement a Subject that broadcasts events to attached Observers.
Drills: observer pattern, spl_object_id(), array keyed by object identity.
Passes: EventSubjectTest
*/
interface Observer
{
    public function update(string $event): void;
}

final class EventSubject
{
    /** @var array<int, Observer> */
    private array $observers = [];

    public function attach(Observer $observer): void
    {
        throw new \RuntimeException('TODO');
    }

    public function detach(Observer $observer): void
    {
        throw new \RuntimeException('TODO');
    }

    public function notify(string $event): void
    {
        throw new \RuntimeException('TODO');
    }
}
