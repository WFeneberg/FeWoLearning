"""Exercise 081 — __init_subclass__ and plugin registries (advanced).

Goal:   Make a base class notice its own subclasses, so plugins register themselves by
        the act of being defined — no decorator, no manual list, no metaclass.
Drills: `__init_subclass__` (implicitly a classmethod), keyword class arguments
        (``class Foo(Plugin, name="foo")``), rejecting duplicates, enforcing an
        interface at class-creation time, and a name-to-class factory.
Passes: when `pytest exercises/03-advanced/test_ex081_init_subclass_registry.py` is green.

Note:   `__init_subclass__` runs on the *base*, once per subclass, at ``class`` statement
        time — not on the base itself. It receives the new class as its first argument
        plus any extra keywords from the class header, and it must call
        ``super().__init_subclass__(**kwargs)`` so cooperative bases further up the MRO
        still get their turn.

Note:   the subclasses live in the test file and are defined *inside* test bodies. A
        module-scope subclass would fire `__init_subclass__` at import time, so an
        unfinished stub would break collection instead of failing a test — and the
        registry is shared class state that each test needs to start clean.
"""

from typing import Any


class Plugin:
    """Base class for self-registering plugins.

    Subclasses opt in with a name::

        class Csv(Plugin, name="csv"):
            def run(self, payload: str) -> str: ...

    and opt out with ``register=False``, which is how intermediate abstract layers avoid
    ending up in the registry.
    """

    registry: dict[str, type["Plugin"]] = {}

    def __init_subclass__(cls, /, name: str | None = None, register: bool = True, **kwargs: Any) -> None:
        """Register `cls` under `name`.

        Rules, in order:

        - forward `**kwargs` to `super().__init_subclass__`;
        - with ``register=False``, do nothing else — and a `name` given anyway is a
          ValueError, since it would be silently ignored;
        - a registering subclass must override `run`, directly or via an ancestor
          (``cls.run is Plugin.run`` means it did not), else TypeError — the cheap
          interface check `abc` charges more for;
        - a missing `name` defaults to the class name lowercased;
        - a name already in the registry raises ValueError, so two plugins cannot
          shadow each other;
        - store the name on the class as `plugin_name` too.
        """
        raise NotImplementedError

    def run(self, payload: str) -> str:
        """What a plugin does. Subclasses override this."""
        raise NotImplementedError

    @classmethod
    def reset_registry(cls) -> None:
        """Empty the registry. Test hygiene, not production code."""
        raise NotImplementedError

    @classmethod
    def available(cls) -> list[str]:
        """The registered names, sorted."""
        raise NotImplementedError

    @classmethod
    def lookup(cls, name: str) -> type["Plugin"]:
        """Return the class registered under `name`, or raise KeyError."""
        raise NotImplementedError

    @classmethod
    def create(cls, name: str, *args: Any, **kwargs: Any) -> "Plugin":
        """Instantiate the plugin registered under `name`, forwarding the arguments."""
        raise NotImplementedError


def run_all(payload: str) -> dict[str, str]:
    """Run every registered plugin on `payload`, keyed by registered name.

    The dispatch a registry buys you: this function names no plugin at all.
    """
    raise NotImplementedError
