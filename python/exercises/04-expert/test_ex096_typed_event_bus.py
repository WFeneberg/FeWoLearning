from dataclasses import dataclass

import pytest

from ex096_typed_event_bus import EventBus


@dataclass
class UserCreated:
    user_id: int


@dataclass
class UserDeleted:
    user_id: int


class AdminCreated(UserCreated):
    pass


def test_publish_calls_the_registered_handler():
    bus = EventBus()
    received = []
    bus.subscribe(UserCreated, lambda event: received.append(event))

    bus.publish(UserCreated(user_id=1))

    assert received == [UserCreated(user_id=1)]


def test_publish_returns_the_number_of_handlers_called():
    bus = EventBus()
    bus.subscribe(UserCreated, lambda event: None)
    bus.subscribe(UserCreated, lambda event: None)

    assert bus.publish(UserCreated(user_id=1)) == 2


def test_publish_with_no_subscribers_returns_zero():
    bus = EventBus()
    assert bus.publish(UserCreated(user_id=1)) == 0


def test_handlers_run_in_subscription_order():
    bus = EventBus()
    order = []
    bus.subscribe(UserCreated, lambda event: order.append("first"))
    bus.subscribe(UserCreated, lambda event: order.append("second"))

    bus.publish(UserCreated(user_id=1))

    assert order == ["first", "second"]


def test_unsubscribe_stops_future_calls():
    bus = EventBus()
    received = []
    unsubscribe = bus.subscribe(UserCreated, lambda event: received.append(event))

    unsubscribe()
    bus.publish(UserCreated(user_id=1))

    assert received == []


def test_unsubscribing_twice_raises():
    bus = EventBus()
    unsubscribe = bus.subscribe(UserCreated, lambda event: None)
    unsubscribe()

    with pytest.raises(ValueError):
        unsubscribe()


def test_different_event_types_do_not_cross_trigger():
    bus = EventBus()
    created_received = []
    deleted_received = []
    bus.subscribe(UserCreated, lambda event: created_received.append(event))
    bus.subscribe(UserDeleted, lambda event: deleted_received.append(event))

    bus.publish(UserDeleted(user_id=1))

    assert created_received == []
    assert deleted_received == [UserDeleted(user_id=1)]


def test_dispatch_is_by_exact_type_not_inheritance():
    bus = EventBus()
    received = []
    bus.subscribe(UserCreated, lambda event: received.append(event))

    bus.publish(AdminCreated(user_id=1))

    # AdminCreated is a UserCreated subclass, but dispatch keys on the exact type —
    # a handler subscribed to the base class does not also see subclass events.
    assert received == []


def test_a_callable_object_works_as_a_handler_without_inheriting_anything():
    class Recorder:
        def __init__(self) -> None:
            self.events: list[UserCreated] = []

        def __call__(self, event: UserCreated) -> None:
            self.events.append(event)

    recorder = Recorder()
    bus = EventBus()
    bus.subscribe(UserCreated, recorder)

    bus.publish(UserCreated(user_id=7))

    assert recorder.events == [UserCreated(user_id=7)]


def test_multiple_event_buses_are_independent():
    bus_a = EventBus()
    bus_b = EventBus()
    received_a = []
    bus_a.subscribe(UserCreated, lambda event: received_a.append(event))

    bus_b.publish(UserCreated(user_id=1))

    assert received_a == []
