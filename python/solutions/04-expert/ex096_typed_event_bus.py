"""Exercise 096 — a typed pub/sub event bus (reference solution)."""

from collections import defaultdict
from typing import Any, Callable, Protocol, TypeVar

E = TypeVar("E")
E_contra = TypeVar("E_contra", contravariant=True)


class Handler(Protocol[E_contra]):
    def __call__(self, event: E_contra) -> None: ...


class EventBus:
    def __init__(self) -> None:
        self._handlers: dict[type, list[Callable[[Any], None]]] = defaultdict(list)

    def subscribe(self, event_type: type[E], handler: Handler[E]) -> Callable[[], None]:
        handlers = self._handlers[event_type]
        handlers.append(handler)

        def unsubscribe() -> None:
            handlers.remove(handler)

        return unsubscribe

    def publish(self, event: E) -> int:
        handlers = list(self._handlers.get(type(event), ()))
        for handler in handlers:
            handler(event)
        return len(handlers)
