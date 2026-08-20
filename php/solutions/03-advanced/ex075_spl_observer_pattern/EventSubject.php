<?php

declare(strict_types=1);

namespace FeWoLearning\Exercises\Advanced\Ex075SplObserverPattern;

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
        $this->observers[spl_object_id($observer)] = $observer;
    }

    public function detach(Observer $observer): void
    {
        unset($this->observers[spl_object_id($observer)]);
    }

    public function notify(string $event): void
    {
        foreach ($this->observers as $observer) {
            $observer->update($event);
        }
    }
}
