"""Exercise 099 — a dependency injection container (expert).

Goal:   Resolve an object graph automatically from constructor type annotations —
        register what each interface is built from, ask for the outermost type, and
        let the container walk the dependency graph, recursively, on its own.
Drills: reading a callable's parameters and their type annotations to discover what
        it needs, three lifetimes (transient — a fresh instance every resolve;
        singleton — one instance for the container's whole life; scoped — one
        instance per `scope()` block, shared within it, fresh in the next one), and
        detecting a circular dependency by tracking what is *currently being
        resolved*, not just what is registered.
Passes: when `pytest exercises/04-expert/test_ex099_di_container.py` is green.

Note:   this mirrors the Transient/Scoped/Singleton lifetimes from ASP.NET Core's
        built-in DI container — same three lifetimes, same reason each exists,
        just resolved from `__init__` annotations instead of a compiled `Startup`.
"""

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
    """Raised when resolving a type would require resolving itself again."""


class Container:
    """A dependency injection container: register interfaces, resolve instances."""

    def __init__(self) -> None:
        """Set up empty registration, singleton-instance, and in-progress-resolution
        tracking. There is no active scope until `scope()` is entered."""
        raise NotImplementedError

    def register(
        self,
        interface: type[T],
        factory: Callable[..., T] | None = None,
        *,
        lifetime: Lifetime = Lifetime.TRANSIENT,
    ) -> None:
        """Register how to build `interface`.

        `factory` defaults to `interface` itself (a class registers as its own
        implementation, built via its own constructor). `factory` may also be a
        plain callable, such as a zero-argument function — anything `resolve` can
        inspect the parameters of.
        """
        raise NotImplementedError

    def resolve(self, interface: type[T]) -> T:
        """Return an instance of `interface`, building its whole dependency graph.

        - Unregistered `interface`: KeyError.
        - `interface` is already being resolved further up the current call stack:
          CircularDependencyError — this must be checked *before* recursing into
          its dependencies, not after they blow the real Python call stack.
        - SINGLETON: build once, ever; every later resolve of this `interface`
          (from any scope, or none) returns that same instance.
        - SCOPED: build once per `scope()` block, shared by every resolve of
          `interface` within that block; resolving a SCOPED interface with no
          active scope is a RuntimeError.
        - TRANSIENT: build fresh, every call.

        Building means: inspect `factory`'s parameters (its own signature if it is
        a plain function, its `__init__`'s if it is a class — skipping `self`),
        read each parameter's type annotation, `resolve` that type recursively, and
        call `factory` with the results as keyword arguments.
        """
        raise NotImplementedError

    @contextmanager
    def scope(self) -> Iterator["Container"]:
        """Open a scope: SCOPED instances resolved inside share one instance per
        interface, distinct from any other scope's. Scopes do not nest — entering
        one while another from the same container is active replaces it for the
        duration of the `with` block, then the previous one (if any) resumes."""
        raise NotImplementedError
