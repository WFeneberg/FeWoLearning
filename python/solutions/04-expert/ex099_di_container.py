"""Exercise 099 — a dependency injection container (reference solution)."""

import inspect
from contextlib import contextmanager
from enum import Enum, auto
from typing import Any, Callable, Iterator, TypeVar, get_type_hints

T = TypeVar("T")


class Lifetime(Enum):
    TRANSIENT = auto()
    SINGLETON = auto()
    SCOPED = auto()


class CircularDependencyError(Exception):
    pass


class Container:
    def __init__(self) -> None:
        self._registrations: dict[type, tuple[Callable[..., Any], Lifetime]] = {}
        self._singletons: dict[type, Any] = {}
        self._scoped_instances: dict[type, Any] | None = None
        self._resolving: set[type] = set()

    def register(
        self,
        interface: type[T],
        factory: Callable[..., T] | None = None,
        *,
        lifetime: Lifetime = Lifetime.TRANSIENT,
    ) -> None:
        self._registrations[interface] = (factory if factory is not None else interface, lifetime)

    def resolve(self, interface: type[T]) -> T:
        if interface not in self._registrations:
            raise KeyError(f"no registration for {interface!r}")
        if interface in self._resolving:
            chain = " -> ".join(t.__name__ for t in self._resolving)
            raise CircularDependencyError(f"circular dependency: {chain} -> {interface.__name__}")

        factory, lifetime = self._registrations[interface]

        if lifetime is Lifetime.SINGLETON and interface in self._singletons:
            return self._singletons[interface]  # type: ignore[no-any-return]

        if lifetime is Lifetime.SCOPED:
            if self._scoped_instances is None:
                raise RuntimeError(f"{interface!r} is scoped but no scope is active")
            if interface in self._scoped_instances:
                return self._scoped_instances[interface]  # type: ignore[no-any-return]

        self._resolving.add(interface)
        try:
            instance = self._build(factory)
        finally:
            self._resolving.discard(interface)

        if lifetime is Lifetime.SINGLETON:
            self._singletons[interface] = instance
        elif lifetime is Lifetime.SCOPED:
            assert self._scoped_instances is not None
            self._scoped_instances[interface] = instance

        return instance  # type: ignore[no-any-return]

    def _build(self, factory: Callable[..., Any]) -> Any:
        target = factory.__init__ if inspect.isclass(factory) else factory
        hints = get_type_hints(target)
        hints.pop("return", None)

        kwargs = {}
        for name in inspect.signature(factory).parameters:
            if name == "self":
                continue
            kwargs[name] = self.resolve(hints[name])
        return factory(**kwargs)

    @contextmanager
    def scope(self) -> Iterator["Container"]:
        previous = self._scoped_instances
        self._scoped_instances = {}
        try:
            yield self
        finally:
            self._scoped_instances = previous
