"""Exercise 096 — a typed pub/sub event bus (expert).

Goal:   Dispatch events to handlers keyed by the event's own type, with the handler
        signature expressed as a generic `Protocol` rather than a base class every
        handler has to inherit from.
Drills: `Protocol[E]` for a callable shape (structural typing — anything with a
        matching `__call__` fits, no inheritance needed), a registry keyed by
        `type(event)` (exact type, not `isinstance` — a subclass's events do not
        reach a superclass's subscribers), and returning an `unsubscribe` closure
        instead of a separate `unsubscribe(handler)` method.
Passes: when `pytest exercises/04-expert/test_ex096_typed_event_bus.py` is green.
"""

from typing import Callable, Protocol, TypeVar

E = TypeVar("E")
E_contra = TypeVar("E_contra", contravariant=True)


class Handler(Protocol[E_contra]):
    """The shape a handler must have: callable with one event argument, no return
    value expected. A plain function or a lambda already satisfies this — nothing
    needs to inherit from `Handler`."""

    def __call__(self, event: E_contra) -> None: ...


class EventBus:
    """Routes each published event to every handler subscribed to its exact type."""

    def __init__(self) -> None:
        raise NotImplementedError

    def subscribe(self, event_type: type[E], handler: Handler[E]) -> Callable[[], None]:
        """Register `handler` for `event_type` and return an `unsubscribe` callable.

        Calling the returned callable removes this exact handler from `event_type`'s
        subscriber list. Calling it a second time raises ValueError — it is no
        longer subscribed.
        """
        raise NotImplementedError

    def publish(self, event: E) -> int:
        """Call every handler subscribed to `type(event)`, in the order they
        subscribed, passing `event` to each. Return how many handlers ran.

        A handler subscribed to one of `type(event)`'s *base* classes is not
        called — dispatch is keyed by the exact type, not `isinstance`.
        """
        raise NotImplementedError
