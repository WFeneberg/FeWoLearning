"""Exercise 082 — metaclasses and class-creation hooks (advanced).

Goal:   Enforce rules on a class *at class-statement time* — before the class object
        is even usable — using a metaclass instead of `__init_subclass__`
        (`ex081_init_subclass_registry.py`). The distinguishing power a metaclass has
        that a hook does not: it can intercept the class body before `type` builds the
        class, and it can intercept *instantiation* by overriding `__call__`.
Drills: `type.__new__`, metaclass keyword arguments, inspecting the class `namespace`
        before creation, deriving a value from the class name, a registry keyed by
        that value, and blocking instantiation of "abstract" classes via
        `Meta.__call__`.
Passes: when `pytest exercises/03-advanced/test_ex082_metaclass_enforcement.py` is green.

Note:   unlike `__init_subclass__`, a metaclass's `__new__` fires for *every* class
        that uses it — including the very first one, the abstract base itself. So
        there is no module-level `Component` class here: even the base class's
        creation would call this stub's `__new__` at import time and break
        collection. The base and every concrete subclass are built inside the test
        file's fixtures/bodies instead, where creating them is part of the test run
        rather than the import.
"""

import re
from typing import Any


def camel_to_kebab(name: str) -> str:
    """Convert ``PascalCase`` or ``camelCase`` to ``kebab-case``.

    ``HTTPServer`` -> ``http-server``, ``UserProfile`` -> ``user-profile``. You do not
    need to handle anything beyond ASCII letters and digits.
    """
    raise NotImplementedError


class ComponentMeta(type):
    """Metaclass enforcing structure on every `Component` subclass.

    Registry, keyed by slug, of every concrete (non-abstract) class ever created.
    """

    registry: dict[str, type] = {}

    def __new__(
        mcls,
        name: str,
        bases: tuple[type, ...],
        namespace: dict[str, Any],
        *,
        abstract: bool = False,
        slug: str | None = None,
        **kwargs: Any,
    ) -> type:
        """Build the class, then enforce the rules below, in order.

        - Build `cls` via `super().__new__`, forwarding `**kwargs`.
        - The class name must be PascalCase (starts with an uppercase ASCII letter) —
          else TypeError. Checked for every class, abstract or not.
        - Record `cls._abstract = abstract`.
        - If `abstract` is true, return `cls` now — no `kind` check, no registration,
          no slug.
        - A concrete class must resolve `kind` somewhere on its MRO (its own namespace
          or an inherited one) — else TypeError. (`hasattr(cls, "kind")` after
          construction is enough; class attributes resolve through the MRO already.)
        - Compute its slug: the `slug` keyword if given, else `camel_to_kebab(name)`.
        - A slug already present in `registry` is a ValueError naming both the new
          class and the class that holds it.
        - Store the slug as `cls.slug` and register `cls` under it.
        """
        raise NotImplementedError

    def __call__(cls, *args: Any, **kwargs: Any) -> Any:
        """Block instantiating an abstract component.

        Raise TypeError naming the class *before* `__init__` would run — an abstract
        class's `__init__` must never execute. Otherwise defer to `type.__call__`.
        """
        raise NotImplementedError


def reset_registry() -> None:
    """Empty `ComponentMeta.registry`. Test hygiene, not production code."""
    raise NotImplementedError
